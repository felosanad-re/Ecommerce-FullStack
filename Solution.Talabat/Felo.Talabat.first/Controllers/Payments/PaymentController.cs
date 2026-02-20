using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;
using Talabat.Core.Entites.Orders;
using Talabat.Core.Services.Contract.OrderService;
using Talabat.Core.Services.Contract.PaymentsService;

namespace Felo.Talabat.Api.Controllers.Payments
{
    public class PaymentController(
        IOrderServices orderServices,
        IPaymentService paymentService,
        ILogger<PaymentController> logger,
        IConfiguration configuration) : BaseController
    {
        private readonly IOrderServices _orderServices = orderServices;
        private readonly IPaymentService _paymentService = paymentService;
        private readonly ILogger<PaymentController> _logger = logger;
        private readonly IConfiguration _configuration = configuration;

        [HttpPost("createCheckoutSession/{orderId}")]
        public async Task<IActionResult> CreateCheckoutSession(int orderId)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail)) return Unauthorized();

            var order = await _orderServices.GetOrder(orderId, userEmail);
            if (order == null) return NotFound("Order Not Found");

            // Left Side [Show Product Items]
            var lineItems = new List<SessionLineItemOptions>();

            // Get Details For Items in cart
            foreach (var item in order.Items)
            {
                lineItems.Add(new SessionLineItemOptions
                {
                    Quantity = item.Count,
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "egp",
                        UnitAmount = (long)(item.Price * 100),

                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Name,
                            //Description = $"Qty: {item.Product.}",
                            Images = new List<string> { $"{_configuration["ProductionBaseUrl"]}/{item.Product.PictureUrl}"}
                        }
                    }
                });
            }
            // Get Delivary Cost
            lineItems.Add(new SessionLineItemOptions
            {
                Quantity = 1,
                PriceData = new SessionLineItemPriceDataOptions
                {
                    Currency = "egp",
                    UnitAmount = (long)(order.DelivaryMethod.Cost * 100),
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = "Delivery",
                    }
                }
            });
            // Right Side [Payment]
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = lineItems,
                Mode = "payment",
                SuccessUrl = "http://localhost:4200/paymentSuccess",
                CancelUrl = "http://localhost:4200/paymentFaild",
                Metadata = new Dictionary<string, string>
                {
                    { "orderId", order.Id.ToString() },
                    { "userEmail", userEmail }
                }
            };

            var service = new SessionService();
            var session = await service.CreateAsync(options);

            return Ok(new
            {
                checkoutUrl = session.Url,
                sessionId = session.Id
            });
        }

        [HttpPost("webhook")]
        [IgnoreAntiforgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> HandleWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            const string endpointSecret = "whsec_921fcccd2945bdc1255a4506dc77623f7740ac4b25c9e1a0763c32ff33738215"; // From Stripe Dashboard

            try
            {
                var signatureHeader = Request.Headers["Stripe-Signature"];

                // أول حاجة: تحقق من التوقيع
                var stripeEvent = EventUtility.ConstructEvent(
                    json,
                    signatureHeader,
                    endpointSecret
                );

                // دلوقتي الـ event موثوق

                if (stripeEvent.Data.Object is Stripe.Checkout.Session session)
                {
                    // استخراج البيانات بأمان
                    if (!session.Metadata.TryGetValue("orderId", out var orderIdStr) ||
                        !int.TryParse(orderIdStr, out var orderId))
                    {
                        _logger.LogWarning("Webhook received without valid order_id in metadata");
                        return Ok();  // رجع 200 عشان Stripe ما يعيدش الإرسال
                    }

                    string userEmail = null;
                    session.Metadata.TryGetValue("userEmail", out userEmail);  // optional

                    bool succeeded;

                    switch (stripeEvent.Type)
                    {
                        case EventTypes.CheckoutSessionCompleted:
                        case EventTypes.CheckoutSessionAsyncPaymentSucceeded:
                            succeeded = true;
                        break;

                        case EventTypes.CheckoutSessionAsyncPaymentFailed:
                            succeeded = false;
                        break;

                        default:
                            _logger.LogInformation("Unhandled checkout event type: {Type}", stripeEvent.Type);
                        return Ok();
                    }

                    var order = await _paymentService.UpdateOrderState(orderId, succeeded);

                    if (succeeded)
                    {
                        _logger.LogInformation("Payment succeeded for order {OrderId} - user: {Email}", orderId, userEmail);
                    }
                    else
                    {
                        _logger.LogWarning("Payment failed for order {OrderId} - user: {Email}", orderId, userEmail);
                    }

                    return Ok();
                }
                else
                {
                    // event مش Checkout Session (ممكن payment_intent, charge, إلخ)
                    _logger.LogDebug("Received non-checkout event: {Type}", stripeEvent.Type);
                    return Ok();
                }
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, "Stripe signature verification failed or invalid event");
                return BadRequest();  // أو Ok() لو عايز تتجاهل
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in webhook");
                return StatusCode(500);
            }
        }
    }
}

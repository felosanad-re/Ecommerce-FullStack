using AutoMapper;
using Felo.Talabat.Api.ModelDto.Products;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Talabat.Core.Entites.Brands;
using Talabat.Core.Entites.Carts;
using Talabat.Core.Entites.Categories;
using Talabat.Core.GenaricRepo;
using Talabat.Core.Services.Contract.ProductServices;
using Talabat.Core.Specifications.SpecModel;

namespace Felo.Talabat.Api.Controllers.Products
{
    public class ProductsController : BaseController
    {
        private readonly IProductService _productService;
        private readonly IBrandService _brandService;
        private readonly ICategoryService _categoryService;
        private readonly IRedisRepo<Cart> _cartRepo;
        private readonly IMapper _mapper;

        public ProductsController(IProductService productService, IMapper mapper, IBrandService brandService, ICategoryService categoryService, IRedisRepo<Cart> cartRepo)
        {
            _productService = productService;
            _mapper = mapper;
            _brandService = brandService;
            _categoryService = categoryService;
            _cartRepo = cartRepo;
        }

        // GET: /api/Products
        [HttpGet]
        public async Task<ActionResult<Pagination<IReadOnlyList<ProductToReturnDto>>>> Get([FromQuery] ProductParams productParams)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // To Get Cart
            var cartId = $"Cart:{userId}";
            var products = await _productService.GetProductsAsync(productParams);
            var data = _mapper.Map<IReadOnlyList<ProductToReturnDto>>(products);

            // Get Cart To Check If User Add This Product To His Carts
            var cart = await _cartRepo.GetCacheAsync(cartId);
            if(cart?.Items.Count > 0)
            {
                var productInCart = cart.Items.Select(i => i.Id).ToHashSet();
                foreach (var item in data)
                {
                    item.IsAddedToCart = productInCart.Contains(item.Id);
                }
            }

            var count = await _productService.GetProductCountAsync(productParams);
            return Ok(new Pagination<ProductToReturnDto>(productParams.PageSize, productParams.PageIndex, data, count));
        }

        // GET api/Products/5
        [HttpGet("{productId}")]
        public async Task<ActionResult<ProductToReturnDto>> Get(int productId)
        {
            var product = await _productService.GetProductAsync(productId);

            return Ok(_mapper.Map<ProductToReturnDto>(product));
        }

        [HttpGet("Brands")] // Get: /api/Products/Brands
        public async Task<ActionResult<Brand>> GetBrands()
        {
            var brands = await _brandService.GetAllAsync();
            return Ok(brands);
        }

        [HttpGet("Categories")] // Get: /api/Products/Categories
        public async Task<ActionResult<Category>> GetCategories()
        {
            var categories = await _categoryService.GetAll();
            return Ok(categories);
        }
    }
}

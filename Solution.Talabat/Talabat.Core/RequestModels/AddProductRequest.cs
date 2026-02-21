using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Talabat.Core.RequestModels
{
    public class AddProductRequest
    {
        [Required(ErrorMessage = "Product Name is required")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Product Description is required")]
        public string Descripaion { get; set; }
        [Required(ErrorMessage = "Product picture is required")]
        public IFormFile ProductPic { get; set; }
        public string? PictureUrl { get; set; }
        [Required(ErrorMessage = "Product Price is required")]
        public decimal Price { get; set; }
        [Required(ErrorMessage = "Product Brand is required")]
        public int BrandId { get; set; }
        [Required(ErrorMessage = "Product Category is required")]
        public int CategoryId { get; set; }
        [Required(ErrorMessage = "Product Stock is required")]
        public int Stock { get; set; }
    }
}

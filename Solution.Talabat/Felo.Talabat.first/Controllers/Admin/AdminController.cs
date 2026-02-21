using AutoMapper;
using Felo.Talabat.Api.ModelDto.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Talabat.Core;
using Talabat.Core.Entites.Brands;
using Talabat.Core.Entites.Categories;
using Talabat.Core.RequestModels;
using Talabat.Core.RequestModels.BrandRequests;
using Talabat.Core.RequestModels.CategoriesRequests;
using Talabat.Core.Services.Contract.AttachmentService;
using Talabat.Core.Services.Contract.ProductServices;
using Talabat.Core.Specifications.SpecModel;


namespace Felo.Talabat.Api.Controllers.Admin
{
    [Authorize]
    [Authorize(Roles = SD.SUPER_ADMIN + "," + SD.ADMIN)]
    public class AdminController : BaseController
    {
        #region Services
        private readonly IProductService _productService;
        private readonly IBrandService _brandService;
        private readonly ICategoryService _categoryService;
        private readonly IAttachmentService _attachmentService;
        private readonly IMapper _mapper;
        public AdminController(IProductService productService, IMapper mapper, IBrandService brandService, ICategoryService categoryService, IAttachmentService attachmentService)
        {
            _productService = productService;
            _brandService = brandService;
            _categoryService = categoryService;
            _mapper = mapper;
            _attachmentService = attachmentService;
        }
        #endregion


        #region GetProducts
        // GET: /api/Admin/GetProducts
        [HttpGet("GetProducts")]
        public async Task<ActionResult<Pagination<IReadOnlyList<ProductToReturnDto>>>> Get([FromQuery] ProductParams productParams)
        {
            var products = await _productService.GetProductsAsync(productParams);
            var data = _mapper.Map<IReadOnlyList<ProductToReturnDto>>(products);
            var count = await _productService.GetProductCountAsync(productParams);
            return Ok(new Pagination<ProductToReturnDto>(productParams.PageSize, productParams.PageIndex, data, count));
        }
        #endregion

        #region add Product
        [HttpPost("AddProduct")] // Post: /api/Admin/AddProduct
        public async Task<ActionResult<ProductToReturnDto>> AddProduct([FromForm] AddProductRequest addProductRequest)
        {
            string? productPic = null;
            if(addProductRequest.ProductPic != null && addProductRequest.ProductPic.Length > 0)
            {
                try
                {
                   var fileName = await _attachmentService.UploadAsync(addProductRequest.ProductPic, "products");
                    productPic = $"/files/products/{fileName}";
                }
                catch (Exception ex)
                {
                    return BadRequest("Failed to upload image: " +ex.Message);
                }
            }
            addProductRequest.PictureUrl = productPic;
            var product = await _productService.AddProductAsync(addProductRequest);
            return Ok(_mapper.Map<ProductToReturnDto>(product));
        }
        #endregion

        #region Edit Product
        [HttpPut("UpdateProduct")] // Put: /api/Admin/UpdateProduct
        public async Task<ActionResult<ProductToReturnDto>> UpdateProduct([FromBody]UpdateProductRequest updateProductRequest)
        {
            var updateProduct = await _productService.UpdateProduct(updateProductRequest);
            return Ok(_mapper.Map<ProductToReturnDto>(updateProduct));
        }
        #endregion

        #region Delete Product
        [HttpDelete("DeleteProduct")] // Delete : /api/Admin/DeleteProduct
        public async Task<ActionResult> Delete([FromQuery]int id)
        {
            await _productService.DeleteProduct(id);
            return Ok(new
            {
                Message = "Product Deleted Succedded"
            });
        }
        #endregion

        #region Get Product By Id
        [HttpGet("{productId}")]
        public async Task<ActionResult<ProductToReturnDto>> Get(int productId)
        {
            var product = await _productService.GetProductAsync(productId);
            return Ok(_mapper.Map<ProductToReturnDto>(product));
        }
        #endregion

        #region Get Brands
        [HttpGet("Brands")] // Get: /api/Admin/Brands
        public async Task<ActionResult<IReadOnlyList< Brand>>> GetBrand()
        {
            var brands = await _brandService.GetAllAsync();
            return Ok(brands);
        }
        #endregion

        #region Add Brand
        [HttpPost("addBrand")] // Post: /api/Admin/addBrand
        public async Task<ActionResult<Brand>> AddBrand([FromBody] BrandRequest brand)
        {
            var addedbrand = await _brandService.AddAsync(brand);
            return Ok(addedbrand);
        }
        #endregion

        #region Edit Brand
        [HttpPut("EditBrand")] // Put: /api/Admin/EditBrand
        public async Task<ActionResult<Brand>> EditBrand(UpdateBrandRequest updateBrandRequest)
        {
            var editBrand = await _brandService.UpdateAsync(updateBrandRequest);
            if (editBrand == null) return BadRequest();
            return Ok(editBrand);
        }
        #endregion

        #region DeleteBrand
        [HttpDelete("DeleteBrand")] // Delete: /api/Admin/DeleteBrand
        public async Task<ActionResult<Brand>> DeleteBrand(int id)
        {
            await _brandService.DeleteBrandAsync(id);

            return Ok(new
            {
                Message = "Brand Delete Succssesfully"
            });
        }
        #endregion

        #region Get All Categories
        [HttpGet("Categories")] // Get: /api/Admin/Categories
        public async Task<ActionResult<IReadOnlyList<Category>>> Get()
        {
            var categories = await _categoryService.GetAll();
            if (categories is null) return NotFound(new
            {
                Message = "There is No Categories"
            });

            return Ok(categories);
        }
        #endregion

        #region Add Category
        [HttpPost("AddCategory")] // Post: /api/Admin/AddCategory
        public async Task<ActionResult<Category>> AddCategory(AddCategoryRequest request)
        {
            var category = await _categoryService.AddCategoryAsync(request);
            if(category is null) return BadRequest(new
            {
                Message = "Category Not Created"
            });
            return Ok(category);
        }
        #endregion

        #region Edit Category
        [HttpPut("EditCategory")] // Post: /api/Admin/EditCategory
        public async Task<ActionResult<Category>> EditCategory(UpdateCategoryRequest request)
        {
            var category = await _categoryService.UpdateCategoryAsync(request);
            if(category is null) return BadRequest(new
            {
                Message = "Update Created Not Applyed"
            });
            return Ok(category);
        }
        #endregion

        #region DeleteCategory
        [HttpDelete("DeleteCategory")] // Post: /api/Admin/DeleteCategory
        public async Task<ActionResult> DeleteCategory(int id)
        {
             await _categoryService.DeleteCategoryAsync(id);

            return Ok(new
            {
                Message = "Category Is Deleted" // change WHen I Apply Soft Delete
            });
        }
        #endregion
    }
}

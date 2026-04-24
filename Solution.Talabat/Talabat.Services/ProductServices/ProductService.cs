using AutoMapper;
using Microsoft.Extensions.Logging;
using Talabat.Core.Entites.Products;
using Talabat.Core.RequestModels;
using Talabat.Core.RequestModels.Import;
using Talabat.Core.RequestModels.Products;
using Talabat.Core.ResponseModel.Import;
using Talabat.Core.Services.Contract.ImportServices;
using Talabat.Core.Services.Contract.ProductServices;
using Talabat.Core.Specifications.SpecModel;
using Talabat.Core.UnitOfWork;

namespace Talabat.Services.ProductServices
{
    public class ProductService : IProductService
    {
        #region Services

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IimportService _iimportService;
        private readonly ILogger<ProductService> _logger;
        public ProductService(IUnitOfWork unitOfWork, IMapper mapper, IimportService iimportService, ILogger<ProductService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _iimportService = iimportService;
            _logger = logger;
        }
        #endregion

        #region User Services
        // Get Products
        public async Task<IReadOnlyList<Product>> GetProductsAsync(ProductParams productParams)
        {
            var spec = new ProductSpecifications(productParams);
            var products = await _unitOfWork.RepositaryAsync<Product>().GetAllAsyncSpec(spec);
            return products;
        }

        // Get Product By Id
        public async Task<Product?> GetProductAsync(int productId)
        {
            var spec = new ProductSpecifications(productId);
            var product = await _unitOfWork.RepositaryAsync<Product>().GetSpec(spec);
            return product;
        }

        // Get Product Count Before Pagination
        public async Task<int> GetProductCountAsync(ProductParams productParams)
        {
            var spec = new ProductCountSpec(productParams);
            return await _unitOfWork.RepositaryAsync<Product>().CountAsyncSpec(spec);
        } 
        #endregion

        #region Admin Region

        public async Task<Product?> AddProductAsync(AddProductRequest addProductRequest)
        {
            // Auto Mapper
            var newProduct = _mapper.Map<Product>(addProductRequest);
            await _unitOfWork.RepositaryAsync<Product>().AddAsync(newProduct);
            await _unitOfWork.CompleteAsync();

            return await GetProductAsync(newProduct.Id);
        }

        public async Task<Product?> UpdateProduct(UpdateProductRequest updateProductRequest)
        {
            var updateProduct = await _unitOfWork.RepositaryAsync<Product>().Get(updateProductRequest.Id);
            if (updateProduct == null) return null;
             // Auto Mapper
            _mapper.Map(updateProductRequest, updateProduct);
            await _unitOfWork.CompleteAsync();
            return await GetProductAsync(updateProduct.Id);
        }

        public async Task<bool> DeleteProduct(int productId)
        {
            var product = await _unitOfWork.RepositaryAsync<Product>().Get(productId);
            if(product == null) return false;
            _unitOfWork.RepositaryAsync<Product>().delete(product);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<IReadOnlyList<ProductExportToReturn>> GetProductForExport()
        {
            var data = await _unitOfWork.RepositaryAsync<Product>().GetAllAsyncSpec(new ProductSpecifications());
            var result = _mapper.Map<IReadOnlyList<ProductExportToReturn>>(data);
            return result;
        }

        public async Task<ImportToReturnDTO<ProductImportToReturnDTO>> GetProductsForImportAsync(ImportDTO<ProductImportToReturnDTO> req)
        {
            try
            {
                // Read the Excel sheet into DTO rows using the shared import service.
                var productImport = await _iimportService.ExcelImportAsync(new ImportDTO<ProductImportToReturnDTO>
                {
                    File = req.File,
                    Config = BuildImportConfig<ProductImportToReturnDTO>("Products")
                });

                var excelRows = productImport.Data;
                var newProductsToSave = new List<(Product Entity, ProductImportToReturnDTO SourceRow)>();
                var importedProducts = new List<ProductImportToReturnDTO>();
                var errors = new List<string>();
                var productRepo = _unitOfWork.RepositaryAsync<Product>();

                // Load existing products once, then update them in memory to avoid one query per row.
                var existingProductsIds = excelRows.Where(x => x.Id > 0).Select(r => r.Id).Distinct().ToList();
                var existingProductsDict = new Dictionary<int, Product>();

                if (existingProductsIds.Any())
                {
                    var spec = new ProductSpecifications(existingProductsIds);
                    var existingList = await productRepo.GetAllAsyncSpec(spec);
                    existingProductsDict = existingList.ToList().ToDictionary(p => p.Id);
                }

                foreach (var row in excelRows)
                {
                    if (string.IsNullOrWhiteSpace(row.Name))
                    {
                        errors.Add($"Row with Id {row.Id}: Product name is required");
                        continue;
                    }

                    if (row.Id > 0 && existingProductsDict.TryGetValue(row.Id, out var existing))
                    {
                        // Existing Id means update the current database entity.
                        MapImportRowToProduct(row, existing);
                        productRepo.Update(existing);
                        importedProducts.Add(row);
                    }
                    else
                    {
                        // Missing/unknown Id means create a brand new product.
                        var newProduct = MapImportRowToNewProduct(row);
                        newProduct.Id = 0;
                        newProductsToSave.Add((newProduct, row));
                        importedProducts.Add(row);
                    }
                }

                if (newProductsToSave.Any())
                {
                    await productRepo.AddRangeAsync(newProductsToSave.Select(x => x.Entity));
                }

                await _unitOfWork.CompleteAsync();

                // EF fills generated Ids after SaveChanges, so copy them back to the returned DTO rows.
                foreach (var saved in newProductsToSave)
                {
                    saved.SourceRow.Id = saved.Entity.Id;
                }

                return new ImportToReturnDTO<ProductImportToReturnDTO>
                {
                    Data = importedProducts,
                    TotalRows = excelRows.Count,
                    AddedCount = newProductsToSave.Count,
                    SkippedDuplicates = errors.Count,
                    Errors = errors
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to import products");
                throw;
            }
        }

        #region helper methods
        private static ImportExcelConfig<T> BuildImportConfig<T>(string sheetName)
        {
            return new ImportExcelConfig<T>
            {
                SheetName = sheetName,
                StartRow = 2,
                HasHeader = true
            };
        }

        // Create a new entity from an imported row.
        private static Product MapImportRowToNewProduct(ProductImportToReturnDTO row)
        {
            var product = new Product();
            MapImportRowToProduct(row, product);
            return product;
        }

        // Copy editable Excel values onto the product entity.
        private static void MapImportRowToProduct(ProductImportToReturnDTO row, Product product)
        {
            product.Name = row.Name.Trim();
            product.Descripaion = row.Descripaion;
            product.PictureUrl = row.PictureUrl;
            product.Price = row.Price;
            product.BrandId = row.BrandId > 0 ? row.BrandId : null;
            product.CategoryId = row.CategoryId > 0 ? row.CategoryId : null;
            product.Stock = row.Stock;
        }
        #endregion
        #endregion
    }
}

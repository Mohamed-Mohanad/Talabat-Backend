using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Talabat.API.Dtos;
using Talabat.API.Errors;
using Talabat.API.Helper;
using Talabat.BLL.Interfaces;
using Talabat.BLL.ProductSpecifications;
using Talabat.DAL.Entities;

namespace Talabat.API.Controllers
{
    [Authorize] 
    public class ProductsController : BaseApiController
    {
        private readonly IGenericRepository<Product> _productRepo;
        private readonly IGenericRepository<ProductType> _productTypeRepo;
        private readonly IGenericRepository<ProductBrand> _productBrandRepo;
        private readonly IMapper _mapper;

        public ProductsController(IGenericRepository<Product> productRepo,
            IGenericRepository<ProductType> productTypeRepo, IGenericRepository<ProductBrand> productBrandRepo, IMapper mapper)
        {
            _productRepo = productRepo;
            _productBrandRepo = productBrandRepo;
            _mapper = mapper;
            _productTypeRepo = productTypeRepo;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [CachedAttribute(600)]
        public async Task<ActionResult<Pageination<ProductDto>>> GetProducts([FromQuery] ProductSpecificationPrams productSpec)
        {
            var spec = new ProductFilterSpecification(productSpec);

            var totalItems = await _productRepo.GetCountAsync(spec);

            var products = await _productRepo.GetAllWithSpec(spec);

            var data = _mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductDto>>(products);
            
            return Ok(new Pageination<ProductDto>(productSpec.PageIndex, productSpec.PageSize, totalItems, data));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [CachedAttribute(600)]
        public async Task<ActionResult<ProductDto>> GetProduct(int id)
        {
            var spec = new ProductFilterSpecification(id);
            var product = await _productRepo.GetEntityWithSpec(spec);
            if (product is null)
                return NotFound(new ApiException(404));
            var productDto = _mapper.Map<Product, ProductDto>(product);
            return Ok(productDto);
        }

        [HttpGet("brands")]
        [CachedAttribute(600)]
        public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetBrands()
        {
            var brands = await _productBrandRepo.GetAllAsync();
            return Ok(brands);
        }
        [HttpGet("types")]
        [CachedAttribute(600)]
        public async Task<ActionResult<IReadOnlyList<ProductType>>> GetTypes()
        {
            var types = await _productTypeRepo.GetAllAsync();
            return Ok(types);
        }
    }
}

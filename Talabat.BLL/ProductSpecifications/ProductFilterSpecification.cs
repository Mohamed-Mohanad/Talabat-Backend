using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.BLL.Specifications;
using Talabat.DAL.Entities;

namespace Talabat.BLL.ProductSpecifications
{
    public class ProductFilterSpecification : BaseSpecifications<Product>
    {
        public ProductFilterSpecification(ProductSpecificationPrams productSpec) 
            : base(p =>
                (string.IsNullOrEmpty(productSpec.Search) || p.Name!.ToLower().Contains(productSpec.Search)) 
                  && (!productSpec.BrandId.HasValue || p.ProductBrandId == productSpec.BrandId.Value)
                  && (!productSpec.TypeId.HasValue || p.ProductTypeId == productSpec.TypeId.Value))
        {
            AddInclude(p => p.ProductType!);
            AddInclude(p => p.ProductBrand!);

            ApplyPaging(productSpec.PageSize * (productSpec.PageIndex - 1), productSpec.PageSize);

            if (!string.IsNullOrEmpty(productSpec.Sort))
            {
                switch (productSpec.Sort)
                {
                    case "priceAsc":
                        AddOrderBy(p => p.Price!);
                        break;
                    case "priceDesc":
                        AddOrderByDescending(p => p.Price);
                        break;
                    default:
                        AddOrderBy(p => p.Name!);
                        break;
                }
            }
            else
            {
                AddOrderBy(p => p.Name!);
            }
        }
        public ProductFilterSpecification(int id): base(p => p.Id == id)
        {
            AddInclude(p => p.ProductType!);
            AddInclude(p => p.ProductBrand!);
        }
    }
}

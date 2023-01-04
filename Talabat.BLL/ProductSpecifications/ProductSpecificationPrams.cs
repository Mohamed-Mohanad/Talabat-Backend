using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talabat.BLL.ProductSpecifications
{
    public class ProductSpecificationPrams
    {
        public int? BrandId { get; set; }
        public int? TypeId { get; set; }
        public string? Sort { get; set; }
        public int PageIndex { get; set; } = 1;

        private const int _maxPageSize = 50;
        private int _pageSize = 5;
        public int PageSize { 
            get { return _pageSize;  } 
            set 
            { 
                _pageSize = value > _maxPageSize ? _maxPageSize : value; 
            } 
        }

        private string? _search;
        public string? Search { 
            get { return _search; } 
            set { _search = value!.ToLower(); } }

    }
}

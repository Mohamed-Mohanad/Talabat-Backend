using Talabat.DAL.Entities;

namespace Talabat.API.Helper
{
    public class Pageination<T>
    {
        public Pageination(int pageIndex, int pageSize, int totalItems, IReadOnlyList<T> data)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
            Count = totalItems;
            Data = data;
        }

        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int Count { get; set; }
        public IReadOnlyList<T> Data { get; set; }

         

    }
}

namespace Cukcuk.Core.DTOs
{

        public class PageResult<T>
        {
            public IEnumerable<T> Items { get; set; } = new List<T>();
            public int TotalItems { get; set; }
            public int TotalPages { get; set; }
        }
  
}

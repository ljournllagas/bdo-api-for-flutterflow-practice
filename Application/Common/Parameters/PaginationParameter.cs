namespace Application.Common.Parameters
{
    public class PaginationParameter
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; } = 10;

        public PaginationParameter()
        {
            PageNumber = PageNumber < 1 ? 1 : PageNumber;

            if (PageSize < 10)
            {
                PageSize = 10;
            }
        }

        public PaginationParameter(int pageNumber, int pageSize)
        {
            //if page number is less than 1 then default it to 1
            PageNumber = pageNumber < 1 ? 1 : pageNumber;

            //if page size is less than 10 then default it to 10
            if (pageSize < 10)
            {
                PageSize = 10;
            }
            else
            {
                PageSize = pageSize;
            }
        }
    }
}

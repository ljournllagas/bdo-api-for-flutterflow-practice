using Application.Common.Parameters;
using System.Net;

namespace Application.Common.Wrappers
{
    public class PagedResponse<T> : Response<T>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalRows { get; set; }

        public PagedResponse(T data, PaginationParameter pagination, int totalRows)
        {
            PageNumber = pagination.PageNumber;
            PageSize = pagination.PageSize;
            TotalRows = totalRows;
            Data = data;
            Message = null;
            Succeeded = true;
            StatusCode = (int)HttpStatusCode.OK;
        }
    }
}

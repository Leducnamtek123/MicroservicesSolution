using Common.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Helpers
{
    public static class PaginationHelper
    {
        public static PagingDto<T> CreatePagedResponse<T>(List<T> items, int pageNumber, int pageSize, int totalRecords)
        {
            return new PagingDto<T>(items, totalRecords, pageNumber, pageSize);
        }
    }
}

namespace WataTek.Common
{
    public class FilterBase
    {
        // Từ khóa tìm kiếm
        public string? Keyword { get; set; } = null;

        // Tên cột để sắp xếp
        public string? SortBy { get; set; } = null;

        // Có phải sắp xếp giảm dần hay không
        public bool? IsSortDescending { get; set; } = null;

        // Chỉ số trang hiện tại (1-based)
        public int? PageIndex { get; set; } = null;

        // Kích thước trang (số lượng bản ghi trên mỗi trang)
        public int? PageSize { get; set; } = null;

        // Phương thức để tính toán số bản ghi bỏ qua (để hỗ trợ phân trang)
        public int GetSkipCount()
        {
            // Sử dụng giá trị mặc định nếu PageIndex hoặc PageSize là null
            int pageIndex = PageIndex ?? 1;
            int pageSize = PageSize ?? 10;
            return (pageIndex - 1) * pageSize;
        }

        // Tổng số trang (optional, có thể tính toán sau khi có tổng số bản ghi)
        public int TotalPages(int totalCount)
        {
            int pageSize = PageSize ?? 10; // Sử dụng giá trị mặc định nếu PageSize là null
            return (int)Math.Ceiling((double)totalCount / pageSize);
        }

        // Kiểm tra xem có yêu cầu sắp xếp không
        public bool HasSort()
        {
            return !string.IsNullOrWhiteSpace(SortBy);
        }

        // Kiểm tra xem có yêu cầu phân trang không
        public bool HasPaging()
        {
            return PageIndex.HasValue && PageSize.HasValue && PageIndex.Value > 0 && PageSize.Value > 0;
        }

        // Kiểm tra xem có yêu cầu tìm kiếm không
        public bool HasKeyword()
        {
            return !string.IsNullOrWhiteSpace(Keyword);
        }

        public bool? IsDeep { get; set; } = false;
    }
}

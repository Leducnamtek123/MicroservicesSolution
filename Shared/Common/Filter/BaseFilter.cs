namespace WataTek.Common
{
    public class FilterBase
    {
        // Từ khóa tìm kiếm
        public string? Keyword { get; set; } = null;

        // Tên cột để sắp xếp
        public string? SortBy { get; set; } = null;

        // Có phải sắp xếp giảm dần hay không
        public bool IsSortDescending { get; set; } = false;

        // Chỉ số trang hiện tại (1-based)
        public int PageIndex { get; set; } = 1;

        // Kích thước trang (số lượng bản ghi trên mỗi trang)
        public int PageSize { get; set; } = 10;

        // Phương thức để tính toán số bản ghi bỏ qua (để hỗ trợ phân trang)
        public int GetSkipCount()
        {
            return (PageIndex - 1) * PageSize;
        }

        // Tổng số trang (optional, có thể tính toán sau khi có tổng số bản ghi)
        public int TotalPages(int totalCount)
        {
            return (int)Math.Ceiling((double)totalCount / PageSize);
        }

        // Kiểm tra xem có yêu cầu sắp xếp không
        public bool HasSort()
        {
            return !string.IsNullOrWhiteSpace(SortBy);
        }

        // Kiểm tra xem có yêu cầu phân trang không
        public bool HasPaging()
        {
            return PageIndex > 0 && PageSize > 0;
        }

        // Kiểm tra xem có yêu cầu tìm kiếm không
        public bool HasKeyword()
        {
            return !string.IsNullOrWhiteSpace(Keyword);
        }

        // Gợi ý: Có thể thêm các phương thức để xử lý logic tìm kiếm, sắp xếp và phân trang tùy thuộc vào yêu cầu cụ thể.
    }
}

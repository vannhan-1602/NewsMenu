

namespace Application.Common
{
    public class BaseResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int? Id { get; set; }

       
        public List<int> InvalidIds { get; set; } = new();
    }
}
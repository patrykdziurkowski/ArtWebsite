namespace web.Models;

public class ErrorViewModel
{
        public int? StatusCode { get; }
        public string? Reason { get; }
        public bool HasReason => StatusCode.HasValue;

        public ErrorViewModel()
        {
        }

        public ErrorViewModel(int statusCode, string reason)
        {
                StatusCode = statusCode;
                Reason = reason;
        }
}

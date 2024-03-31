namespace BlazorStack.Portal.Models
{
    public enum NotificationType
    {
        Success,
        Warning,
        Error
    }

    public class NotificationMessage
    {
        public string Message { get; set; }
        public NotificationType Type { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }

}

namespace ChatApi.Domain.Notifications
{
    public interface INotificationContext
    {
        public void AddNotification(int key, string message);
    }
}

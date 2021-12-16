namespace ChatApi.Domain.Notifications
{
    public class Notification
    {
		public int Key { get; }
		public string Message { get; }

		public Notification(int key, string message)
		{
			Key = key;
			Message = message;
		}
	}
}

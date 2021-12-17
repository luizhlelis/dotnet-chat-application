
using System.Collections.Generic;
using System.Linq;

namespace ChatApi.Domain.Notifications
{
    public class NotificationContext : INotificationContext
    {
		private readonly List<Notification> _notifications;
		public IReadOnlyCollection<Notification> Notifications => _notifications;
		public bool HasNotifications => _notifications.Any();

		public NotificationContext()
		{
			_notifications = new List<Notification>();
		}

		public void AddNotification(int key, string message)
		{
			_notifications.Add(new Notification(key, message));
		}
	}
}

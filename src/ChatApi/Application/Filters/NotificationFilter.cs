﻿
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ChatApi.Application.Filters
{
	//public class NotificationFilter : IAsyncResultFilter
	//{
	//	private readonly NotificationContext _notificationContext;

	//	public NotificationFilter(NotificationContext notificationContext)
	//	{
	//		_notificationContext = notificationContext;
	//	}

	//	public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
	//	{
	//		if (_notificationContext.HasNotifications)
	//		{
	//			context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
	//			context.HttpContext.Response.ContentType = "application/json";

	//			var notifications = JsonConvert.SerializeObject(_notificationContext.Notifications);
	//			await context.HttpContext.Response.WriteAsync(notifications);

	//			return;
	//		}

	//		await next();
	//	}
	//}
}

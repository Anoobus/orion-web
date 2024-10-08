﻿using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Orion.Web.Notifications
{
    public static class UserNameExtenions
    {
        public static string SafeUserName(this ClaimsPrincipal user)
        {
            return user?.Identity?.Name ?? "NOT AUTHED";
        }
    }

    [Route("api/Notifications")]
    [Authorize]
    public class NotificationsController : Controller
    {
        public static readonly ConcurrentDictionary<string, Queue<string>> Notifications = new ConcurrentDictionary<string, Queue<string>>();

        public static void AddNotification(string userName, string msg)
        {
            Notifications.AddOrUpdate(
                userName,
                new Queue<string>(new string[] { msg }),
                (username, queue) =>
                 {
                     queue.Enqueue(msg);
                     return queue;
                 });
        }

        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            var lastOne = User.SafeUserName();
            var msgs = Notifications.GetOrAdd(lastOne, new Queue<string>());
            var allNotifications = new List<string>();
            while (msgs.TryDequeue(out var stringy))
            {
                allNotifications.Add(stringy);
            }

            return allNotifications;
        }
    }
}

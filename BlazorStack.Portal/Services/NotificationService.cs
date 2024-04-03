using System;
using System.Collections.Generic;
using System.Linq;
using BlazorStack.Portal.Models;
using Microsoft.AspNetCore.Components;

namespace BlazorStack.Portal.Services
{
    public class NotificationService
    {
        private readonly List<NotificationMessage> _messages = new();

        public IReadOnlyList<NotificationMessage> Messages => _messages.AsReadOnly();

        public event Action OnChange;

        public async Task ShowNotification(string message, NotificationType type)
        {
            var notificationMessage = new NotificationMessage { Message = message, Type = type };
            _messages.Insert(0, notificationMessage);

            if (_messages.Count > 5)
            {
                _messages.RemoveAt(5);
            }

            NotifyStateChanged();

            await Task.Delay(10000); // Wait for 10 seconds
            _messages.Remove(notificationMessage);
            NotifyStateChanged();
        }

        public async Task ShowError(string message) => await ShowNotification(message, NotificationType.Error);


        public async Task ShowErrorNotifications(List<string> errors)
        {
            foreach (var error in errors)
            {
                await ShowNotification(error, NotificationType.Error);
            }
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }

}

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
        private readonly Dictionary<NotificationMessage, Timer> _messageTimers = new();

        public IReadOnlyList<NotificationMessage> Messages => _messages.AsReadOnly();

        public event Action OnChange;

        public void ShowNotification(string message, NotificationType type)
        {
            var notificationMessage = new NotificationMessage { Message = message, Type = type };
            _messages.Insert(0, notificationMessage);

            if (_messages.Count > 5)
            {
                var messageToRemove = _messages[5];
                _messages.RemoveAt(5);
                RemoveMessageTimer(messageToRemove);
            }

            var timer = new Timer(RemoveMessageCallback, notificationMessage, TimeSpan.FromSeconds(10), Timeout.InfiniteTimeSpan);

            NotifyStateChanged();
        }

        public void ShowError(string message) => ShowNotification(message, NotificationType.Error);
        public void ShowSuccess(string message) => ShowNotification(message, NotificationType.Success);


        public void ShowErrors(List<string> errors)
        {
            foreach (var error in errors)
            {
                ShowNotification(error, NotificationType.Error);
            }
        }

        private void RemoveMessageTimer(NotificationMessage message)
        {
            if (_messageTimers.TryGetValue(message, out var timer))
            {
                _messageTimers.Remove(message);
                timer.Dispose();
            }
        }

        private void RemoveMessageCallback(object? state)
        {
            if (state is null) return;
            var message = (NotificationMessage)state;
            _messages.Remove(message);
            RemoveMessageTimer(message);
            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}

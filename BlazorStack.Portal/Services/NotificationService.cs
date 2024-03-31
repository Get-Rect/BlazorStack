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

        public void ShowNotification(string message, NotificationType type)
        {
            _messages.Insert(0, new NotificationMessage { Message = message, Type = type });

            // Keep only the 5 most recent messages
            if (_messages.Count > 5)
            {
                _messages.RemoveAt(5);
            }

            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }

}

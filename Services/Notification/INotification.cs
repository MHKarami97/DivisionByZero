using Microsoft.AspNetCore.Http;
using System;

namespace Services.Notification
{
    public interface INotification
    {
        int Send(string message);
    }
}
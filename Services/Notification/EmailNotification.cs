using System;

namespace Services.Notification
{
    public class EmailNotification : INotification
    {
        private readonly string _address;

        public EmailNotification(string address)
        {
            _address = address;
        }

        public int Send(string message)
        {
            throw new NotImplementedException();
        }
    }
}
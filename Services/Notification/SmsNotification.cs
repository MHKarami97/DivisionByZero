using System;

namespace Services.Notification
{
    public class SmsNotification : INotification
    {
        private readonly string _phoneNumber;

        public SmsNotification(string phone)
        {
            _phoneNumber = phone;
        }

        public int Send(string message)
        {
            throw new NotImplementedException();
        }
    }
}
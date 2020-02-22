namespace Services.Notification
{
    public interface INotification
    {
        int Send(string message);
    }
}
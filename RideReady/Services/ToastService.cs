using Radzen;

namespace RideReady.Services;

public class ToastService : IToastService
{
    private readonly NotificationService _notifications;

    public ToastService(NotificationService notifications)
    {
        _notifications = notifications;
    }

    public void ShowSuccess(string message) =>
        _notifications.Notify(new NotificationMessage
        {
            Severity = NotificationSeverity.Success,
            Summary = "Sucesso",
            Detail = message,
            Duration = 3000
        });

    public void ShowError(string message) =>
        _notifications.Notify(new NotificationMessage
        {
            Severity = NotificationSeverity.Error,
            Summary = "Erro",
            Detail = message,
            Duration = 6000
        });

    public void ShowWarning(string message) =>
        _notifications.Notify(new NotificationMessage
        {
            Severity = NotificationSeverity.Warning,
            Summary = "Aviso",
            Detail = message,
            Duration = 4000
        });

    public void ShowInfo(string message) =>
        _notifications.Notify(new NotificationMessage
        {
            Severity = NotificationSeverity.Info,
            Summary = "Informação",
            Detail = message,
            Duration = 3000
        });
}
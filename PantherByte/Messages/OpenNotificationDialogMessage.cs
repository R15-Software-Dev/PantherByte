using CommunityToolkit.Mvvm.Messaging.Messages;
using PantherByte.ViewModels;

namespace PantherByte.Messages;

public class OpenNotificationDialogMessage(string title, string message)
    : AsyncRequestMessage<NotificationDialogViewModel?> {
    public readonly string Title = title;
    public readonly string Message = message;
}
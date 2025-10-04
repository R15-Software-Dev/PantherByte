namespace PantherByte.ViewModels;

public class NotificationDialogViewModel : ViewModelBase {
    public string Title { get; set; } = "Notification";
    public string Message { get; set; } = "Testing message";

    public NotificationDialogViewModel() { }

    public NotificationDialogViewModel(string title, string message) {
        (Title, Message) = (title, message);
    }
}
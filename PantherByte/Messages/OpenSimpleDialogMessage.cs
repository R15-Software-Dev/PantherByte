using CommunityToolkit.Mvvm.Messaging.Messages;
using PantherByte.ViewModels;

namespace PantherByte.Messages;

public class OpenSimpleDialogMessage(SimpleDialogViewModel vm) : AsyncRequestMessage<SimpleDialogViewModel.SimpleDialogResult?> {
    public SimpleDialogViewModel ViewModel = vm;
}
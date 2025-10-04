using System.Diagnostics;
using CommunityToolkit.Mvvm.Messaging.Messages;
using PantherByte.ViewModels;

namespace PantherByte.Messages;

public class OpenProgressDialogMessage(ProgressWindowViewModel vm) : AsyncRequestMessage<ProgressWindowViewModel?> {
    public readonly ProgressWindowViewModel ViewModel = vm;
};
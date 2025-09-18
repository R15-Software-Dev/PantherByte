using System.Diagnostics;
using CommunityToolkit.Mvvm.Messaging.Messages;
using PantherByte.ViewModels;

namespace PantherByte.Messages;

public class OpenProgressDialogMessage(ProcessStartInfo info) : AsyncRequestMessage<ProgressWindowViewModel?> {
    public readonly ProcessStartInfo Info = info;
};
using Avalonia.Controls;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Messaging;
using PantherByte.Messages;
using PantherByte.ViewModels;

namespace PantherByte.Views;

public partial class MainWindow : Window {
    public MainWindow() {
        InitializeComponent();

        WeakReferenceMessenger.Default.Register<MainWindow, OpenProgressDialogMessage>(this,
            static (w, m) => {
                var dialog = new ProgressWindow {
                    DataContext = m.ViewModel
                };
                m.Reply(dialog.ShowDialog<ProgressWindowViewModel?>(w));
            });

        WeakReferenceMessenger.Default.Register<MainWindow, OpenSimpleDialogMessage>(this,
            static async (w, m) => {
                var dialog = new SimpleDialog {
                    DataContext = m.ViewModel
                };
                var response = dialog.ShowDialog<SimpleDialogViewModel.SimpleDialogResult?>(w);
                m.Reply(response);
            });

        WeakReferenceMessenger.Default.Register<MainWindow, OpenNotificationDialogMessage>(this,
            static (w, m) => {
                var dialog = new NotificationDialog {
                    DataContext = new NotificationDialogViewModel(m.Title, m.Message)
                };
                m.Reply(dialog.ShowDialog<NotificationDialogViewModel?>(w));
            });
    }
}
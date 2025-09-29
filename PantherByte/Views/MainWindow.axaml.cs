using Avalonia.Controls;
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
                    DataContext = new ProgressWindowViewModel(m.Info)
                };
                m.Reply(dialog.ShowDialog<ProgressWindowViewModel?>(w));
            });

        WeakReferenceMessenger.Default.Register<MainWindow, OpenSimpleDialogMessage>(this,
            static (w, m) => {
                var dialog = new SimpleDialog {
                    DataContext = m.ViewModel
                };
                m.Reply(dialog.ShowDialog<SimpleDialogViewModel.SimpleDialogResult?>(w));
            });
    }
}

using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace PantherByte.Views;

public partial class NotificationDialog : Window {
    public NotificationDialog() {
        InitializeComponent();
    }

    private void OkButton_Click(object? sender, RoutedEventArgs e) =>
        Close(DataContext);
}
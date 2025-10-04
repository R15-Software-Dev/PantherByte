using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using PantherByte.ViewModels;

namespace PantherByte.Views;

public partial class SimpleDialog : Window {
    public SimpleDialog() {
        InitializeComponent();
    }

    public void YesButtonClicked(object sender, RoutedEventArgs e) =>
        Close(SimpleDialogViewModel.SimpleDialogResult.Yes);

    public void NoButtonClicked(object sender, RoutedEventArgs e) =>
        Close(SimpleDialogViewModel.SimpleDialogResult.No);
}
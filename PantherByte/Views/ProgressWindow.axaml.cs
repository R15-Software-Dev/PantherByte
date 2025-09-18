using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using PantherByte.ViewModels;

namespace PantherByte.Views;

public partial class ProgressWindow : Window {
    public ProgressWindow() {
        InitializeComponent();
        
        // This feels a little fragile, but we'll leave it for now.
        Opened += async (_, _) => {
            if (DataContext is not ProgressWindowViewModel vm)
                return;
            
            await vm.RunProcessAsync();
        };
    }
}

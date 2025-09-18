using Avalonia.Controls;
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

        StdOutText.SizeChanged += (_, _) => ScrollView.ScrollToEnd();
    }
}

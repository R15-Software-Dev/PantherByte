using Avalonia;
using Avalonia.Controls;
using PantherByte.ViewModels;

namespace PantherByte.Views;

public partial class ProgressWindow : Window {
    public ProgressWindow() {
        InitializeComponent();

        // Runs the specified process in the view model when the window is opened.
        Opened += async (_, _) => {
            if (DataContext is IProgressWindowViewModel vm)
                await vm.RunProcessAsync();
        };

        // Automatically scrolls to the end of the logs when more information is added.
        StdOutText.SizeChanged += (_, _) => ScrollView.ScrollToEnd();

        // Automatically resizes the window when the user collapses the expander.
        InfoExpander.Collapsing += (_, _) => {
            if (Content is not Control content) return;

            content.Measure(Size.Infinity);
            var desired = content.DesiredSize;
            Width = desired.Width;
            Height = desired.Height;
        };
    }
}
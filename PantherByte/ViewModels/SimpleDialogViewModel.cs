using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;

namespace PantherByte.ViewModels;

public partial class SimpleDialogViewModel(string prompt, string title) : ViewModelBase {
    public enum SimpleDialogResult {
        Yes,
        No
    }
    
    public string PromptMessage { get; set; } = prompt;
    public string Title { get; set; } = title;
}
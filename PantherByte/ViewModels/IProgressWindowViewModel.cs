using System.ComponentModel;
using System.Threading.Tasks;

namespace PantherByte.ViewModels;

/// <summary>
/// Defines a series of properties and methods that are used by the <see cref="Views.ProgressWindow"/>.
/// It requires the implementation of <see cref="INotifyPropertyChanged"/> as well, so that it can
/// preserve the reactivity of the interface.
/// </summary>
public interface IProgressWindowViewModel : INotifyPropertyChanged {
    public string ProcessStdOut { get; set; }
    public string StatusMessage { get; set; }
    public bool Error { get; set; }
    public bool InProgress { get; set; }
    public string BarColor { get; set; }
    public Task RunProcessAsync();
}

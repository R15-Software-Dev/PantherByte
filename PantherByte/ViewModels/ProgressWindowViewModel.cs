using System;
using System.Diagnostics;
using System.Threading.Tasks;
using ReactiveUI;

namespace PantherByte.ViewModels;

public partial class ProgressWindowViewModel : ViewModelBase, IProgressWindowViewModel {
    // Pass variables between the MainWindow and this window, specifically what point the application
    // is at in terms of downloading. We'll likely do this as a simple percentage of total progress.
    private readonly ProcessStartInfo _processStartInfo;
    private string _processStdOut = "";
    private bool _error = false;
    private bool _inProgress = false;
    private string _statusMessage = "Downloading video...";
    private string _barColor = "CornflowerBlue";

    public string ProcessStdOut {
        get => _processStdOut;
        set => this.RaiseAndSetIfChanged(ref _processStdOut, value);
    }

    public string StatusMessage {
        get => _statusMessage;
        set => this.RaiseAndSetIfChanged(ref _statusMessage, value);
    }

    public bool Error {
        get => _error;
        set => this.RaiseAndSetIfChanged(ref _error, value);
    }

    public bool InProgress {
        get => _inProgress;
        set => this.RaiseAndSetIfChanged(ref _inProgress, value);
    }

    public string BarColor {
        get => _barColor;
        set => this.RaiseAndSetIfChanged(ref _barColor, value);
    }

    public ProgressWindowViewModel(ProcessStartInfo info) {
        _processStartInfo = info;
        Console.WriteLine("Created ProgressWindowViewModel instance");
    }

    public async Task RunProcessAsync() {
        Process downloadProcess = new();
        downloadProcess.StartInfo = _processStartInfo;

        downloadProcess.EnableRaisingEvents = true;
        downloadProcess.OutputDataReceived += (_, e) => {
            if (string.IsNullOrWhiteSpace(e.Data)) return;

            Console.WriteLine(e.Data);
            ProcessStdOut += e.Data + "\n";
        };

        downloadProcess.ErrorDataReceived += (_, e) => {
            if (string.IsNullOrWhiteSpace(e.Data)) return;
            Console.WriteLine(e.Data);
            ProcessStdOut += e.Data + "\n";
            Error = true;
            StatusMessage = "Errors while downloading video.";
            BarColor = "Red";
        };

        Console.WriteLine("Download process is started.");
        InProgress = true;
        try {
            downloadProcess.Start();
            downloadProcess.BeginOutputReadLine();
            downloadProcess.BeginErrorReadLine();
            await downloadProcess.WaitForExitAsync();
        }
        catch (Exception e) {
            Console.WriteLine(e.StackTrace);
            ProcessStdOut += "Error: " + e.StackTrace + "\n";
            StatusMessage = "Error while performing download.";
            BarColor = "Red";
            Error = true;
        }
        finally {
            InProgress = false;

            if (!Error) {
                BarColor = "Green";
                StatusMessage = "Download finished!";
            }
        }
    }
}
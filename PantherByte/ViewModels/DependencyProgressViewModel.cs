using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using ReactiveUI;

namespace PantherByte.ViewModels;

public partial class DependencyProgressViewModel : ViewModelBase, IProgressWindowViewModel {
    private readonly ProcessStartInfo _processStartInfo;
    private string _processStdOut = "";
    private bool _error = false;
    private bool _inProgress = false;
    private string _statusMessage = "Downloading ffmpeg...";
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

    public DependencyProgressViewModel() {
        _processStartInfo = new ProcessStartInfo() {
            UseShellExecute = false
        };
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
            // Windows specific command.
            _processStartInfo.FileName = "winget";
            _processStartInfo.Arguments = "install ffmpeg";
            _processStartInfo.Verb = "runas";
        } else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
            // Linux specific command.
            _processStartInfo.FileName = "sudo";
            _processStartInfo.Arguments = "apt install ffmpeg";
        }
    }
    
    [RelayCommand]
    public async Task RunProcessAsync() {
        Process installProcess = new();
        installProcess.StartInfo = _processStartInfo;
        
        installProcess.EnableRaisingEvents = true;
        installProcess.OutputDataReceived += (s, e) => {
            if (string.IsNullOrWhiteSpace(e.Data)) return;

            Console.WriteLine(e.Data);
            ProcessStdOut += e.Data;
        };

        installProcess.ErrorDataReceived += (s, e) => {
            if (string.IsNullOrWhiteSpace(e.Data)) return;
            Console.WriteLine(e.Data);
            ProcessStdOut += e.Data + "\n";
            Error = true;
            StatusMessage = "Error occurred while installing ffmpeg.";
            BarColor = "Red";
        };

        InProgress = true;
        try {
            installProcess.Start();
            Console.WriteLine("Installation process started.");
            installProcess.BeginOutputReadLine();
            installProcess.BeginErrorReadLine();
            await installProcess.WaitForExitAsync();
        }
        catch (Exception ex) {
            Console.WriteLine(ex.Message);
            Error = true;
            StatusMessage = "Error occurred while installing ffmpeg.";
            BarColor = "Red";
            ProcessStdOut += ex.Message;
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
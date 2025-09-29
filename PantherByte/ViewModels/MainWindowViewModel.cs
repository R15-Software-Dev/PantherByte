using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.Messaging;
using DynamicData;
using PantherByte.Messages;
using PantherByte.Services;
using ReactiveUI;
using PantherByte.Services.Interfaces;
using PantherByte.Views;

namespace PantherByte.ViewModels;

public partial class MainWindowViewModel : ViewModelBase {
    private string _link = string.Empty;
    private string _saveLocation = string.Empty;
    private string _format = "mp4";
    private ObservableCollection<string> _availableFormats = ["mp4", "mp3", "wav", "aac", "mkv"];
    private bool _canDownload = true;

    /// <summary>
    /// The link to the video to download. For now, only use YouTube links.
    /// </summary>
    public string Link { 
        get => _link;
        set => this.RaiseAndSetIfChanged(ref _link, value);
    }

    /// <summary>
    /// The format to download the video in. Currently supported options are MP3, MP4, and WAV. This will become the
    /// argument passed to the "-t" argument in <see cref="GenerateCmdArgsAsync"/>.
    /// </summary>
    public string Format {
        get => _format;
        set => this.RaiseAndSetIfChanged(ref _format, value);
    }

    public ObservableCollection<string> AvailableFormats {
        get => _availableFormats;
        set => this.RaiseAndSetIfChanged(ref _availableFormats, value);
    }
    
    /// <summary>
    /// The folder to save the files in. This will become the argument passed to the "-P" argument in
    /// <see cref="GenerateCmdArgsAsync"/>.
    /// </summary>
    public string SaveLocation {
        get => _saveLocation;
        set => this.RaiseAndSetIfChanged(ref _saveLocation, value);
    }
    
    /// <summary>
    /// Indicates if the file can be downloaded. If not, the download button is disabled.
    /// </summary>
    public bool CanDownload {
        get => _canDownload;
        set => this.RaiseAndSetIfChanged(ref _canDownload, value);
    }
    
    /// <summary>
    /// Constructor. Mainly used for subscribing properties to the changes from other properties.
    /// </summary>
    public MainWindowViewModel() {
        // Subscribe the CanDownload bool to the results of the Link, SaveLocation, and Format property changes.
        this.WhenAnyValue(
                x => x.Link,
                x => x.SaveLocation,
                x => x.Format,
                (link, saveLocation, format) => !string.IsNullOrWhiteSpace(link)
                                                && !string.IsNullOrWhiteSpace(format)
                                                && !string.IsNullOrWhiteSpace(saveLocation)
            )
            .Subscribe(value => CanDownload = value);
    }
    
    /// <summary>
    /// Gets the save location for the generated file. We are only allowing users to download one file at a time for now.
    /// </summary>
    /// <exception cref="NullReferenceException">
    /// Thrown when there are no available services that implement <see cref="IFileSaveService"/>, or when the
    /// <see cref="Avalonia.Platform.Storage.IStorageFolder"/> returned from the <see cref="IFileSaveService.ChooseFolderAsync"/>
    /// method is null.
    /// </exception>
    /// <exception cref="DirectoryNotFoundException">
    /// Thrown when the selected directory path does not exist in the current filesystem.
    /// </exception>
    [RelayCommand]
    private async Task GetSaveLocationAsync() {
        try {
            var fileService = App.Current?.Services?.GetService(typeof(IFileSaveService));
            if (fileService is FileService service) {
                var filePath = await service.ChooseFolderAsync();
                if (filePath is not null) {
                    var tempPath = filePath.Path.AbsolutePath;
                    if (Directory.Exists(tempPath)) {
                        SaveLocation = tempPath;
                        Console.WriteLine("Successfully set the file path.");
                    } else {
                        throw new DirectoryNotFoundException($"Directory does not exist: {tempPath}");
                    }
                } else {
                    throw new NullReferenceException("The resulting storage folder was null.");
                }
            } else {
                throw new NullReferenceException("Missing a FileSaveService instance");
            }
        } catch (Exception ex) {
            // Exceptions are caught locally for now, but will eventually require a modal error window.
            Console.WriteLine(ex.ToString());
        }
    }

    /// <summary>
    /// Generates the arguments used for the main command string.
    /// </summary>
    [RelayCommand]
    private async Task<List<string>> GenerateCmdArgsAsync() {
        // The command string format to use: yt-dlp <link> -t <mp3|mp4|wav> -P <path> --no-playlist
        // yt-dlp requires usage of ffmpeg as well.
        // Code will be executed when the button is clicked.

        return await Task.Run(() => {
            var fileName = "";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                Console.WriteLine("Using Windows program.");
                fileName = "Programs/yt-dlp-win64.exe";
            } else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
                Console.WriteLine("Using Linux program. May require additional run permissions.");
                fileName = "Programs/yt-dlp-linux";
            }

            List<string> args = [fileName, Link, "-P", SaveLocation, "--no-playlist"];

            // WAV files require special command arguments
            if (Format.Equals("wav")) {
                args.AddRange(["--extract-audio", "--audio-format", Format]);
            } else {
                args.AddRange(["-t", Format]);
            }
            
            return args;
        });
    }

    private async Task<bool> CheckFfmpegInstalled() {
        return await Task.Run(async () => {
            ProcessStartInfo ffmpegInfo = new("ffmpeg") {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            };

            Process process = new() {
                StartInfo = ffmpegInfo
            };

            bool result = true;

            try {
                process.Start();
                await process.WaitForExitAsync();
            }
            catch (Exception ex) {
                result = false;
            }

            return result;
        });
    }

    /// <summary>
    /// Handles the download process of the file. Generates the command and runs it.
    /// </summary>
    [RelayCommand]
    private async Task DownloadFileAsync() {
        // Build the command and run the yt-dlp program by using the generated string.
        // Somehow we'll need to package the program in with the application during distribution.
        var args = await GenerateCmdArgsAsync();
        // Run the command.
        try {
            // The command that's run must work on at least Linux and Windows
            ProcessStartInfo info = new() {
                FileName = args[0],
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            info.ArgumentList.AddRange(args[1..]);
            
            // Check that ffmpeg is installed.
            var temp = await CheckFfmpegInstalled();
            // if (!await CheckFfmpegInstalled()) {
            SimpleDialogViewModel vm = new("Dependency ffmpeg was not found. Would you like to install it?",
                "Dependency Install");
                var result = await WeakReferenceMessenger.Default.Send(new OpenSimpleDialogMessage(vm));
                if (result is not null) {
                    Console.WriteLine($"Received result: {result}");
                }
            // }
            
            // This window isn't linked to the download process yet, but that is the plan.
            await OpenProgressWindowAsync(info);
        } catch (Exception ex) {
            Console.WriteLine(ex.ToString());
        }
    }

    /// <summary>
    /// Opens a ProgressWindow as a modal dialog. I haven't decided if this window should run the command or if it
    /// should simply display the progress of a command that's running here.
    /// </summary>
    [RelayCommand]
    private async Task OpenProgressWindowAsync(ProcessStartInfo info) {
        await WeakReferenceMessenger.Default.Send(new OpenProgressDialogMessage(info));
    }
}

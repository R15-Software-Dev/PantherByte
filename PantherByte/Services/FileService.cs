using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using PantherByte.Services.Interfaces;

namespace PantherByte.Services;

public class FileService : IFileSaveService {
    private readonly Window _target;

    public FileService(Window target) {
        _target = target;
    }
    
    /// <summary>
    /// Opens a file picker for the user and returns the path of the file they'd like to create.
    /// </summary>
    /// <returns>A filepath to the user's selected save location.</returns>
    public async Task<IStorageFolder?> ChooseFolderAsync() {
        var folderList = await _target.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions() {
            Title = "Choose Download Location",
        });

        return folderList[0];
    }
}

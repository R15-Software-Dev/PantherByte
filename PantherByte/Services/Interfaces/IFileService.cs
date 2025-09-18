using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;

namespace PantherByte.Services.Interfaces;

public interface IFileSaveService {
    public Task<IStorageFolder?> ChooseFolderAsync();
}

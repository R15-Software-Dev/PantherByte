using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;

namespace PantherByte;

public interface IFileSaveService {
    public Task<IStorageFolder?> ChooseFolderAsync();
}

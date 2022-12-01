using Domain.Entities.CaffFileAggregate;
using System.Runtime.InteropServices;

namespace Application.Services;

public class FileService : IFileService
{
    [DllImport(@"../../../Parser/x64/Debug/Parser.dll")]
    private static extern ulong GeneratePreviewFromCaff(byte[] inBuffer, ulong inLength, byte[] outBuffer, ulong outLength);

    public async Task<CaffFile> UploadFile(string originalFileName, byte[] caffFile) 
    {
        var file = new CaffFile
        {
            Id = Guid.NewGuid(),
            OriginalFileName = originalFileName,
        };

        byte[] outBuffer = new byte[caffFile.Length];
        // parse caff file
        GeneratePreviewFromCaff(caffFile, (ulong)caffFile.Length + 1, outBuffer, (ulong)outBuffer.Length + 1);

        // Save preview
        await SaveFile(file.Id, "../../Api/wwwroot/previews", "bmp", outBuffer);
        // Save caff file
        await SaveFile(file.Id, "../../Api/CaffFiles", "caff", caffFile);

        return file;
    }

    private async Task SaveFile(Guid id, string path, string fileExtension, byte[] content)
    {
        var fileName = $"{id}.{fileExtension}";
        var filePath = $"{path}/{fileName}";
        await File.WriteAllBytesAsync(filePath, content);
    }

    private async Task<byte[]> LoadFile(Guid id, string path, string fileExtension)
    {
        var fileName = $"{id}.{fileExtension}";
        var filePath = $"{path}/{fileName}";
        return await File.ReadAllBytesAsync(filePath);
    }


    public async Task<byte[]> LoadPreview(Guid id)
    {
        return await LoadFile(id, "../../Api/wwwroot/previews", "bmp");
    }

    public async Task<byte[]> LoadCaffFile(Guid id)
    {
        return await LoadFile(id, "../../Api/CaffFiles", "caff");
    }
}

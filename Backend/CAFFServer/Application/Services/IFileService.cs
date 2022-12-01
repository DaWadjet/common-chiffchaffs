using Domain.Entities.CaffFileAggregate;

namespace Application.Services;

public interface IFileService
{
    public Task<CaffFile> UploadFile(string originalFileName, byte[] file);
    Task<byte[]> LoadPreview(Guid id);
    Task<byte[]> LoadCaffFile(Guid id);
}

using Domain.Entities.CaffFileAggregate;

namespace Application.Services
{
    public interface IFileService
    {
        public Task<CaffFile> UploadFileAsync(string originalFileName, byte[] caffFile);
        Task<byte[]> LoadPreviewAsync(Guid id);
        Task<byte[]> LoadCaffFileAsync(Guid id);
        void DeleteFiles(Guid id);
    }
}
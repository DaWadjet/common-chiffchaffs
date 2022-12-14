using Application.Interfaces;
using CSONGE.Application.Exceptions;
using Domain.Entities.CaffFileAggregate;
using Microsoft.Extensions.Logging;
using System.Runtime.InteropServices;

namespace Application.Services
{

    public class FileService : IFileService
    {
        private readonly ILogger<FileService> logger;
        private readonly IIdentityService identityService;

        [DllImport(@"../../../../../../Parser/x64/Debug/Parser.dll")]
        private static extern ulong GeneratePreviewFromCaff(byte[] inBuffer, ulong inLength, byte[] outBuffer, ulong outLength);

        public FileService(ILogger<FileService> logger, IIdentityService identityService)
        {
            this.logger = logger;
            this.identityService = identityService;
        }

        public async Task<CaffFile> UploadFileAsync(string originalFileName, byte[] caffFile)
        {
            logger.LogInformation(message: $"File feltöltés kezdete: Felhasználó: {identityService.GetCurrentUserId()}, File: {originalFileName}");

            var file = new CaffFile
            {
                Id = Guid.NewGuid(),
                OriginalFileName = originalFileName,
                UploaderId = identityService.GetCurrentUserId()
            };

            byte[] outBuffer = new byte[caffFile.Length];
            // parse caff file
            try
            {
                GeneratePreviewFromCaff(caffFile, (ulong)caffFile.Length + 1, outBuffer, (ulong)outBuffer.Length + 1);
            }
            catch (Exception)
            {
                throw new CSONGE.Application.Exceptions.ApplicationException("A-a.");
            }

            if (!Directory.Exists("../Api/wwwroot/previews"))
            {
                Directory.CreateDirectory("../Api/wwwroot/previews");
            }

            if (!Directory.Exists("../Api/CaffFiles"))
            {
                Directory.CreateDirectory("../Api/CaffFiles");
            }

            // Save preview
            await SaveFile(file.Id, "../Api/wwwroot/previews", "bmp", outBuffer);
            // Save caff file
            await SaveFile(file.Id, "../Api/CaffFiles", "caff", caffFile);

            logger.LogInformation(message: $"File feltöltés vége: Felhasználó: {identityService.GetCurrentUserId()}, File: {originalFileName}");
            return file;
        }

        private static async Task SaveFile(Guid id, string path, string fileExtension, byte[] content)
        {
            var fileName = $"{id}.{fileExtension}";
            var filePath = $"{path}/{fileName}";
            await File.WriteAllBytesAsync(filePath, content);
        }

        private static async Task<byte[]> LoadFile(Guid id, string path, string fileExtension)
        {
            var fileName = $"{id}.{fileExtension}";
            var filePath = $"{path}/{fileName}";
            return await File.ReadAllBytesAsync(filePath);
        }


        public async Task<byte[]> LoadPreviewAsync(Guid id)
        {
            logger.LogInformation(message: $"File letöltése: Felhasználó: {identityService.GetCurrentUserId()}, File: {id}");
            return await LoadFile(id, "../Api/wwwroot/previews", "bmp");
        }

        public async Task<byte[]> LoadCaffFileAsync(Guid id)
        {
            logger.LogInformation(message: $"File letöltése: Felhasználó: {identityService.GetCurrentUserId()}, File: {id}");
            return await LoadFile(id, "../Api/CaffFiles", "caff");
        }

        public void DeleteFiles(Guid id)
        {
            logger.LogInformation(message: $"File törlése: Felhasználó: {identityService.GetCurrentUserId()}, File: {id}");
            File.Delete($"../Api/wwwroot/previews/{id}.caff");
            File.Delete($"../Api/CaffFiles/{id}.caff");
        }
    }
}
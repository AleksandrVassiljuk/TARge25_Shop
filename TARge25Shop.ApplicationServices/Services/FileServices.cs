
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TARge25_Shop.Core.Domain;
using TARge25_Shop.Core.Dto;
using TARge25_Shop.Core.ServiceInterface;
using TARge25_Shop.Data;

namespace TARge25_Shop.ApplicationServices.Services
{
    public class FileServices : IFileServices
    {
        private readonly TARge25_ShopContext _context;
        private readonly string _uploadsFolder;

        public FileServices(TARge25_ShopContext context)
        {
            _context = context;

            _uploadsFolder = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "multipleFileUpload");
        }

        public void FilesToApi(SpaceshipDto dto, Spaceship domain)
        {
            if (dto.Files != null && dto.Files.Count > 0)
            {
                Directory.CreateDirectory(_uploadsFolder);

                foreach (var file in dto.Files)
                {
                    string uniqueFileName =
                        Guid.NewGuid().ToString() + "_" +
                        Path.GetFileName(file.FileName);

                    string filePath = Path.Combine(
                        _uploadsFolder,
                        uniqueFileName);

                    using (var fileStream = new FileStream(
                        filePath, FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }

                    FileToApi path = new FileToApi
                    {
                        Id = Guid.NewGuid(),
                        ExistingFilePath = uniqueFileName,
                        SpaceshipId = domain.Id
                    };

                    _context.FileToApis.Add(path);
                }
            }
        }

        public async Task<FileToApi> RemoveImageFromApi(FileToApiDto dto)
        {
            var imageId = await _context.FileToApis
                .FirstOrDefaultAsync(x => x.Id == dto.Id);

            if (imageId == null)
            {
                return null;
            }

            string filePath = Path.Combine(
                _uploadsFolder,
                imageId.ExistingFilePath);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            _context.FileToApis.Remove(imageId);
            await _context.SaveChangesAsync();

            return null;
        }

        public async Task<List<FileToApi>> RemoveImagesFromApi(
            FileToApiDto[] dtos)
        {
            foreach (var dto in dtos)
            {
                var imageId = await _context.FileToApis
                    .FirstOrDefaultAsync(
                        x => x.ExistingFilePath == dto.ExistingFilePath);

                if (imageId == null)
                {
                    continue;
                }

                string filePath = Path.Combine(
                    _uploadsFolder,
                    imageId.ExistingFilePath);

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                _context.FileToApis.Remove(imageId);
            }

            await _context.SaveChangesAsync();

            return null;
        }

        public void UploadRealEstateFilesToDatabase(
            RealEstateDto dto,
            RealEstate domain)
        {
            UploadFilesToDataBase(dto.Files, file => new FileToDatabase
            {
                Id = Guid.NewGuid(),
                ImageTitle = file.FileName,
                RealEstateId = domain.Id
            });
        }

        public void UploadKindergartenFilesToDatabase(
            KindergartenDto dto,
            Kindergarten domain)
        {
            UploadFilesToDataBase(dto.Files, file => new FileToDatabase
            {
                Id = Guid.NewGuid(),
                ImageTitle = file.FileName,
                KindergartenId = domain.Id
            });
        }

        private void UploadFilesToDataBase(
            List<IFormFile> fileList,
            Func<IFormFile, FileToDatabase> fileToDatabase)
        {
            if (fileList != null && fileList.Count > 0)
            {
                foreach (var file in fileList)
                {
                    using (var target = new MemoryStream())
                    {
                        FileToDatabase files = fileToDatabase(file);

                        file.CopyTo(target);
                        files.ImageData = target.ToArray();

                        _context.FileToDatabases.Add(files);
                    }
                }
            }
        }

        public async Task<FileToDatabase> RemoveImagesFromDatabase(
            FileToDatabaseDto[] dtos)
        {
            foreach (var dto in dtos)
            {
                var imageId = await _context.FileToDatabases
                    .FirstOrDefaultAsync(x => x.Id == dto.Id);

                if (imageId != null)
                {
                    _context.FileToDatabases.Remove(imageId);
                }
            }

            await _context.SaveChangesAsync();

            return null;
        }

        public async Task<FileToDatabase> RemoveImageFromDatabase(
            FileToDatabaseDto dto)
        {
            var image = await _context.FileToDatabases
                .FirstOrDefaultAsync(x => x.Id == dto.Id);

            if (image == null)
            {
                return null;
            }

            _context.FileToDatabases.Remove(image);
            await _context.SaveChangesAsync();

            return image;
        }
    }
}

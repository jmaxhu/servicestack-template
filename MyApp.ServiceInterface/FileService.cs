using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DayuCloud.Common;
using MyApp.ServiceModel;
using MyApp.ServiceModel.Common;
using MyApp.ServiceModel.Models;
using ServiceStack;

namespace MyApp.ServiceInterface
{
    public class FileService : Service
    {
        private static async Task<string> GetWebUrl()
        {
            return await Task.FromResult(
                Environment.GetEnvironmentVariable("ASPNETCORE_URLS") ?? "http://localhost:5000/"
            );
        }

        private static readonly UploadConfig UploadConfig = HostContext.AppSettings.Get<UploadConfig>("Upload");

        public async Task<List<FileResDto>> Post(SaveUploadFile request)
        {
            if (Request.Files.Length == 0)
            {
                throw new UserFriendlyException("必须上传文件。");
            }

            // 统一的文件保存路径
            var relPath = Path.Combine(
                UploadConfig.UploadPath,
                request.FileType,
                DateTime.Now.ToString("yyyy-MM-dd"));

            var rootPath = Path.Combine(HostContext.AppHost.GetWebRootPath(), relPath);

            if (!Directory.Exists(rootPath))
            {
                Directory.CreateDirectory(rootPath);
            }

            var fileResDtos = new List<FileResDto>(Request.Files.Length);

            foreach (var file in Request.Files)
            {
                if (file.ContentLength > UploadConfig.MaxSize)
                {
                    throw new UserFriendlyException($"最大不能超过: {UploadConfig.MaxSize / 1024 / 1024} mb。");
                }

                var ext = file.FileName.Substring(file.FileName.LastIndexOf('.') + 1).ToLower();

                if (!UploadConfig.AllowExts.Contains(ext))
                {
                    throw new UserFriendlyException($"不支持的文件类型：{ext}.");
                }

                var newName = $"{Guid.NewGuid():N}{DateTime.Now:HHmmss}.{ext}";

                var filePath = Path.Combine(rootPath, newName);

                file.SaveTo(filePath);

                fileResDtos.Add(new FileResDto
                {
                    Name = file.FileName,
                    Path = $"{await GetWebUrl()}{Path.Combine(relPath, newName)}",
                    RelPath = $"{Path.Combine(relPath, newName)}",
                    Size = file.ContentLength,
                    Meta = request.Meta
                });
            }

            return await Task.FromResult(fileResDtos);
        }
    }
}
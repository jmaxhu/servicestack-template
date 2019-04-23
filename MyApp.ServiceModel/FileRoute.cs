using System.Collections.Generic;
using MyApp.ServiceModel.Common;
using ServiceStack;

namespace MyApp.ServiceModel
{
    [Tag("文件上传")]
    [Route("/file", Verbs = "POST", Summary = "文件上传接口")]
    public class SaveUploadFile : IReturn<List<FileResDto>>
    {
        [ApiMember(IsRequired = true, DataType = "string", Description = "自定义的文件类型。作为文件名的一部分。")]
        public string FileType { get; set; }
        
        [ApiMember(DataType = "object", Description = "额外的参数，上传成功后会原样返回。")]
        public object Meta { get; set; }

        [ApiMember(IsRequired = true, DataType = "file", Description = "上传的文件")]
        public string File { get; set; }
    }
}
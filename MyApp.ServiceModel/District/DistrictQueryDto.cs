using MyApp.ServiceModel.Common;
using ServiceStack;

namespace MyApp.ServiceModel.District
{
    public class DistrictQueryDto : IDto
    {
        [ApiMember(IsRequired = true, Description = "父节点id，根节点的父节点为0.")]
        public int ParentId { get; set; }
        
        [ApiMember(Description = "应用key")]
        public string AppKey { get; set; }
    }
}
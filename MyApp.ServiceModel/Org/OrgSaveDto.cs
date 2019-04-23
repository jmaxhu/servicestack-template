using MyApp.ServiceModel.Common;
using ServiceStack;

namespace MyApp.ServiceModel.Org
{
    public class OrgSaveDto: IDto
    {
        [ApiMember(DataType = "long", Description = "组织id,在修改时需要提供.")]
        public long Id { get; set; }

        [ApiMember(DataType = "long", Description = "父组织id, 如果为根组织,值为0.")]
        public long ParentId { get; set; }

        [ApiMember(DataType = "string", Description = "组织名称")]
        public string Name { get; set; }
    }
}
using ServiceStack;

namespace MyApp.ServiceModel.Org
{
    [Route("/org", Verbs = "GET", Notes = "分页取组织列表, 必须提供第几页及每页记录数参数.", Summary = "取组织列表")]
    [Api("组织管理api")]
    public class GetOrgs : OrgQuery, IReturn<OrganizationEntity>
    {
    }

    [Route("/org", Verbs = "POST", Summary = "新增或编辑一个组织")]
    [Api("组织管理api")]
    public class SaveOrg : OrgSaveDto, IReturn<long>
    {
    }

    [Route("/org", Verbs = "DELETE", Summary = "根据组织id删除一个组织")]
    [Api("组织管理api")]
    public class DeleteOrg : IReturn<long>
    {
        [ApiMember(IsRequired = true, DataType = "long", Description = "要删除的组织id")]
        public long Id { get; set; }
    }
}
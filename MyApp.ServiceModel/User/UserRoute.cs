using DayuCloud.Common;
using ServiceStack;

namespace MyApp.ServiceModel.User
{
    [Api("用户管理Api")]
    [Route("/user/{Id}", Verbs = "GET", Summary = "根据用户id得到单个用户信息")]
    public class GetUser : IReturn<UserResDto>
    {
        [ApiMember(IsRequired = true, DataType = "long", Description = "用户id")]
        public long Id { get; set; }
    }

    [Api("用户管理Api")]
    [Route("/user", Verbs = "GET", Summary = "根据条件搜索用户")]
    public class GetUsers : UserQueryDto, IReturn<PagedResult<UserResDto>>
    {
    }

    [Api("用户管理Api")]
    [Route("/user", Verbs = "POST", Summary = "新增或保存一个用户")]
    public class SaveUser : UserSaveDto, IReturn<long>
    {
    }

    [Api("用户管理Api")]
    [Route("/user", Verbs = "DELETE", Summary = "删除一个用户")]
    public class DeleteUser : IReturn<long>
    {
        [ApiMember(IsRequired = true, DataType = "long", Description = "用户id")]
        public long Id { get; set; }
    }
}
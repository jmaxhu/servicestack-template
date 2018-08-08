using System.Threading.Tasks;
using DayuCloud.Common;
using MyApp.Manage;
using MyApp.ServiceModel.User;
using ServiceStack;

namespace MyApp.ServiceInterface
{
    public class UserService : Service
    {
        public IUserManage UserManage{ get; set; }

        public async Task<object> Get(GetUser query)
        {
            return await UserManage.GetUserById(query.Id);
        }

        public async Task<PagedResult<UserResDto>> Get(GetUsers query)
        {
            return await UserManage.GetUsers(query);
        }

        public async Task<long> Post(SaveUser request)
        {
            return await UserManage.SaveUser(request);
        }

        public async Task<long> Delete(DeleteUser request)
        {
            await UserManage.DeleteUser(request.Id);

            return await Task.FromResult(request.Id);
        }
    }
}
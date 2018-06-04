using System.Threading.Tasks;
using DayuCloud.Model.Common;
using MyApp.ServiceModel.User;

namespace MyApp.Manage
{
    /// <summary>
    /// 用户服务
    /// </summary>
    public interface IUserManage
    {
        /// <summary>
        /// 根据查询条件分页得到用户列表
        /// </summary>
        /// <param name="queryDto">用户搜索条件</param>
        /// <returns>分页用户列表</returns>
        Task<PagedResult<UserResDto>> GetUsers(UserQueryDto queryDto);

        /// <summary>
        /// 根据用户id得到用户信息
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <returns>用户</returns>
        Task<UserResDto> GetUserById(long userId);

        /// <summary>
        /// 保存一个用户信息
        /// </summary>
        /// <param name="userSaveDto">用户信息</param>
        /// <returns>返回保存后用户的id</returns>
        Task<long> SaveUser(UserSaveDto userSaveDto);

        /// <summary>
        /// 更新用户密码
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <param name="oldPassword">旧密码</param>
        /// <param name="newPassword">新密码</param>
        /// <returns>如果用户不存在,报异常. 如果旧密码不匹配或强度没有符合要求,报异常.</returns>
        Task ChangePassword(long userId, string oldPassword, string newPassword);

        /// <summary>
        /// 删除一个用户
        /// </summary>
        /// <param name="id">用户id</param>
        /// <returns>如果用户有被使用,则不能删除.</returns>
        Task DeleteUser(long id);
    }
}
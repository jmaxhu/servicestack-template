using System.Collections.Generic;
using System.Threading.Tasks;
using MyApp.ServiceModel.District;
using MyApp.ServiceModel.Permission;
using MyApp.ServiceModel.Role;
using MyApp.ServiceModel.User;
using MyApp.ServiceModel.Common;

namespace MyApp.Manage
{
    /// <summary>
    /// 用户服务
    /// </summary>
    public interface IAccountManage
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
        Task<UserResDto> GetUserById(int userId);

        /// <summary>
        /// 保存一个用户信息
        /// </summary>
        /// <param name="userSaveDto">用户信息</param>
        /// <returns>返回保存后用户的id</returns>
        Task<int> SaveUser(UserSaveDto userSaveDto);

        /// <summary>
        /// 更新用户密码
        /// </summary>
        /// <param name="request">用户修改密码请求</param>
        /// <returns>如果用户不存在,报异常. 如果旧密码不匹配或强度没有符合要求,报异常.</returns>
        Task<int> ChangePassword(UserChangePasswordSaveDto request);

        /// <summary>
        /// 用户重置密码
        /// </summary>
        /// <param name="request">重置请求</param>
        /// <returns></returns>
        Task<int> ResetPassword(UserResetPasswordSaveDto request);

        /// <summary>
        /// 删除一个用户
        /// </summary>
        /// <param name="id">用户id</param>
        /// <returns>如果用户有被使用,则不能删除.</returns>
        Task<int> DeleteUser(int id);

        /// <summary>
        /// 根据条件返回角色分组信息
        /// 只返回一层节点，方便树状展示。
        /// </summary>
        /// <param name="query">分组搜索条件</param>
        /// <returns>角色分组信息</returns>
        Task<Node<RoleGroupResDto>> GetRoleGroups(RoleGroupQueryDto query);

        /// <summary>
        /// 保存一个角色分组
        /// </summary>
        /// <param name="request">角色分组信息</param>
        /// <returns>保存后角色分组的id</returns>
        Task<long> SaveRoleGroup(RoleGroupSaveDto request);

        /// <summary>
        /// 删除一个角色分组
        /// </summary>
        /// <param name="id">分组id</param>
        /// <returns>成功删除时返回id</returns>
        Task<long> DeleteRoleGroup(long id);

        /// <summary>
        /// 根据查询条件得到角色列表
        /// </summary>
        /// <param name="query">角色查询条件</param>
        /// <returns>角色信息列表</returns>
        Task<List<RoleResDto>> GetRoles(RoleQueryDto query);

        /// <summary>
        /// 根据角色id得到角色信息
        /// </summary>
        /// <param name="id">角色id</param>
        /// <returns>角色信息</returns>
        Task<RoleResDto> GetRoleById(long id);

        /// <summary>
        /// 新增或修改角色. 如果角色分组是新的是自动新增分组。
        /// </summary>
        /// <param name="request">角色保存信息</param>
        /// <returns>返回保存后的角色id</returns>
        Task<long> SaveRole(RoleSaveDto request);

        /// <summary>
        /// 根据角色id和应用key删除一个角色。
        /// </summary>
        /// <param name="id">角色id</param>
        /// <returns>返回角色id</returns>
        Task<long> DeleteRole(long id);

        /// <summary>
        /// 给用户授权角色
        /// </summary>
        /// <param name="request">授权相关信息</param>
        /// <returns>成功关联的角色数量</returns>
        Task<int> AssignRoles(AssignRoleSavDto request);

        /// <summary>
        /// 根据应用key得到该应用下的所有权限分组信息
        /// </summary>
        /// <param name="query">包含应用key的搜索条件</param>
        /// <returns></returns>
        Task<Node<PermissionGroupResDto>> GetPermissionGroups(PermissionGroupQueryDto query);

        /// <summary>
        /// 新增或编辑一个权限分组
        /// </summary>
        /// <param name="request">权限分组信息</param>
        /// <returns>返回保存后的分组id</returns>
        Task<long> SavePermissionGroup(PermissionGroupSaveDto request);

        /// <summary>
        /// 删除一个权限分组.
        /// 如果该分组有具体权限项，则不能删除。
        /// </summary>
        /// <param name="id">分组id</param>
        /// <returns>已删除的分组id</returns>
        /// <exception cref="UserFriendlyException">分组下有具体权限项时，不能删除。抛异常。</exception>
        Task<long> DeletePermissionGroup(long id);

        /// <summary>
        /// 根据搜索条件得到授权列表。
        /// 可根据角色，用户，授权分组等条件来筛选。
        /// </summary>
        /// <param name="query">搜索条件</param>
        /// <returns>所有符合条件的授权列表</returns>
        Task<PagedResult<PermissionResDto>> GetPermissions(PermissionQueryDto query);

        /// <summary>
        /// 根据id和应用key得到授权信息
        /// </summary>
        /// <param name="id">授权id</param>
        /// <returns>授权信息</returns>
        Task<PermissionResDto> GetPermissionById(long id);

        /// <summary>
        /// 新增或编辑一个授权
        /// </summary>
        /// <param name="request">授权信息</param>
        /// <returns>返回保存后的新id</returns>
        Task<long> SavePermission(PermissionSaveDto request);

        /// <summary>
        /// 根据id和应用key删除一个授权。
        /// </summary>
        /// <param name="id">授权id</param>
        /// <returns>删除的授权id</returns>
        Task<long> DeletePermission(long id);


        /// <summary>
        /// 根据父节点的id得到该节点下的直接子节点(地区)的列表
        /// </summary>
        /// <param name="query">包含父节点id的搜索条件</param>
        /// <returns>子节点列表</returns>
        Task<List<DistrictResDto>> GetDistricts(DistrictQueryDto query);

        /// <summary>
        /// 根据编号得到某个行政区划的信息
        /// </summary>
        /// <param name="id">编号</param>
        /// <returns>单个行政区划信息</returns>
        Task<DistrictResDto> GetDistrictById(int id);

        /// <summary>
        /// 根据行政区划代码查询该信息
        /// </summary>
        /// <param name="code">行政区划代码（邮编)</param>
        /// <returns></returns>
        Task<DistrictResDto> GetDistrictByCode(string code);

        /// <summary>
        /// 保存一个行政区划
        /// </summary>
        /// <param name="request">行政区划信息</param>
        /// <returns>新只在的编号</returns>
        Task<int> SaveDistrict(DistrictSaveDto request);

        /// <summary>
        /// 删除一个行政区划
        /// </summary>
        /// <param name="id">行政区划id</param>
        /// <returns>删除的id</returns>
        Task<int> DeleteDistrict(int id);
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;
using MyApp.Manage;
using MyApp.ServiceModel;
using MyApp.ServiceModel.District;
using MyApp.ServiceModel.Permission;
using MyApp.ServiceModel.Role;
using MyApp.ServiceModel.User;
using MyApp.ServiceModel.Common;
using ServiceStack;
using AssignRoles = MyApp.ServiceModel.AssignRoles;

namespace MyApp.ServiceInterface.Service
{
    /// <summary>
    /// 账户服务，包括：用户，角色，权限
    /// </summary>
    [Authenticate]
    public class AccountService : ServiceStack.Service
    {
        public IAccountManage AccountManage { get; set; }

        public virtual async Task<object> Get(GetUser query)
        {
            return await AccountManage.GetUserById(query.Id);
        }

        public virtual async Task<PagedResult<UserResDto>> Get(GetUsers query)
        {
            return await AccountManage.GetUsers(query);
        }

        public virtual async Task<long> Post(SaveUser request)
        {
            return await AccountManage.SaveUser(request);
        }


        public virtual async Task<long> Delete(DeleteUser request)
        {
            await AccountManage.DeleteUser(request.Id, request.AppKey);

            return request.Id;
        }

        public virtual async Task<Node<RoleGroupResDto>> Get(GetRoleGroups query)
        {
            return await AccountManage.GetRoleGroups(query);
        }

        public virtual async Task<long> Post(SaveRoleGroup request)
        {
            return await AccountManage.SaveRoleGroup(request);
        }

        public virtual async Task<long> Delete(DeleteRoleGroup request)
        {
            return await AccountManage.DeleteRoleGroup(request.Id, request.AppKey);
        }

        public virtual async Task<List<RoleResDto>> Get(GetRoles query)
        {
            return await AccountManage.GetRoles(query);
        }

        public virtual async Task<RoleResDto> Get(GetRole query)
        {
            return await AccountManage.GetRoleById(query.Id, query.AppKey);
        }

        public virtual async Task<long> Post(SaveRole request)
        {
            return await AccountManage.SaveRole(request);
        }

        public virtual async Task<long> Delete(DeleteRole request)
        {
            return await AccountManage.DeleteRole(request.Id, request.AppKey);
        }

        public virtual async Task<int> Post(AssignRoles request)
        {
            return await AccountManage.AssignRoles(request);
        }

        public virtual async Task<Node<PermissionGroupResDto>> Get(GetPermissionGroups query)
        {
            if (query.AppKey == "0")
            {
                var local = new LocalAccountManage<User>();
                return await local.GetPermissionGroups(query);
            }

            return await AccountManage.GetPermissionGroups(query);
        }

        public virtual async Task<long> Post(SavePermissionGroup request)
        {
            if (request.AppKey == "0")
            {
                var local = new LocalAccountManage<User>();
                return await local.SavePermissionGroup(request);
            }

            return await AccountManage.SavePermissionGroup(request);
        }

        public virtual async Task<long> Delete(DeletePermissionGroup request)
        {
            if (request.AppKey == "0")
            {
                var local = new LocalAccountManage<User>();
                return await local.DeletePermissionGroup(request.Id, "");
            }

            return await AccountManage.DeletePermissionGroup(request.Id, request.AppKey);
        }

        public virtual async Task<PagedResult<PermissionResDto>> Get(GetPermissions query)
        {
            if (query.AppKey == "0")
            {
                var local = new LocalAccountManage<User>();
                return await local.GetPermissions(query);
            }

            return await AccountManage.GetPermissions(query);
        }

        public virtual async Task<PermissionResDto> Get(GetPermissionById query)
        {
            if (query.AppKey == "0")
            {
                var local = new LocalAccountManage<User>();
                return await local.GetPermissionById(query.Id, "");
            }

            return await AccountManage.GetPermissionById(query.Id, query.AppKey);
        }

        public virtual async Task<long> Post(SavePermission request)
        {
            if (request.AppKey == "0")
            {
                var local = new LocalAccountManage<User>();
                return await local.SavePermission(request);
            }

            return await AccountManage.SavePermission(request);
        }

        public virtual async Task<long> Delete(DeletePermission request)
        {
            if (request.AppKey == "0")
            {
                var local = new LocalAccountManage<User>();
                return await local.DeletePermission(request.Id, "");
            }

            return await AccountManage.DeletePermission(request.Id, request.AppKey);
        }

        public virtual async Task<int> Post(ChangePassword request)
        {
            return await AccountManage.ChangePassword(request);
        }

        public virtual async Task<int> Post(ResetPassword request)
        {
            return await AccountManage.ResetPassword(request);
        }

        public virtual async Task<List<DistrictResDto>> Get(GetDistricts query)
        {
            return await AccountManage.GetDistricts(query);
        }

        public virtual async Task<DistrictResDto> Get(GetDistrict query)
        {
            return await AccountManage.GetDistrictById(query.Id, query.AppKey);
        }

        public virtual async Task<DistrictResDto> Get(GetDistrictByCode query)
        {
            return await AccountManage.GetDistrictByCode(query.Code, query.AppKey);
        }

        public virtual async Task<int> Post(SaveDistrict request)
        {
            return await AccountManage.SaveDistrict(request);
        }

        public virtual async Task<int> Delete(DeleteDistrict request)
        {
            return await AccountManage.DeleteDistrict(request.Id, request.AppKey);
        }
    }
}
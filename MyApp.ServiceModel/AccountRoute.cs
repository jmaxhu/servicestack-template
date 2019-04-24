using System;
using System.Collections.Generic;
using MyApp.ServiceModel.District;
using MyApp.ServiceModel.Permission;
using MyApp.ServiceModel.Role;
using MyApp.ServiceModel.User;
using MyApp.ServiceModel.Common;
using ServiceStack;

namespace MyApp.ServiceModel
{
    [Tag("账户管理")]
    [Route("/account/user", Verbs = "GET", Summary = "查询单个用户", Notes = "根据用户id得到单个用户信息")]
    public class GetUser : IReturn<UserResDto>
    {
        [ApiMember(IsRequired = true, DataType = "integer", Description = "用户id")]
        public int Id { get; set; }
    }

    [Tag("账户管理")]
    [Route("/account/users", Verbs = "GET", Summary = "查询用户列表", Notes = "根据条件搜索用户")]
    public class GetUsers : UserQueryDto, IReturn<PagedResult<UserResDto>>
    {
    }

    [Tag("账户管理")]
    [Route("/account/user", Verbs = "POST", Summary = "保存一个用户", Notes = "新增或保存一个用户")]
    public class SaveUser : UserSaveDto, IReturn<long>
    {
    }

    [Tag("账户管理")]
    [Route("/account/user", Verbs = "DELETE", Summary = "删除一个用户", Notes = "删除一个用户")]
    public class DeleteUser : IReturn<long>
    {
        [ApiMember(IsRequired = true, DataType = "integer", Description = "用户id")]
        public int Id { get; set; }
    }

    [Tag("账户管理")]
    [Route("/account/role_group", Verbs = "GET", Summary = "查询角色分组", Notes = "根据条件得到角色分组列表")]
    public class GetRoleGroups : RoleGroupQueryDto, IReturn<Node<RoleGroupResDto>>
    {
    }

    [Tag("账户管理")]
    [Route("/account/role_group", Verbs = "POST", Summary = "新增或编辑一个角色分组", Notes = @"
    角色分组只能添加到授权的应用下。分组下如果已经包含具体角色，则不能添加。 分组编辑时不能更换父分组。
    ")]
    public class SaveRoleGroup : RoleGroupSaveDto, IReturn<long>
    {
    }

    [Tag("账户管理")]
    [Route("/account/role_group", Verbs = "DELETE", Summary = "删除角色分组", Notes = @"
    如果分组包含子分组或者分组包含具体角色，则不允许删除。如果待删除的分组不在授权应用内，则也不允许删除。"
    )]
    public class DeleteRoleGroup : IReturn<long>
    {
        [ApiMember(IsRequired = true, DataType = "integer", Format = "int64", Description = "角色分组id")]
        public long Id { get; set; }
    }

    [Tag("账户管理")]
    [Route("/account/roles", Verbs = "GET", Summary = "查询角色", Notes = "根据条件得到角色列表")]
    public class GetRoles : RoleQueryDto, IReturn<List<RoleResDto>>
    {
    }

    [Tag("账户管理")]
    [Route("/account/role", Verbs = "GET", Summary = "查询单个角色", Notes = "根据id得到角色列表")]
    public class GetRole : IReturn<RoleResDto>
    {
        [ApiMember(IsRequired = true, DataType = "integer", Format = "int64", Description = "角色id")]
        public long Id { get; set; }
    }

    [Tag("账户管理")]
    [Route("/account/role", Verbs = "POST", Summary = "保存一个角色", Notes = "新增或修改一个角色")]
    public class SaveRole : RoleSaveDto, IReturn<long>
    {
    }

    [Tag("账户管理")]
    [Route("/account/role", Verbs = "DELETE", Summary = "删除一个角色", Notes = "删除一个角色")]
    public class DeleteRole : IReturn<long>
    {
        [ApiMember(IsRequired = true, DataType = "integer", Format = "int64", Description = "角色id")]
        public long Id { get; set; }
    }

    [Tag("账户管理")]
    [Route("/account/assign_role", Verbs = "POST", Summary = "用户分配角色", Notes = "给用户分配角色")]
    public class AssignRoles : AssignRoleSavDto, IReturn<int>
    {
    }

    [Tag("账户管理")]
    [Route("/account/permission_group", Verbs = "GET", Summary = "查询权限分组", Notes = "按条件读取权限分组")]
    public class GetPermissionGroups : PermissionGroupQueryDto, IReturn<Node<PermissionGroupResDto>>
    {
    }

    [Tag("账户管理")]
    [Route("/account/permission_group", Verbs = "POST", Summary = "保存权限分组", Notes = "新增或修改一个权限分组")]
    public class SavePermissionGroup : PermissionGroupSaveDto, IReturn<long>
    {
    }

    [Tag("账户管理")]
    [Route("/account/permission_group", Verbs = "DELETE", Summary = "删除权限分组", Notes = "删除一个权限分组")]
    public class DeletePermissionGroup : IReturn<long>
    {
        [ApiMember(IsRequired = true, DataType = "integer", Format = "int64", Description = "权限分组id")]
        public long Id { get; set; }
    }

    [Tag("账户管理")]
    [Route("/account/permission", Verbs = "GET", Summary = "查询权限列表", Notes = "根据条件得到权限列表")]
    public class GetPermissions : PermissionQueryDto, IReturn<PagedResult<PermissionResDto>>
    {
    }

    [Tag("账户管理")]
    [Route("/account/permission_by_id", Verbs = "GET", Summary = "查询单个权限", Notes = "根据id得到权限")]
    public class GetPermissionById : IReturn<PermissionResDto>
    {
        [ApiMember(IsRequired = true, DataType = "integer", Format = "int64", Description = "权限项id")]
        public long Id { get; set; }
    }

    [Tag("账户管理")]
    [Route("/account/permission", Verbs = "POST", Summary = "保存权限", Notes = "新增或保存一个权限")]
    public class SavePermission : PermissionSaveDto, IReturn<long>
    {
    }

    [Tag("账户管理")]
    [Route("/account/permission", Verbs = "DELETE", Summary = "删除权限", Notes = "删除一个权限项")]
    public class DeletePermission : IReturn<long>
    {
        [ApiMember(IsRequired = true, DataType = "integer", Format = "int64", Description = "权限id")]
        public long Id { get; set; }
    }


    [Tag("账户管理")]
    [Route("/account/change_pwd", Verbs = "POST", Summary = "修改密码", Notes = "修改用户的密码")]
    public class ChangePassword : UserChangePasswordSaveDto, IReturn<int>
    {
    }

    [Tag("账户管理")]
    [Route("/account/reset_pwd", Verbs = "POST", Summary = "重置密码", Notes = "重置用户的密码")]
    public class ResetPassword : UserResetPasswordSaveDto, IReturn<int>
    {
    }

    [Tag("账户管理")]
    [Route("/account/districts", Verbs = "GET", Summary = "查询行政区划", Notes = "根据父节点查询行政区划，返回该节点下的所有同级行政区划信息。")]
    public class GetDistricts : DistrictQueryDto, IReturn<List<DistrictResDto>>
    {
    }

    [Tag("账户管理")]
    [Route("/account/district", Verbs = "GET", Summary = "查询行政区划", Notes = "根据编号查询行政区划")]
    public class GetDistrict : IReturn<DistrictResDto>
    {
        [ApiMember(IsRequired = true, DataType = "integer", Description = "行政区划id")]
        public int Id { get; set; }
    }

    [Tag("账户管理")]
    [Route("/account/district_by_code", Verbs = "GET", Summary = "查询行政区划", Notes = "根据编码查询行政区划")]
    public class GetDistrictByCode : IReturn<DistrictResDto>
    {
        [ApiMember(IsRequired = true, Description = "行政区划编码")]
        public string Code { get; set; }
    }

    [Tag("账户管理")]
    [Route("/account/district", Verbs = "POST", Summary = "保存行政区划", Notes = "保存行政区划信息")]
    public class SaveDistrict : DistrictSaveDto, IReturn<int>
    {
    }

    [Tag("账户管理")]
    [Route("/account/district", Verbs = "DELETE", Summary = "删除一个行政区划", Notes = "根据编号删除一个行政区划")]
    public class DeleteDistrict : IReturn<int>
    {
        [ApiMember(IsRequired = true, Description = "行政区划id")]
        public int Id { get; set; }
    }
}
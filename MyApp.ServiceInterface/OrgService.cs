﻿using System.Threading.Tasks;
using DayuCloud.Common;
using MyApp.Manage;
using MyApp.ServiceModel.Org;
using ServiceStack;

namespace MyApp.ServiceInterface
{
    public class OrgService : Service
    {
        public IOrgManage OrgManage{ get; set; }

        public async Task<PagedResult<Organization>> Get(GetOrgs request)
        {
            return await OrgManage.GetOrganizations(request);
        }

        public async Task<long> Post(SaveOrg request)
        {
            var org = request.ConvertTo<Organization>();

            return await OrgManage.SaveOrganization(org);
        }

        public async Task<long> Delete(DeleteOrg request)
        {
            await OrgManage.DeleteOrganization(request.Id);

            return await Task.FromResult<long>(request.Id);
        }
    }
}
using System.Linq;
using System.Threading.Tasks;
using DayuCloud.Common;
using MyApp.ServiceModel.Org;
using NUnit.Framework;
using ServiceStack.OrmLite;

namespace MyApp.Tests.UnitTests
{
    [TestFixture]
    public class OrganizationTest : UnitTestBase
    {
        [Test]
        public async Task Can_Save_Organization()
        {
            var org = new Organization {ParentId = 0, Name = "水利厅"};
            var orgId = await OrgManage.SaveOrganization(org);

            Assert.True(org.Id > 0);

            var dbOrg = await Db.SingleByIdAsync<Organization>(orgId);

            Assert.AreEqual(orgId, dbOrg.Id);
            Assert.AreEqual(org.Name, dbOrg.Name);
            Assert.AreEqual(org.ParentId, dbOrg.ParentId);
        }

        [Test]
        public async Task Can_Read_ParentName()
        {
            var oneChild = await Db.SingleAsync<Organization>(x => x.ParentId > 0);
            var parentOrg = await Db.SingleByIdAsync<Organization>(oneChild.ParentId);

            var org = await OrgManage.GetOrganizationById(oneChild.Id);

            Assert.AreEqual(oneChild.Id, org.Id);
            Assert.AreEqual(oneChild.ParentId, org.ParentId);
            Assert.AreEqual(parentOrg.Name, org.ParentName);
        }

        [Test]
        public async Task Query_Root_Org_Test()
        {
            var rootOrgs = await Db.SelectAsync<Organization>(x => x.ParentId == 0);

            var orgs = await OrgManage.GetRootOrganizations();

            Assert.AreEqual(rootOrgs.Count, orgs.Count);

            rootOrgs.ForEach(org =>
            {
                var dbOrg = orgs.FirstOrDefault(x => x.Id == org.Id);
                Assert.NotNull(dbOrg);
                Assert.AreEqual(org.Name, dbOrg.Name);
                Assert.AreEqual(org.ParentId, dbOrg.ParentId);
            });
        }

        [Test]
        public async Task Can_Query_Organization()
        {
            var parentId = await Db.ScalarAsync<long>(
                Db.From<Organization>().Where(x => x.ParentId == 0).Select(x => Sql.Max(x.Id)));

            var childCount = await Db.CountAsync<Organization>(x => x.ParentId == parentId);

            var queryResults = await OrgManage.GetOrganizations(
                new OrgQuery {PageIndex = 1, PageSize = 10, ParentId = parentId});

            Assert.AreEqual(queryResults.Total, childCount);

            var searchCount = await Db.CountAsync<Organization>(x => x.Name.Contains("组织3"));

            queryResults = await OrgManage.GetOrganizations(
                new OrgQuery {PageIndex = 1, PageSize = 10, SearchName = "组织3"});

            Assert.AreEqual(queryResults.Total, searchCount);
        }

        [Test]
        public async Task Can_Delete_Organization()
        {
            var org = new Organization {ParentId = 0, Name = "待删除"};
            var orgId = await Db.InsertAsync(org, true);

            Assert.IsTrue(orgId > 0);

            await OrgManage.DeleteOrganization(orgId);

            var exist = await Db.ExistsAsync<Organization>(x => x.Id == orgId);

            Assert.IsFalse(exist);
        }

        [Test]
        public async Task Can_Update_Organization()
        {
            var org = new Organization {ParentId = 0, Name = "变更前"};
            var orgId = await Db.InsertAsync(org, true);

            org.Id = orgId;
            org.Name = "变更后";

            await OrgManage.SaveOrganization(org);

            var newOrg = await Db.SingleByIdAsync<Organization>(orgId);
            Assert.AreEqual(newOrg.Name, "变更后");

            //同组织下名称不能重复
            var oneChild = new Organization {ParentId = orgId, Name = "子组织1"};
            var wrongChild = new Organization {ParentId = orgId, Name = "子组织1"};
            await Db.InsertAsync(oneChild);

            Assert.ThrowsAsync<UserFriendlyException>(async () =>
                await OrgManage.SaveOrganization(wrongChild));
        }
    }
}
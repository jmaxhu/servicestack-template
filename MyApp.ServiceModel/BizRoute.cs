using ServiceStack;

namespace MyApp.ServiceModel
{
    [Tag("统一的业务表操作")]
    [Route("/biz", Verbs = "GET", Summary = "统一的业务表搜索入口")]
    public class GetBizs : QueryBase, IReturn<IQueryResponse>
    {
        [ApiMember(DataType = "string", Description = "表名", IsRequired = true)]
        public string TableName { get; set; }
    }

    [Tag("统一的业务表操作")]
    [Route("/biz", Verbs = "POST", Summary = "统一的业务表保存")]
    public class SaveBiz : IReturn<long>
    {
        [ApiMember(DataType = "string", Description = "表名", IsRequired = true)]
        public string TableName { get; set; }
        
        [ApiMember(DataType = "long", Description = "数据目录id", IsRequired = true)]
        public long DataCatalogId { get; set; }
    }

    [Tag("统一的业务表操作")]
    [Route("/biz", Verbs = "DELETE", Summary = "统一的业务表删除")]
    public class DeleteBiz : IReturn<long>
    {
        [ApiMember(DataType = "long", Description = "业务表记录id", IsRequired = true)]
        public long Id { get; set; }

        [ApiMember(DataType = "string", Description = "表名", IsRequired = true)]
        public string TableName { get; set; }
    }
}
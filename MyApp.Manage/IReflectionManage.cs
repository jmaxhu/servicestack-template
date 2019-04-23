using System.Reflection;
using System.Threading.Tasks;
using MyApp.ServiceModel.Common;

namespace MyApp.Manage
{
    public interface IReflectionManage
    {
        /// <summary>
        /// 根据实体定义动态生成类型
        /// </summary>
        /// <param name="entityDef">实体定义</param>
        /// <returns>动态生成的类型. 只包含属性.</returns>
        Task<TypeInfo> CreateEntityType(EntityDef entityDef);
    }
}
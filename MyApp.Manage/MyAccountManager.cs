using DayuCloud.Account.Manage;
using DayuCloud.Account.Model.User;

namespace MyApp.Manage
{
    public class MyAccountManager<TU> : LocalAccountManage<TU> where TU : User
    {
        // TODO: 根据需要重写相关方法 
    }
}
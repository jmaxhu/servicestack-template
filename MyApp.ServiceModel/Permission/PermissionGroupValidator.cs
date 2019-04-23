using ServiceStack.FluentValidation;

namespace MyApp.ServiceModel.Permission
{
    public class PermissionGroupValidator : AbstractValidator<PermissionGroupSaveDto>
    {
        public PermissionGroupValidator()
        {
            RuleFor(x => x.AppKey)
                .NotEmpty()
                .WithMessage("用户key不能为空.");

            RuleFor(x => x.Name)
                .Length(2, 100)
                .WithMessage("分组名称不能为空，长度范围为：2~100。");
        }
    }
}
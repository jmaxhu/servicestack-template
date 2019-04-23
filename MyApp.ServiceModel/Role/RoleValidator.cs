using ServiceStack.FluentValidation;

namespace MyApp.ServiceModel.Role
{
    public class RoleValidator : AbstractValidator<RoleSaveDto>
    {
        public RoleValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .Length(2, 20)
                .WithMessage("角色名称不能为空。并且长度在2~20之间。");

            RuleFor(x => x.AppKey)
                .NotEmpty()
                .WithMessage("应用key不能为空。");
        }
    }
}
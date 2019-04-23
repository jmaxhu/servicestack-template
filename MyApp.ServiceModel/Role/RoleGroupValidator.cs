using ServiceStack.FluentValidation;

namespace MyApp.ServiceModel.Role
{
    public class RoleGroupValidator : AbstractValidator<RoleGroupSaveDto>
    {
        public RoleGroupValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .Length(2, 20)
                .WithMessage("角色分组名称的长度必须大于2，并且小于20.");
        }
    }
}
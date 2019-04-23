using ServiceStack.FluentValidation;

namespace MyApp.ServiceModel.Permission
{
    public class PermissionValidator : AbstractValidator<PermissionSaveDto>
    {
        public PermissionValidator()
        {
            RuleFor(x => x.PermissionGroupId)
                .NotEmpty()
                .NotNull()
                .GreaterThanOrEqualTo(0L)
                .WithMessage("授权分组必须提供。");

            RuleFor(x => x.Code)
                .NotEmpty()
                .NotNull()
                .Length(2, 100)
                .WithMessage("授权编码必须提供, 建议使用英文。长度为：2~100。");

            RuleFor(x => x.Name)
                .NotEmpty()
                .NotNull()
                .Length(2, 100)
                .WithMessage("授权名称必须提供。长度为：2~100。");
        }
    }
}
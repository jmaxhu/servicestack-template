using ServiceStack.FluentValidation;

namespace MyApp.ServiceModel.User
{
    public class UserValidator : AbstractValidator<UserSaveDto>
    {
        public UserValidator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty()
                .Length(4, 20)
                .WithMessage("用户名不能为空，长度必须是 4～20 之间。");

            RuleFor(x => x.DisplayName)
                .NotEmpty()
                .WithMessage("用户姓名不能为空。");
        }
    }
}
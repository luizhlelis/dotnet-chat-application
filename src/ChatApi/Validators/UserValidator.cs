using FluentValidation;

namespace ChatApi.Validators
{
    public class UserValidator : AbstractValidator<User>
    {
        public UserValidator()
        {
            RuleFor(x => x.Username).NotNull().NotEmpty().WithMessage("The username must not be empty not null");
            RuleFor(x => x.Password).NotNull().NotEmpty().WithMessage("The password must not be empty not null");
            RuleFor(x => x.Username).MaximumLength(10).WithMessage("The username must contain less than 10 characters");
        }
    }
}

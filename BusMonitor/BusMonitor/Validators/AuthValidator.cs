using FluentValidation;
using BusMonitor.Controllers;
using BusMonitor.DTOs;

namespace BusMonitor.Validators
{
    public class LoginModelValidator : AbstractValidator<LoginModel>
    {
        public LoginModelValidator()
        {
            RuleFor(x => x.Username).NotEmpty();
            RuleFor(x => x.Password).NotEmpty();
        }
    }
} 
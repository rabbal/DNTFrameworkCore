using System;
using System.Linq;
using DNTFrameworkCore.EFCore.Context;
using DNTFrameworkCore.FluentValidation;
using DNTFrameworkCore.TestWebApp.Application.Identity.Models;
using DNTFrameworkCore.TestWebApp.Domain.Identity;
using DNTFrameworkCore.TestWebApp.Helpers;
using DNTFrameworkCore.TestWebApp.Resources;
using FluentValidation;

namespace DNTFrameworkCore.TestWebApp.Application.Identity.Validators
{
    public class UserValidator : FluentModelValidator<UserModel>
    {
        private readonly IUnitOfWork _uow;

        public UserValidator(IUnitOfWork uow, IMessageLocalizer localizer)
        {
            _uow = uow ?? throw new ArgumentNullException(nameof(uow));

            RuleFor(m => m.DisplayName).NotEmpty()
                .WithMessage(localizer["User.Fields.DisplayName.Required"])
                .MinimumLength(3)
                .WithMessage(localizer["User.Fields.DisplayName.MinimumLength"])
                .MaximumLength(User.MaxDisplayNameLength)
                .WithMessage(localizer["User.Fields.DisplayName.MaximumLength"])
                .Matches(@"^[\u0600-\u06FF,\u0590-\u05FF,0-9\s]*$")
                .WithMessage(localizer["User.Fields.DisplayName.RegularExpression"])
                .DependentRules(() =>
                {
                    RuleFor(m => m).Must(model =>
                         !CheckDuplicateDisplayName(model.DisplayName, model.Id))
                        .WithMessage(localizer["User.Fields.DisplayName.Unique"])
                        .OverridePropertyName(nameof(UserModel.DisplayName));
                });

            RuleFor(m => m.UserName).NotEmpty()
                .WithMessage(localizer["User.Fields.UserName.Required"])
                .MinimumLength(3)
                .WithMessage(localizer["User.Fields.UserName.MinimumLength"])
                .MaximumLength(User.MaxUserNameLength)
                .WithMessage(localizer["User.Fields.UserName.MaximumLength"])
                .Matches("^[a-zA-Z0-9_]*$")
                .WithMessage(localizer["User.Fields.UserName.RegularExpression"])
                .DependentRules(() =>
                {
                    RuleFor(m => m).Must(model =>
                         !CheckDuplicateUserName(model.UserName, model.Id))
                        .WithMessage(localizer["User.Fields.UserName.Unique"])
                        .OverridePropertyName(nameof(UserModel.UserName));
                });

            RuleFor(m => m.Password).NotEmpty()
                .WithMessage(localizer["User.Fields.Password.Required"])
                .When(m => m.IsNew(), ApplyConditionTo.CurrentValidator)
                .MinimumLength(6)
                .WithMessage(localizer["User.Fields.Password.MinimumLength"])
                .MaximumLength(User.MaxPasswordLength)
                .WithMessage(localizer["User.Fields.Password.MaximumLength"]);

            RuleFor(m => m).Must(model => !CheckDuplicateRoles(model))
                .WithMessage(localizer["User.Fields.Roles.Unique"])
                .When(m => m.Roles != null && m.Roles.Any(r => !r.IsDeleted()));
        }

        private bool CheckDuplicateUserName(string userName, long id)
        {
            var normalizedUserName = userName.ToUpperInvariant();
            return _uow.Set<User>().Any(u => u.NormalizedUserName == normalizedUserName && u.Id != id);
        }

        private bool CheckDuplicateDisplayName(string displayName, long id)
        {
            var normalizedDisplayName = displayName.NormalizePersianTitle();
            return _uow.Set<User>().Any(u => u.NormalizedDisplayName == normalizedDisplayName && u.Id != id);
        }

        private bool CheckDuplicateRoles(UserModel model)
        {
            var roles = model.Roles.Where(a => !a.IsDeleted());
            return roles.GroupBy(r => r.RoleId).Any(r => r.Count() > 1);
        }
    }
}
using System;
using System.Linq;
using DNTFrameworkCore.EFCore.Context;
using DNTFrameworkCore.FluentValidation;
using FluentValidation;
using ProjectName.Application.Identity.Models;
using ProjectName.Common.Localization;
using ProjectName.Domain.Identity;

namespace ProjectName.Application.Identity.Validators
{
    public class UserValidator : FluentModelValidator<UserModel>
    {
        private readonly IDbContext _dbContext;

        public UserValidator(IDbContext dbContext, ITranslationService translation)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

            RuleFor(m => m.DisplayName).NotEmpty()
                .WithMessage(translation["User.Fields.DisplayName.Required"])
                .MinimumLength(3)
                .WithMessage(translation["User.Fields.DisplayName.MinimumLength"])
                .MaximumLength(User.DisplayNameLength)
                .WithMessage(translation["User.Fields.DisplayName.MaximumLength"])
                .DependentRules(() =>
                {
                    RuleFor(m => m).Must(model =>
                         IsUniqueDisplayName(model.DisplayName, model.Id))
                        .WithMessage(translation["User.Fields.DisplayName.Unique"])
                        .OverridePropertyName(nameof(UserModel.DisplayName));
                });

            RuleFor(m => m.UserName).NotEmpty()
                .WithMessage(translation["User.Fields.UserName.Required"])
                .MinimumLength(3)
                .WithMessage(translation["User.Fields.UserName.MinimumLength"])
                .MaximumLength(User.UserNameLength)
                .WithMessage(translation["User.Fields.UserName.MaximumLength"])
                .Matches("^[a-zA-Z0-9_]*$")
                .WithMessage(translation["User.Fields.UserName.RegularExpression"])
                .DependentRules(() =>
                {
                    RuleFor(m => m).Must(model =>
                         IsUniqueUserName(model.UserName, model.Id))
                        .WithMessage(translation["User.Fields.UserName.Unique"])
                        .OverridePropertyName(nameof(UserModel.UserName));
                });

            RuleFor(m => m.Password).NotEmpty()
                .WithMessage(translation["User.Fields.Password.Required"])
                .When(m => m.IsNew(), ApplyConditionTo.CurrentValidator)
                .MinimumLength(6)
                .WithMessage(translation["User.Fields.Password.MinimumLength"])
                .MaximumLength(User.PasswordLength)
                .WithMessage(translation["User.Fields.Password.MaximumLength"]);

            RuleFor(m => m).Must(model => !CheckDuplicateRoles(model))
                .WithMessage(translation["User.Fields.Roles.Unique"])
                .When(m => m.Roles != null && m.Roles.Any(r => !r.IsDeleted()));
        }

        private bool IsUniqueUserName(string userName, long id)
        {
            return !_dbContext.Set<User>().Any(u => u.NormalizedUserName == User.NormalizeUserName(userName) && u.Id != id);
        }

        private bool IsUniqueDisplayName(string displayName, long id)
        {
            return !_dbContext.Set<User>().Any(u => u.NormalizedDisplayName == User.NormalizeDisplayName(displayName) && u.Id != id);
        }

        private bool CheckDuplicateRoles(UserModel model)
        {
            var roles = model.Roles.Where(a => !a.IsDeleted());
            return roles.GroupBy(r => r.RoleId).Any(r => r.Count() > 1);
        }
    }
}
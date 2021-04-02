using System;
using System.Linq;
using DNTFrameworkCore.EFCore.Context;
using DNTFrameworkCore.FluentValidation;
using FluentValidation;
using ProjectName.Application.Identity.Models;
using ProjectName.Application.Localization;
using ProjectName.Domain.Identity;

namespace ProjectName.Application.Identity.Validators
{
    public class RoleValidator : FluentModelValidator<RoleModel>
    {
        private readonly IDbContext _dbContext;

        public RoleValidator(IDbContext dbContext, ITranslationService translation)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

            RuleFor(m => m.Name).NotEmpty()
                .WithMessage(translation["Role.Fields.Name.Required"])
                .MinimumLength(3)
                .WithMessage(translation["Role.Fields.Name.MinimumLength"])
                .MaximumLength(Role.NameLength)
                .WithMessage(translation["Role.Fields.Name.MaximumLength"])
                .DependentRules(() =>
                {
                    RuleFor(m => m).Must(model =>
                            IsUniqueName(model.Name, model.Id))
                        .WithMessage(translation["Role.Fields.Name.Unique"])
                        .OverridePropertyName(nameof(RoleModel.Name));
                });

            RuleFor(m => m.Description)
                .MaximumLength(Role.DescriptionLength)
                .WithMessage(translation["Role.Fields.Description.MaximumLength"]);

            RuleFor(m => m).Must(model => !CheckDuplicatePermissions(model))
                .WithMessage(translation["Role.Fields.Permissions.Unique"])
                .When(m => m.Permissions != null && m.Permissions.Any(r => !r.IsDeleted()));
        }

        private bool IsUniqueName(string name, long id)
        {
            return !_dbContext.Set<Role>().Any(u => u.NormalizedName == Role.NormalizeName(name) && u.Id != id);
        }

        private bool CheckDuplicatePermissions(RoleModel model)
        {
            var permissions = model.Permissions.Where(a => !a.IsDeleted());
            return permissions.GroupBy(r => r.Name).Any(r => r.Count() > 1);
        }
    }
}
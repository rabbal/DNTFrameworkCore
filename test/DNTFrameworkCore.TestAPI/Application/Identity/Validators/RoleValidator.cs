using System;
using System.Linq;
using DNTFrameworkCore.EFCore.Context;
using DNTFrameworkCore.FluentValidation;
using DNTFrameworkCore.TestAPI.Application.Identity.Models;
using DNTFrameworkCore.TestAPI.Domain.Identity;
using DNTFrameworkCore.TestAPI.Resources;
using FluentValidation;

namespace DNTFrameworkCore.TestAPI.Application.Identity.Validators
{
    public class RoleValidator : FluentModelValidator<RoleModel>
    {
        private readonly IUnitOfWork _uow;

        public RoleValidator(IUnitOfWork uow, IMessageLocalizer localizer)
        {
            _uow = uow ?? throw new ArgumentNullException(nameof(uow));

            RuleFor(m => m.Name).NotEmpty()
                .WithMessage(localizer["Role.Fields.Name.Required"])
                .MinimumLength(3)
                .WithMessage(localizer["Role.Fields.Name.MinimumLength"])
                .MaximumLength(Role.MaxNameLength)
                .WithMessage(localizer["Role.Fields.Name.MaximumLength"])
                .DependentRules(() =>
                {
                    RuleFor(m => m).Must(model =>
                         !CheckDuplicateName(model.Name, model.Id))
                        .WithMessage(localizer["Role.Fields.Name.Unique"])
                        .OverridePropertyName(nameof(RoleModel.Name));
                });

            RuleFor(m => m.Description)
                .MaximumLength(Role.MaxDescriptionLength)
                .WithMessage(localizer["Role.Fields.Description.MaximumLength"]);

            RuleFor(m => m).Must(model => !CheckDuplicatePermissions(model))
                .WithMessage(localizer["Role.Fields.Permissions.Unique"])
                .When(m => m.Permissions != null && m.Permissions.Any(r => !r.IsDeleted()));
        }

        private bool CheckDuplicateName(string name, long id)
        {
            var normalizedName = name.ToUpperInvariant();
            return _uow.Set<Role>().Any(u => u.NormalizedName == normalizedName && u.Id != id);
        }

        private bool CheckDuplicatePermissions(RoleModel model)
        {
            var permissions = model.Permissions.Where(a => !a.IsDeleted());
            return permissions.GroupBy(r => r.Name).Any(r => r.Count() > 1);
        }
    }
}
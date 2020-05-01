using System.Globalization;
using Microsoft.AspNetCore.Mvc.Razor;

namespace DNTFrameworkCore.Web.Mvc.Views
{
    public abstract class EntityFormRazorPage<T> : RazorPage<T>
    {
        protected string EntityName
        {
            get => ViewData[nameof(EntityName)].ToString();
            set => ViewData[nameof(EntityName)] = value;
        }

        protected string EntityDisplayName
        {
            get => ViewData[nameof(EntityDisplayName)].ToString();
            set => ViewData[nameof(EntityDisplayName)] = value;
        }

        protected string DeletePermission
        {
            get => ViewData[nameof(DeletePermission)].ToString();
            set => ViewData[nameof(DeletePermission)] = value;
        }

        protected string CreatePermission
        {
            get => ViewData[nameof(CreatePermission)].ToString();
            set => ViewData[nameof(CreatePermission)] = value;
        }

        protected string EditPermission
        {
            get => ViewData[nameof(EditPermission)].ToString();
            set => ViewData[nameof(EditPermission)] = value;
        }

        protected string CreateActionName
        {
            get => ViewData.TryGetValue(nameof(CreateActionName), out var value) ? value.ToString() : "Create";
            set => ViewData[nameof(CreateActionName)] = value;
        }

        protected string EditActionName
        {
            get => ViewData.TryGetValue(nameof(EditActionName), out var value) ? value.ToString() : "Edit";
            set => ViewData[nameof(EditActionName)] = value;
        }

        protected string DeleteActionName
        {
            get => ViewData.TryGetValue(nameof(DeleteActionName), out var value) ? value.ToString() : "Delete";
            set => ViewData[nameof(DeleteActionName)] = value;
        }

        protected string FormId => $"{EntityName}Form";
        protected bool IsNew => (Model as dynamic).IsNew();
        protected string Id => (Model as dynamic).Id.ToString(CultureInfo.InvariantCulture);
        protected byte[] Version => (Model as dynamic).Version;
    }
}
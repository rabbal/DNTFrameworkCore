using System;
using System.Threading.Tasks;
using DNTFrameworkCore.Application.Models;
using DNTFrameworkCore.TestWebApp.Application.Blogging;
using DNTFrameworkCore.TestWebApp.Application.Blogging.Models;
using DNTFrameworkCore.TestWebApp.Authorization;
using DNTFrameworkCore.Web.ActionSelectors;
using DNTFrameworkCore.Web.Authorization;
using DNTFrameworkCore.Web.Extensions;
using DNTFrameworkCore.Web.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DNTFrameworkCore.TestWebApp.Controllers
{
    public class BlogsController : CrudController<IBlogService, int, BlogModel>
    {
        public BlogsController(IBlogService service) : base(service)
        {
        }

        protected override string CreatePermissionName => PermissionNames.Administration_Blogs_Create;
        protected override string EditPermissionName => PermissionNames.Administration_Blogs_Edit;
        protected override string ViewPermissionName => PermissionNames.Administration_Blogs_View;
        protected override string DeletePermissionName => PermissionNames.Administration_Blogs_Delete;

        protected override string ViewName => "Blog";
    }
}
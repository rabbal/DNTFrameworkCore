using DNTFrameworkCore.TestAPI.Application.Tasks;
using DNTFrameworkCore.TestAPI.Application.Tasks.Models;
using DNTFrameworkCore.TestAPI.Authorization;
using DNTFrameworkCore.Web.API;
using Microsoft.AspNetCore.Mvc;

namespace DNTFrameworkCore.TestAPI.Controllers
{
    [Route("api/[controller]")]
    public class
        TasksController : EntityController<ITaskService, int, TaskReadModel, TaskModel, TaskFilteredPagedRequest>
    {
        public TasksController(ITaskService service) : base(service)
        {
        }

        protected override string CreatePermissionName => PermissionNames.Tasks_Create;
        protected override string EditPermissionName => PermissionNames.Tasks_Edit;
        protected override string ViewPermissionName => PermissionNames.Tasks_View;
        protected override string DeletePermissionName => PermissionNames.Tasks_Delete;

        [HttpGet("[action]")]
        public IActionResult TamperedFromOutside()
        {
            return Ok(EntityService.TamperedTaskExists());
        }
    }
}
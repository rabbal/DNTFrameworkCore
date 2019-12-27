namespace DNTFrameworkCore.TestTenancy.Controllers
{
    [Route("api/[controller]")]
    public class FrontEndController : ControllerBase
    {
        private readonly ITenantSession _tenantSession;

        public FrontEndController(ITenantSession tenantSession)
        {
            _tenantSession = tenantSession;
        }
        
        [HttpGet("[action]")]
        public IActionResult TenantInfo()
        {
            return Ok(_tenantSession);
        }
    }
}
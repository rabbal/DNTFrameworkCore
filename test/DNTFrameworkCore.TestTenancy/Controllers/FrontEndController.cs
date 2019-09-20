using DNTFrameworkCore.Tenancy;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DNTFrameworkCore.TestTenancy.Controllers
{
    [Route("api/[controller]")]
    public class FrontEndController : ControllerBase
    {
        private readonly ITenantSession _tenantSession;
        private readonly IOptions<CookiePolicyOptions> _cookieOptions;

        public FrontEndController(ITenantSession tenantSession,IOptions<CookiePolicyOptions> cookieOptions)
        {
            _tenantSession = tenantSession;
            _cookieOptions = cookieOptions;
        }
        
        [HttpGet("[action]")]
        public IActionResult TenantInfo()
        {
            return Ok(_tenantSession.Tenant);
        }
        
        [HttpGet("[action]")]
        public IActionResult CookieOption()
        {
            return Ok(_cookieOptions.Value);
        }
    }
}
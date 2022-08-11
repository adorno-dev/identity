using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Identity.Controllers
{
    [Route("api/roles")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly RoleManager<IdentityRole<Guid>> roleManager;

        public RoleController(RoleManager<IdentityRole<Guid>> roleManager)
        {
            this.roleManager = roleManager;
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<IEnumerable<IdentityRole<Guid>>>> GetRoles()
        {
            return await roleManager.Roles.AsNoTracking().ToListAsync();
        }
    }
}
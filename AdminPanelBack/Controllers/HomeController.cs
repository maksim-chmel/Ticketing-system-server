using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminPanelBack.Controllers;
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class HomeController : ControllerBase
{

}
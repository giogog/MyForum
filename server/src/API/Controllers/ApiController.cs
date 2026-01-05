using Contracts;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ApiController:ControllerBase
{
    private readonly IServiceManager _serviceManager;
    protected ApiResponse _response;
    

    public ApiController(IServiceManager serviceManager) => _serviceManager = serviceManager;
}

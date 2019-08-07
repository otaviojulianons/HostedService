using Microsoft.AspNetCore.Mvc;
using System;

namespace HostedService.Controllers
{
    [Route("/")]
    [ApiController]
    public class AboutController : ControllerBase
    {
        [HttpGet]
        public object Get()
        {
            return new
            {
                Name = "HostedService",
                Version = typeof(Startup).Assembly.GetName().Version.ToString(),
                Machine = Environment.MachineName
            };
        }
    }
}

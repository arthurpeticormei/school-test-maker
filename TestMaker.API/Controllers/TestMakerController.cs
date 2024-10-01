using Microsoft.AspNetCore.Mvc;
using TestMaker.API.DTOs;
using TestMaker.API.Models;
using TestMaker.API.Services;

namespace TestMaker.API.Controllers
{
    [ApiController]
    [Route("TestMaker")]
    public class TestMakerController : ControllerBase
    {
        [HttpPost()]
        public ActionResult TestMaker(Test test)
        {
            TestMakerService service = new();
            TestDTO testDTO = new TestDTO()
            {
                base64 = service.NewTest(test)
            };
            return Ok(testDTO);
        }
    }
}

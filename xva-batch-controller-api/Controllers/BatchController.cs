using Microsoft.AspNetCore.Mvc;

namespace xva_batch_controller_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        IBatchService _batchService;
        public BookController(IBatchService batchService)
        {
            _batchService = batchService;
        }

        [HttpPost]
        public async Task<IActionResult> Trigger([FromBody] BookModel model) {
            await _batchService.TriggerBookXVA(model);
            return Created();
        }

        [HttpDelete("{bookid}")]
        public async Task<IActionResult> Trigger([FromRoute]string bookid)
        {
            await _batchService.DeleteJobIfExists(bookid);
            return Created();
        }
    }
}

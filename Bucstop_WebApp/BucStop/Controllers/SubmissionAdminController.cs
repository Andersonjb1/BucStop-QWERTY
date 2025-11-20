using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace BucStop.Controllers
{
    [ApiController]
    [Route("admin/submissions")]
    public class SubmissionAdminController : Controller
    {
        [HttpDelete("{folderName}")]
        public IActionResult RejectSubmission(string folderName)
        {
            string fullPath = $"/app/Submissions/{folderName}";

            try
            {
                if (!Directory.Exists(fullPath))
                {
                    return NotFound(new { success = false, message = "Folder not found" });
                }

                Directory.Delete(fullPath, recursive: true);

                return Ok(new { success = true, folder = folderName });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}

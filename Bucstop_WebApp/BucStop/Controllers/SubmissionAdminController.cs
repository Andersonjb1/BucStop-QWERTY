using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;

namespace BucStop.Controllers
{
    [ApiController]
    [Route("admin/submissions")]
    public class SubmissionAdminController : Controller
    {
        private static readonly string SubmissionsBasePath = "/app/Submissions";

        [HttpDelete("{folderName}")]
        public IActionResult RejectSubmission(string folderName)
        {
            if (string.IsNullOrWhiteSpace(folderName))
            {
                return BadRequest(new { success = false, message = "Invalid folder name." });
            }

            if (folderName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0 ||
                folderName.Contains('/') ||
                folderName.Contains('\\') ||
                folderName.Contains(".."))
            {
                return BadRequest(new { success = false, message = "Invalid folder name." });
            }

            var basePath = Path.GetFullPath(SubmissionsBasePath);
            if (!basePath.EndsWith(Path.DirectorySeparatorChar))
            {
                basePath += Path.DirectorySeparatorChar;
            }

            var fullPath = Path.GetFullPath(Path.Combine(basePath, folderName));

            if (!fullPath.StartsWith(basePath, StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(new { success = false, message = "Invalid folder name." });
            }

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
                
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
            const string Root = "/app/Submissions";
            // Validate input
            if (string.IsNullOrWhiteSpace(folderName))
                return BadRequest(new { success = false, message = "Folder name is required." });
            // Sanitize folder name
            string safeFolderName = System.Text.RegularExpressions.Regex.Replace(
                folderName,
                @"[^A-Za-z0-9._-]",   // allowed characters
                "_"
            ).Replace("..", "_");       // prevent parent directory traversal

            if (safeFolderName.Length == 0)
                return BadRequest(new { success = false, message = "Invalid folder name." });
            // Combine with root path    
            string candidatePath = Path.Combine(Root, safeFolderName);

            // Normalize and enforce it stays under /app/Submissions
            string fullPath = Path.GetFullPath(candidatePath);

            string fullRoot = Path.GetFullPath(Root)
                .TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;

            if (!fullPath.StartsWith(fullRoot, StringComparison.Ordinal))
                return BadRequest(new { success = false, message = "Invalid folder path." });

            // Perform deletion
            try
            {
                if (!Directory.Exists(fullPath))
                {
                    return NotFound(new { success = false, message = "Folder not found" });
                }

                Directory.Delete(fullPath, recursive: true);

                return Ok(new { success = true, folder = safeFolderName });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}

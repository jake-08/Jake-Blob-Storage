using AzureBlobProject.Models;
using AzureBlobProject.Services;
using Microsoft.AspNetCore.Mvc;

namespace AzureBlobProject.Controllers
{
    public class BlobController : Controller
    {
        private readonly IBlobService _blobService;

        public BlobController(IBlobService blobService)
        {
            _blobService = blobService;
        }

        /// <summary>
        /// Get all the blobs under the given container name
        /// </summary>
        /// <param name="containerName"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Manage(string containerName)
        {
            var blobsObj = await _blobService.GetAllBlobs(containerName);

            return View(blobsObj);
        }

        [HttpGet]
        public IActionResult AddFile()
        {
            return View();
        }

        /// <summary>
        /// Add the given file to the given container
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddFile(string containerName, IFormFile file, Blob blob)
        {
            if (file == null || file.Length < 1) return View();

            var fileName = Path.GetFileNameWithoutExtension(blob.Title) + "_" + Guid.NewGuid() + Path.GetExtension(file.FileName);

            var result = await _blobService.UploadBlob(fileName, file, containerName, blob);

            if (result)
            {
                return RedirectToAction("Manage", "Blob", routeValues: new {containerName = containerName});
            }

            return View();
        }

        /// <summary>
        /// Redirect to Blob Absolute URI
        /// </summary>
        /// <param name="name"></param>
        /// <param name="containerName"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> ViewFile(string name, string containerName)
        {
            return Redirect(await _blobService.GetBlob(name, containerName));
        }

        /// <summary>
        /// Delete Blob with the given name and container
        /// </summary>
        /// <param name="name"></param>
        /// <param name="containerName"></param>
        /// <returns></returns>
        public async Task<IActionResult> DeleteFile(string name, string containerName)
        {
            await _blobService.DeleteBlob(name, containerName);

            return RedirectToAction("Manage", "Blob", routeValues: new { containerName = containerName });
        }
    }
}

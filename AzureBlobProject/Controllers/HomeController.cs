using Azure.Storage.Blobs;
using AzureBlobProject.Models;
using AzureBlobProject.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AzureBlobProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly IContainerService _containerService;
        private readonly IBlobService _blobService;

        public HomeController(IContainerService containerService, IBlobService blobService, BlobServiceClient blobServiceClient)
        {
            _containerService = containerService;
            _blobService = blobService;
            _blobServiceClient = blobServiceClient;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _containerService.GetAllContainerAndBlobs());
        }

        public async Task<IActionResult> Images()
        {
            try
            {
                List<Blob> blobList = await _blobService.GetAllBlobsWithUri("jakeblobstorage-abc");
                return View(blobList);
            }
            catch
            {
                return RedirectToAction("Create", "Container");
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
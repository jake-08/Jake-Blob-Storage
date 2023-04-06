using Azure.Storage.Blobs;
using AzureBlobProject.Models;
using AzureBlobProject.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AzureBlobProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly IContainerService _containerService;
        private readonly IBlobService _blobService;

        public HomeController(IContainerService containerService, IBlobService blobService)
        {
            _containerService = containerService;
            _blobService = blobService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                List<Blob> blobList = await _blobService.GetAllBlobsWithUri("jakeblobstorage-images");
                return View(blobList);
            }
            catch
            {
                return RedirectToAction("Create", "Container");
            }
            
        }

        public async Task<IActionResult> Hierarchy()
        {
            return View(await _containerService.GetAllContainerAndBlobs());
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
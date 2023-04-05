using AzureBlobProject.Models;
using AzureBlobProject.Services;
using Microsoft.AspNetCore.Mvc;

namespace AzureBlobProject.Controllers
{
    public class ContainerController : Controller
    {
        private readonly IContainerService _containerService;

        public ContainerController(IContainerService containerService)
        {
            _containerService = containerService;
        }

        /// <summary>
        /// Get all the containers and return
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {
            var allContainer = await _containerService.GetAllContainers();
            return View(allContainer);
        }

        public async Task<IActionResult> Create()
        {
            return View(new Container());
        }

        /// <summary>
        /// Create blob container
        /// </summary>
        /// <param name="container"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Create(Container container)
        {
            await _containerService.CreateContainer(container.Name.ToLower());
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Delete container given the container name
        /// </summary>
        /// <param name="containerName"></param>
        /// <returns></returns>
        public async Task<IActionResult> Delete(string containerName)
        {
            await _containerService.DeleteContainer(containerName);
            return RedirectToAction(nameof(Index));
        }
    }
}

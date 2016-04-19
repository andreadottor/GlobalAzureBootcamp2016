namespace Dottor.gab16pn.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Dottor.gab16pn.Services;
    using Dottor.gab16pn.Web.Extensions;
    using Dottor.gab16pn.Web.Models;
    using Microsoft.AspNet.Http;
    using Microsoft.AspNet.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Net.Http.Headers;

    public class HomeController : Controller
    {
        IStorageBlobService blob;
        IConfigurationRoot config;

        public HomeController(IStorageBlobService blobStorage, IConfigurationRoot config)
        {
            this.blob = blobStorage;
            this.config = config;
        }

        public async Task<IActionResult> Index()
        {
            var files = await blob.GetAllFilesAsync(config.Get<string>("blobContainerName"), "240x");

            IndexViewModel model = new IndexViewModel();
            model.Files = files.Select(f => f.Uri.ToString());

            return View(model);
        }

        [HttpGet]
        public IActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null)
                return RedirectToAction("Upload");

            try
            {
                var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');

                // validazione formato/estensione del file caricato
                //
                if (fileName.ToLower().EndsWith(".jpg") || fileName.ToLower().EndsWith(".png"))
                {
                    using (var stream = file.OpenReadStream())
                    {
                        await blob.UploadFromStreamAsync(config.Get<string>("blobContainerUploadName"), fileName, file.ContentType, ReadFully(stream));
                    }
                }
                else
                {
                    throw new Exception("Formato immagine non valido. Caricare un file .jpg o .png.");
                }
            }
            catch (Exception ex)
            {
                TempData.Put("error", new ErrorModel(ex));
                return RedirectToAction("UploadError");
            }
            return RedirectToAction("UploadSuccess");
        }

        public IActionResult UploadSuccess()
        {
            return View();
        }

        public IActionResult UploadError()
        {
            ErrorModel ex = TempData.Get<ErrorModel>("error");
            return View(ex);
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }

        private static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
    }
}

using EllipticCurve.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using projektdotnet.Models;
using projektdotnet.Services;

namespace projektdotnet.Controllers
{
    public class FileController : Controller
    {
        private readonly EmployeeService _employeeService;
        public FileController(EmployeeService employeeService)
        {
            _employeeService = employeeService;
        }
        [HttpGet]
        public IActionResult UploadFile()
        {
            return View();
        }
        [HttpPost]
        [Authorize(Roles = "NORMAL")]
        public async Task<IActionResult> UploadFile(IFormFile file, CancellationToken cancellationToken)
        {
            if (file == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var result = await WriteFile(file);
            return RedirectToAction("Index", "Home");
        }
        [Authorize(Roles = "NORMAL")]
        public async Task<string> WriteFile(IFormFile file)
        {
            string filename = "";
            try
            {
                var extenstion = "." + file.FileName.Split('.')[file.FileName.Split('.').Length - 1];
                if (extenstion == ".jpg")
                {
                    var employee = await _employeeService.GetEmployeeFromHttp();
                    filename = employee.Login + "profilepicture.jpg";
                    var filepath = "wwwroot\\Files";
                    if (!Directory.Exists(filepath))
                    {
                        Directory.CreateDirectory(filepath);
                    }
                    var exactpath = Path.Combine("wwwroot\\Files", filename);
                    using (var stream = new FileStream(exactpath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                }

            }
            catch (Exception ex)
            {
            }
            return filename;
        }
        [HttpGet]
        [Authorize(Roles = "NORMAL")]
        public async Task<IActionResult> DownloadFile(string filepath)
        {
            if (string.IsNullOrEmpty(filepath))
            {
                return BadRequest("File path cannot be null or empty");
            }

            // Ensure filepath does not start with a slash
            if (filepath.StartsWith("/"))
            {
                filepath = filepath.Substring(1);
            }

            // Map the file path to the physical location
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", filepath);

            if (!System.IO.File.Exists(fullPath))
            {
                return NotFound();
            }

            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(fullPath, out var contentType))
            {
                contentType = "application/octet-stream";
            }

            var fileBytes = await System.IO.File.ReadAllBytesAsync(fullPath);
            return File(fileBytes, contentType, Path.GetFileName(fullPath));
        }
    }
    }

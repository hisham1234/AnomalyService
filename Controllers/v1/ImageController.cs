using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AnomalyService.Data;
using AnomalyService.Helpers;
using AnomalyService.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace ImageService.Controllers
{
    [EnableCors("CorsApi")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly ApplicationDBContext _db;
        public ImageController(ApplicationDBContext db)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult GetAllImages()
        {

            var result = new
            {
                response = _db.Images.ToList()
            };
            return Ok(result);
        }

        [HttpPost, DisableRequestSizeLimit]
        public async Task<IActionResult> AddImage(IFormCollection data, IFormFile imageFile)
        {

            
            //Set Key Name
            string ImageName = DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss") + Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);

            Console.WriteLine(ImageName);
            //Get url To Save
            string SavePath = Path.Combine(Directory.GetCurrentDirectory(), ImageName);

            using (var stream = new FileStream(SavePath, FileMode.Create))
            //using (var stream = imageFile.OpenReadStream())

            {
                await imageFile.CopyToAsync(stream);
                await new AzureStorageHelper().UploadAsync(ImageName, SavePath, stream);

            }

            Image newImageData = new Image();
            newImageData.Name = ImageName;
            newImageData.Kp = data["kp"];
            newImageData.Road = data["road"];
            newImageData.Latitude = data["latitude"];
            newImageData.Longitude = data["longitude"];
            newImageData.TakenAt = string.IsNullOrWhiteSpace(data["takenAt"]) ? DateTime.Now : DateTime.ParseExact(data["takenAt"].ToString(), "yyyy-MM-dd HH:mm:ss", null);
            newImageData.CreatedAt = string.IsNullOrWhiteSpace(data["createdAt"]) ? DateTime.Now : DateTime.ParseExact(data["createdAt"].ToString(), "yyyy-MM-dd HH:mm:ss", null);
            newImageData.TakenAt = DateTime.Now;
            newImageData.CreatedAt = DateTime.Now;
            newImageData.UpdatedAt = DateTime.Now;


            _db.Images.Add(newImageData);
            await _db.SaveChangesAsync();
            return new JsonResult(new
            {
                response = newImageData
            })
            {
                StatusCode = 200
            };
        }




        [HttpPost("upload")]
        public async Task<IActionResult> UploadImage(IFormCollection data, IFormFile imageFile)
        {
            if (imageFile != null)
            {

                //Set Key Name
                string ImageName = DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss") + Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);

                //Get url To Save
                string SavePath = Path.Combine(Directory.GetCurrentDirectory(), ImageName);

                using (var stream = imageFile.OpenReadStream())

                {
                    await imageFile.CopyToAsync(stream);
                    await new AzureStorageHelper().UploadAsync(ImageName, SavePath, stream);
                }

            }

            return new JsonResult("Image Uploaded successfully");
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetImage([FromRoute] int id)
        {
            var findImage = await _db.Images.FindAsync(id);
            if (findImage == null)
            {
                return NotFound();
            }

            var result = new
            {
                response = findImage,
            };

            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateImage([FromRoute] int id, [FromBody] Image objImage)
        {
            if (objImage == null || id != objImage.Id)
            {
                return new JsonResult("Image was not Found");
            }
            else
            {
                _db.Images.Update(objImage);
                await _db.SaveChangesAsync();
                return new JsonResult("Image created Successfully");
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteImage([FromRoute] int id)
        {
            var findImage = await _db.Images.FindAsync(id);

            if (findImage == null)
            {
                return NotFound();
            }
            else
            {
                _db.Images.Remove(findImage);
                await _db.SaveChangesAsync();
                return new JsonResult("Image Deleted Successfully");
            }
        }
    }
}

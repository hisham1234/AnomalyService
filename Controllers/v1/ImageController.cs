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
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Net.Http.Headers;

namespace ImageService.Controllers
{
    [EnableCors("CorsApi")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly ApplicationDBContext _db;
        private LoggerHelper logHelp;
        public ImageController(ApplicationDBContext db)
        {
            _db = db;
            logHelp = new LoggerHelper();
        }

        [HttpGet]
        public IActionResult GetAllImages()
        {
            var methodName = "GetAllImages";
            logHelp.Log(logHelp.getMessage(methodName));
            try
            {
                var result = new
                {
                    response = _db.Images.ToList()
                };
                logHelp.Log(logHelp.getMessage(methodName, 200));
                return Ok(result);
            }
            catch (Exception ex)
            {

                logHelp.Log(logHelp.getMessage(methodName, 500));
                logHelp.Log(logHelp.getMessage(methodName, ex.Message));
                return StatusCode(500);
            }
            
        }

        
        [HttpPost, DisableRequestSizeLimit]
       
        public async Task<IActionResult> AddImage(IFormCollection data,IFormFile imageFile)
        {
            var methodName = "AddImage";
            logHelp.Log(logHelp.getMessage(methodName));

            try
            {
                var stream = new MemoryStream();
                imageFile.CopyTo(stream);
                //Set Key Name
                string ImageName = "pic_" + DateTime.Now.ToString("yyyy-MM-dd--HH-mm-ss") + Path.GetExtension(imageFile.FileName);

                Console.WriteLine(ImageName);
                //Get url To Save
                string SavePath = Path.Combine(Directory.GetCurrentDirectory(), ImageName);

               
                
                    //await imageFile.CopyToAsync(stream);
                    stream.Seek(0, SeekOrigin.Begin);
                    await new AzureStorageHelper().UploadAsync(ImageName, SavePath, stream);

                

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

                logHelp.Log(logHelp.getMessage(methodName, 200));
                return new JsonResult(new
                {
                    response = newImageData
                })
                {
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {

                logHelp.Log(logHelp.getMessage(methodName, 500));
                logHelp.Log(logHelp.getMessage(methodName, ex.Message));
                return StatusCode(500);
            }
            
        }




        [HttpPost("upload")]
        public async Task<IActionResult> UploadImage(IFormCollection data, IFormFile imageFile)
        {
            var methodName = "UploadImage";
            logHelp.Log(logHelp.getMessage(methodName));

            try
            {
                if (imageFile != null)
                {

                    //Set Key Name
                    string ImageName = DateTime.Now.ToString("yyyy-MM-dd--HH-mm-ss") + Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);

                    //Get url To Save
                    string SavePath = Path.Combine(Directory.GetCurrentDirectory(), ImageName);

                    using (var stream = imageFile.OpenReadStream())

                    {
                        await imageFile.CopyToAsync(stream);
                        await new AzureStorageHelper().UploadAsync(ImageName, SavePath, stream);
                    }
                    logHelp.Log(logHelp.getMessage(methodName, 200));
                    return new JsonResult("Image Uploaded successfully");
                }else
                {
                    logHelp.Log(logHelp.getMessage(methodName, 500));
                    logHelp.Log(logHelp.getMessage(methodName, "Image was empty"));
                    return new JsonResult("Image was empty");
                }

                
            }
            catch (Exception ex)
            {

                logHelp.Log(logHelp.getMessage(methodName, 500));
                logHelp.Log(logHelp.getMessage(methodName, ex.Message));
                return StatusCode(500);
            }
           
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetImage([FromRoute] int id)
        {
            var methodName = "GetImage";
            logHelp.Log(logHelp.getMessage(methodName));
            try
            {
                var findImage = await _db.Images.FindAsync(id);
                if (findImage == null)
                {
                    logHelp.Log(logHelp.getMessage(methodName, 500));
                    logHelp.Log(logHelp.getMessage(methodName, "Not Found"));
                    return NotFound();
                }

                var result = new
                {
                    response = findImage,
                };
                logHelp.Log(logHelp.getMessage(methodName, 200));
                return Ok(result);
            }
            catch (Exception ex)
            {

                logHelp.Log(logHelp.getMessage(methodName, 500));
                logHelp.Log(logHelp.getMessage(methodName, ex.Message));
                return StatusCode(500);
            }
           
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateImage([FromRoute] int id, [FromBody] Image objImage)
        {
            var methodName = "UpdateImage";
            logHelp.Log(logHelp.getMessage(methodName));
            try
            {
                if (objImage == null || id != objImage.Id)
                {
                    logHelp.Log(logHelp.getMessage(methodName, 500));
                    logHelp.Log(logHelp.getMessage(methodName, "Image was not found"));
                    return new JsonResult("Image was not Found");
                }
                else
                {
                    _db.Images.Update(objImage);
                    await _db.SaveChangesAsync();
                    logHelp.Log(logHelp.getMessage(methodName, 200));
                    return new JsonResult("Image updated Successfully");
                }
            }
            catch (Exception ex)
            {

                logHelp.Log(logHelp.getMessage(methodName, 500));
                logHelp.Log(logHelp.getMessage(methodName, ex.Message));
                return StatusCode(500);
            }
            
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteImage([FromRoute] int id)
        {
            var methodName = "DeleteImage";
            logHelp.Log(logHelp.getMessage(methodName));

            try
            {
                var findImage = await _db.Images.FindAsync(id);

                if (findImage == null)
                {
                    logHelp.Log(logHelp.getMessage(methodName, 500));
                    logHelp.Log(logHelp.getMessage(methodName, "Image was not found"));
                    return NotFound();
                }
                else
                {
                    _db.Images.Remove(findImage);
                    await _db.SaveChangesAsync();
                    logHelp.Log(logHelp.getMessage(methodName, 200));
                    return new JsonResult("Image Deleted Successfully");
                }
            }
            catch (Exception ex)
            {

                logHelp.Log(logHelp.getMessage(methodName, 500));
                logHelp.Log(logHelp.getMessage(methodName, ex.Message));
                return StatusCode(500);
            }
           
        }
    }


    
}

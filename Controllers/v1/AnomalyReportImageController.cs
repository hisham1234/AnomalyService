using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AnomalyService.Data;
using AnomalyService.Helpers;
using AnomalyService.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace AnomalyService.Controllers
{
    [EnableCors("CorsApi")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class AnomalyReportImageController : ControllerBase
    {
        private readonly ApplicationDBContext _db;
        private LoggerHelper logHelp;
        public AnomalyReportImageController(ApplicationDBContext db)
        {
            _db = db;
            logHelp = new LoggerHelper();
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAnomalyReportImagesAsync()
        {
            var methodName = "GetAllAnomalyReportImagesAsync";
            logHelp.Log(logHelp.getMessage(methodName));
            try
            {
                logHelp.Log(logHelp.getMessage(methodName, 200));
                return Ok(await _db.AnomalyReportImages.Include(r => r.Image).ToListAsync());
            }
            catch (Exception ex)
            {

                logHelp.Log(logHelp.getMessage(methodName, 500));
                logHelp.Log(logHelp.getMessage(methodName, ex.Message));
                return StatusCode(500);
            }
            
        }


        [HttpPost]
        public async Task<IActionResult> AddAnomalyReportImage([FromBody] AnomalyReportImage objAnomalyReportImage)
        {
            var methodName = "AddAnomalyReportImage";
            logHelp.Log(logHelp.getMessage(methodName));
            try
            {
                if (!ModelState.IsValid)
                {
                    logHelp.Log(logHelp.getMessage(methodName, 500));
                    logHelp.Log(logHelp.getMessage(methodName, "Error while creating new Anomaly Report"));
                    return new JsonResult("Error while creating new Anomaly Report Image");
                }

                _db.AnomalyReportImages.Add(objAnomalyReportImage);
                await _db.SaveChangesAsync();
                logHelp.Log(logHelp.getMessage(methodName, 200));
                return new JsonResult("Anomaly created successfully");
            }
            catch (Exception ex)
            {

                logHelp.Log(logHelp.getMessage(methodName, 500));
                logHelp.Log(logHelp.getMessage(methodName, ex.Message));
                return StatusCode(500);
            }
           
        }


        [HttpPost("with-image-ids")]
        public async Task<IActionResult> AddAnomalyReportImageWithImageIds(JObject s)
        {
            var methodName = "AddAnomalyReportImageWithImageIds";
            logHelp.Log(logHelp.getMessage(methodName));


            int anomalyReportId = (int)s["anomalyReportId"];
            var imageIds = s["imageIds"];


            using var transaction = _db.Database.BeginTransaction();

            try
            {
                _db.AnomalyReportImages.RemoveRange(_db.AnomalyReportImages.Where(anomalyRImage => anomalyRImage.AnomalyReportId == anomalyReportId));

                foreach (int imageId in imageIds)
                {
                    AnomalyReportImage newAnomalyReportImage = new AnomalyReportImage();
                    newAnomalyReportImage.AnomalyReportId = anomalyReportId;
                    newAnomalyReportImage.ImageId = imageId;
                    newAnomalyReportImage.CreatedAt = DateTime.Now;
                    newAnomalyReportImage.UpdatedAt = DateTime.Now;
                    _db.AnomalyReportImages.Add(newAnomalyReportImage);
                }

                await _db.SaveChangesAsync();
                // Commit transaction if all commands succeed, transaction will auto-rollback
                // when disposed if either commands fails
                transaction.Commit();
                logHelp.Log(logHelp.getMessage(methodName, 200));
                return Ok("Okay");
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                logHelp.Log(logHelp.getMessage(methodName, 500));
                logHelp.Log(logHelp.getMessage(methodName, ex.Message));
                return StatusCode(500);
            }
                
        }


        [HttpPost("{anomalyReportId}/image-id/{imageId}")]
        public async Task<IActionResult> AddAnomalyReportImageWithImageIds([FromRoute] int anomalyReportId, int imageId)
        {
            var methodName = "AddAnomalyReportImageWithImageIds";
            logHelp.Log(logHelp.getMessage(methodName));

            using var transaction = _db.Database.BeginTransaction();

            try
            {

                
                    AnomalyReportImage newAnomalyReportImage = new AnomalyReportImage();
                    newAnomalyReportImage.AnomalyReportId = anomalyReportId;
                    newAnomalyReportImage.ImageId = imageId;
                    newAnomalyReportImage.CreatedAt = DateTime.Now;
                    newAnomalyReportImage.UpdatedAt = DateTime.Now;
                    _db.AnomalyReportImages.Add(newAnomalyReportImage);
               

                await _db.SaveChangesAsync();
                // Commit transaction if all commands succeed, transaction will auto-rollback
                // when disposed if either commands fails
                transaction.Commit();
                logHelp.Log(logHelp.getMessage(methodName, 200));
                return Ok("Okay");
            }
            catch (Exception ex)
            {
                transaction.Rollback();
              
                logHelp.Log(logHelp.getMessage(methodName, 500));
                logHelp.Log(logHelp.getMessage(methodName, ex.Message));
                return StatusCode(500);
            }
            
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAnomalyReportImage([FromRoute] int id, [FromBody] AnomalyReportImage objAnomalyReportImage)
        {
            var methodName = "UpdateAnomalyReportImage";
            logHelp.Log(logHelp.getMessage(methodName));
            try
            {
                if (objAnomalyReportImage == null || id != objAnomalyReportImage.Id)
                {
                    logHelp.Log(logHelp.getMessage(methodName, 500));
                    logHelp.Log(logHelp.getMessage(methodName, "Image was not Found"));
                    return new JsonResult("Image was not Found");
                }
                else
                {
                    var changedObjAnomalyReportImage = _db.AnomalyReportImages.Update(objAnomalyReportImage);
                    await _db.SaveChangesAsync();
                    logHelp.Log(logHelp.getMessage(methodName, 200));
                    return new JsonResult("Image Updated Successfully");
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
        public async Task<IActionResult> DeleteAnomalyReportImages([FromRoute] int id)
        {
            var methodName = "DeleteAnomalyReportImages";
            logHelp.Log(logHelp.getMessage(methodName));

            try
            {
                var findAnomalyReportImage = await _db.AnomalyReportImages.FindAsync(id);

                if (findAnomalyReportImage == null)
                {
                    logHelp.Log(logHelp.getMessage(methodName, 404));
                    logHelp.Log(logHelp.getMessage(methodName, "NotFound"));
                    return NotFound();
                }
                else
                {
                    _db.AnomalyReportImages.Remove(findAnomalyReportImage);
                    await _db.SaveChangesAsync();
                    logHelp.Log(logHelp.getMessage(methodName, 200));
                    return new JsonResult("Anomaly Deleted Successfully");

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

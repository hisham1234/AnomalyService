using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AnomalyService.Data;
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
        public AnomalyReportImageController(ApplicationDBContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAnomalyReportImagesAsync()
        {

            return Ok(await _db.AnomalyReportImages.Include(r => r.Image).ToListAsync());
        }


        [HttpPost]
        public async Task<IActionResult> AddAnomalyReportImage([FromBody] AnomalyReportImage objAnomalyReportImage)
        {
            if (!ModelState.IsValid)
            {
                return new JsonResult("Error while creating new Anomaly Report");
            }

            _db.AnomalyReportImages.Add(objAnomalyReportImage);
            await _db.SaveChangesAsync();

            return new JsonResult("Anomaly created successfully");
        }


        [HttpPost("with-image-ids")]
        public async Task<IActionResult> AddAnomalyReportImageWithImageIds(JObject s)
        {
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
            }
            catch (Exception)
            {
                transaction.Rollback();
            }
                return Ok("Okay");
        }


        [HttpPost("{anomalyReportId}/image-id/{imageId}")]
        public async Task<IActionResult> AddAnomalyReportImageWithImageIds([FromRoute] int anomalyReportId, int imageId)
        {
           
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
            }
            catch (Exception)
            {
                transaction.Rollback();
            }
            return Ok("Okay");
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAnomalyReportImage([FromRoute] int id, [FromBody] AnomalyReportImage objAnomalyReportImage)
        {
            if (objAnomalyReportImage == null || id != objAnomalyReportImage.Id)
            {
                return new JsonResult("Anomaly was not Found");
            }
            else
            {
                var changedObjAnomalyReportImage = _db.AnomalyReportImages.Update(objAnomalyReportImage);
                await _db.SaveChangesAsync();
                return new JsonResult("Anomaly created Successfully");
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAnomalyReportImages([FromRoute] int id)
        {
            var findAnomalyReportImage = await _db.AnomalyReportImages.FindAsync(id);

            if (findAnomalyReportImage == null)
            {
                return NotFound();
            }
            else
            {
                _db.AnomalyReportImages.Remove(findAnomalyReportImage);
                await _db.SaveChangesAsync();
                return new JsonResult("Anomaly Deleted Successfully");

            }
        }
    }
}

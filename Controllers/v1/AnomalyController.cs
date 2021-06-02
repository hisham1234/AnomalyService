﻿using System;
using System.Linq;
using System.Threading.Tasks;
using AnomalyService.Data;
using AnomalyService.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace AnomalyService.Controllers
{
    [EnableCors("CorsApi")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class AnomalyController : ControllerBase
    {
        private readonly ApplicationDBContext _db;
        public AnomalyController(ApplicationDBContext db)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult GetAllAnomaly()
        {
            var result =new
            {
                response = _db.Anomalys.ToList(),
                totalCount = _db.Anomalys.ToList().ToArray().Length
            };
            
            return Ok(result);
        }

       
        [HttpPost]
        public async Task<IActionResult> AddAnomaly ([FromBody] Anomaly objAnomaly)
        {
            if (!ModelState.IsValid)
            {
                return new JsonResult("Error while creating new Anomaly  ");
            }

            _db.Anomalys.Add(objAnomaly);
            await _db.SaveChangesAsync();

            return new JsonResult("Anomaly created successfully");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAnomaly ([FromRoute] int id, [FromBody] Anomaly objAnomaly)
        {
            
            if (objAnomaly == null || id != objAnomaly.Id)
            {
                return new JsonResult("Anomaly was not Found");
            }
            else
            {
                _db.Anomalys.Update(objAnomaly);
                await _db.SaveChangesAsync();
                return new JsonResult("Anomaly created Successfully");
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAnomaly([FromRoute] int id)
        {
            var findAnomaly = await _db.Anomalys.FindAsync(id);

            if (findAnomaly == null)
            {
                return NotFound();
            }
            else
            {
                _db.Anomalys.Remove(findAnomaly);
                await _db.SaveChangesAsync();
                return new JsonResult("Anomaly Deleted Successfully");

            }
        }
    }
}

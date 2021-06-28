﻿using System;
using System.Linq;
using System.Threading.Tasks;
using AnomalyService.Data;
using AnomalyService.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using System.Text;
using AnomalyService.Helpers;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;

namespace AnomalyService.Controllers
{
    [EnableCors("CorsApi")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class AnomalyController : ControllerBase
    {
        private readonly ApplicationDBContext _db;
        private RabbitMQHelper rbbit = new RabbitMQHelper();
        private AnomalyHelper anm;
        public AnomalyController(ApplicationDBContext db)
        {
            _db = db;
            anm = new AnomalyHelper(db);
           
        }

        [HttpGet]
        public  IActionResult GetAllAnomaly()
        {
            var result =new
            {
                response = _db.Anomalys.ToList(),
                totalCount = _db.Anomalys.ToList().ToArray().Length
            };


            
            
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAnomalyById([FromRoute] int id)
        {

            var findAnomaly = await _db.Anomalys.FindAsync(id);
            if(findAnomaly == null)
            {
                return NotFound();
            }

            var result = new
            {
                response = findAnomaly,
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
           
           
            var jsonifiedAnomaly = JsonConvert.SerializeObject(objAnomaly);
            rbbit.SendMessage(jsonifiedAnomaly, "Anomaly.Created");
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
                anm.UpdateAnomalyLatLon(id);
                var updatedAnomaly = _db.Anomalys.Include(c=>c.AnomelyReport).ToList().FirstOrDefault(x => x.Id == id);

                var jsonifiedAnomaly = JsonConvert.SerializeObject(updatedAnomaly);
                rbbit.SendMessage(jsonifiedAnomaly, "Anomaly.Updated");
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
               
                var updatedAnomaly = _db.Anomalys.Include(c => c.AnomelyReport).ToList().FirstOrDefault(x => x.Id == id);

                var jsonifiedAnomaly = JsonConvert.SerializeObject(updatedAnomaly);
                rbbit.SendMessage(jsonifiedAnomaly, "Anomaly.Deleted");

                _db.Anomalys.Remove(findAnomaly);
                await _db.SaveChangesAsync();
                
                return new JsonResult("Anomaly Deleted Successfully");

            }
        }
    }
}

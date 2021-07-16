using System;
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
using System.Diagnostics;
using System.Reflection;
using Microsoft.Extensions.Logging;

namespace AnomalyService.Controllers
{
    [EnableCors("CorsApi")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class AnomalyController : ControllerBase
    {

        private readonly ILogger _logger;
        private readonly ApplicationDBContext _db;
        private RabbitMQHelper rbbit = new RabbitMQHelper();
        private AnomalyHelper anm;
        private LoggerHelper logHelp;

        public AnomalyController(ApplicationDBContext db, ILogger<AnomalyController> logger)
        {
            _logger = logger;
            _db = db;
            anm = new AnomalyHelper(db);
            logHelp = new LoggerHelper();
        }

        [HttpGet]
        public  IActionResult GetAllAnomaly()
        {
            logHelp.Log(logHelp.getMessage("GetAllAnomaly"));
            _logger.LogInformation(logHelp.getMessage("GetAllAnomaly"));
            try
            {
                var result = new
                {
                    response = _db.Anomalys.ToList(),
                    totalCount = _db.Anomalys.ToList().ToArray().Length
                };
                logHelp.Log(logHelp.getMessage("GetAllAnomaly", 200));
                _logger.LogInformation(logHelp.getMessage("GetAllAnomaly", 200));
                return Ok(result);
            }
            catch (Exception ex)
            {
                logHelp.Log(logHelp.getMessage("GetAllAnomaly", 500));
                logHelp.Log(logHelp.getMessage("GetAllAnomaly", ex.Message));
                _logger.LogError(logHelp.getMessage("GetAllAnomaly", 500));
                _logger.LogError(logHelp.getMessage("GetAllAnomaly", ex.Message));
                return StatusCode(500);
            }
            

            
            
           
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAnomalyById([FromRoute] int id)
        {
            logHelp.Log(logHelp.getMessage("GetAnomalyById"));
            _logger.LogInformation(logHelp.getMessage("GetAnomalyById"));

            try
            {
                var findAnomaly = await _db.Anomalys.FindAsync(id);
                if (findAnomaly == null)
                {
                    return NotFound();
                }

                var result = new
                {
                    response = findAnomaly,
                };
                logHelp.Log(logHelp.getMessage("GetAnomalyById",200));
                _logger.LogInformation(logHelp.getMessage("GetAnomalyById", 200));

                return Ok(result);
            }
            catch (Exception ex)
            {
                logHelp.Log(logHelp.getMessage("GetAnomalyById",500));
                logHelp.Log(logHelp.getMessage("GetAnomalyById",ex.Message));
                _logger.LogInformation(logHelp.getMessage("GetAnomalyById"));
                _logger.LogInformation(logHelp.getMessage("GetAnomalyById", ex.Message));

                return StatusCode(500);
            }

           
        }


        [HttpPost]
        public async Task<IActionResult> AddAnomaly ([FromBody] Anomaly objAnomaly)
        {
            logHelp.Log(logHelp.getMessage("AddAnomaly"));
            _logger.LogInformation(logHelp.getMessage("AddAnomaly"));

            try
            {
                if (!ModelState.IsValid)
                {
                    logHelp.Log(logHelp.getMessage("AddAnomaly", 500));
                    logHelp.Log(logHelp.getMessage("Error while creating new Anomaly"));
                    _logger.LogError(logHelp.getMessage("AddAnomaly", 500));
                    _logger.LogError(logHelp.getMessage("Error while creating new Anomaly"));

                    return new JsonResult("Error while creating new Anomaly  ");
                }

                _db.Anomalys.Add(objAnomaly);
                await _db.SaveChangesAsync();

                logHelp.Log(logHelp.getMessage("AddAnomaly",200));
                _logger.LogInformation(logHelp.getMessage("AddAnomaly", 200));

                var jsonifiedAnomaly = JsonConvert.SerializeObject(objAnomaly);
                rbbit.SendMessage(jsonifiedAnomaly, "Anomaly.Created");
                return  Ok();
            }
            catch (Exception ex)
            {
                logHelp.Log(logHelp.getMessage("AddAnomaly", 500));
                logHelp.Log(logHelp.getMessage("AddAnomaly",ex.Message));
                _logger.LogError(logHelp.getMessage("AddAnomaly", 500));
                _logger.LogError(logHelp.getMessage("Error while creating new Anomaly"));
                return StatusCode(500);
            }
            
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAnomaly ([FromRoute] int id, [FromBody] Anomaly objAnomaly)
        {
            logHelp.Log(logHelp.getMessage("UpdateAnomaly"));
            _logger.LogInformation(logHelp.getMessage("UpdateAnomaly"));
            try
            {
                if (objAnomaly == null || id != objAnomaly.Id)
                {
                    logHelp.Log(logHelp.getMessage("UpdateAnomaly", 500));
                    logHelp.Log(logHelp.getMessage("UpdateAnomaly","Anomaly was not Found"));
                    _logger.LogError(logHelp.getMessage("UpdateAnomaly", 500));
                    _logger.LogError(logHelp.getMessage("UpdateAnomaly", "Anomaly was not Found"));

                    return new JsonResult("Anomaly was not Found");
                }
                else
                {
                    _db.Anomalys.Update(objAnomaly);
                    await _db.SaveChangesAsync();
                    anm.UpdateAnomalyLatLon(id);
                    var updatedAnomaly = _db.Anomalys.Include(c => c.Reports).ToList().FirstOrDefault(x => x.Id == id);

                    logHelp.Log(logHelp.getMessage("UpdateAnomaly",200));
                    _logger.LogInformation(logHelp.getMessage("UpdateAnomaly", 200));

                    var jsonifiedAnomaly = JsonConvert.SerializeObject(updatedAnomaly);
                    rbbit.SendMessage(jsonifiedAnomaly, "Anomaly.Updated");
                    return new JsonResult("Anomaly Updated Successfully");
                }
            }
            catch (Exception ex)
            {
                logHelp.Log(logHelp.getMessage("UpdateAnomaly",500));
                logHelp.Log(logHelp.getMessage("UpdateAnomaly",ex.Message));
                _logger.LogError(logHelp.getMessage("UpdateAnomaly", 500));
                _logger.LogError(logHelp.getMessage("UpdateAnomaly", ex.Message));
                return StatusCode(500);
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAnomaly([FromRoute] int id)
        {
            logHelp.Log(logHelp.getMessage("DeleteAnomaly"));
            _logger.LogInformation(logHelp.getMessage("DeleteAnoamly"));

            try
            {
                var findAnomaly = await _db.Anomalys.FindAsync(id);

                if (findAnomaly == null)
                {
                    logHelp.Log(logHelp.getMessage("DeleteAnomaly", 500));
                    logHelp.Log(logHelp.getMessage("DeleteAnomaly", "Anomaly was not Found"));
                    _logger.LogError(logHelp.getMessage("DeleteAnomaly", 500));
                    _logger.LogError(logHelp.getMessage("DeleteAnomaly", "Anomaly was not Found"));
                    return NotFound();
                }
                else
                {

                    var updatedAnomaly = _db.Anomalys.Include(c => c.Reports).ToList().FirstOrDefault(x => x.Id == id);

                    var jsonifiedAnomaly = JsonConvert.SerializeObject(updatedAnomaly);
                    rbbit.SendMessage(jsonifiedAnomaly, "Anomaly.Deleted");

                    _db.Anomalys.Remove(findAnomaly);
                    await _db.SaveChangesAsync();
                    logHelp.Log(logHelp.getMessage("DeleteAnomaly", 200));
                    _logger.LogInformation(logHelp.getMessage("DeleteAnomaly", 200));

                    return new JsonResult("Anomaly Deleted Successfully");

                }
            }
            catch (Exception ex)
            {

                logHelp.Log(logHelp.getMessage("DeleteAnomaly",500));
                logHelp.Log(logHelp.getMessage("DeleteAnomaly",ex.Message));
                _logger.LogError(logHelp.getMessage("DeleteAnomaly", 500));
                _logger.LogError(logHelp.getMessage("DeleteAnomaly", ex.Message));
                return StatusCode(500);
            }
           
        }
    }
}

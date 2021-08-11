using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AnomalyService.Data;
using AnomalyService.Helpers;
using AnomalyService.Models;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AnomalyService.Controllers
{
    [EnableCors("CorsApi")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class AnomalyReportController : ControllerBase
    {

        private readonly TelemetryClient _telemetry;
        private readonly ILogger _logger;
        private readonly ApplicationDBContext _db;
        private RabbitMQHelper rbbit = new RabbitMQHelper();
        private AnomalyHelper anm;
        private LoggerHelper logHelp;
        public AnomalyReportController(ApplicationDBContext db, ILogger<AnomalyReportController> logger, TelemetryClient telemetry)
        {
            _telemetry = telemetry; // -> used by _logger according microsfot doc
            _logger = logger;
            _db = db;
            anm = new AnomalyHelper(db);
            logHelp = new LoggerHelper();
        }

        [HttpGet]
        public IActionResult GetAllAnomalyReports()
        {
            logHelp.Log(logHelp.getMessage("GetAllAnomalyReports"));
            _logger.LogInformation(logHelp.getMessage("GetAllAnomalyReports"));
            try
            {
                var result = new
                {
                    response = _db.AnomalyReports.ToList(),
                    totalCount = _db.AnomalyReports.ToList().ToArray().Length
                };
                logHelp.Log(logHelp.getMessage("GetAllAnomalyReports", 200));
                _logger.LogInformation(logHelp.getMessage("GetAllAnomalyReports", 200));

                return Ok(result);
            }
            catch (Exception ex)
            {

                logHelp.Log(logHelp.getMessage("GetAllAnomalyReports", 500));
                logHelp.Log(logHelp.getMessage("GetAllAnomalyReports", ex.Message));
                _logger.LogError(logHelp.getMessage("GetAllAnomalyReports", 500));
                _logger.LogError(logHelp.getMessage("GetAllAnomalyReports", ex.Message));

                return StatusCode(500);
            }
           
        }


        [HttpGet("Anomaly/{anomalyId}")]
        public IActionResult GetAnomalyReportsForAnomalyId([FromRoute] int anomalyId)
        {
            logHelp.Log(logHelp.getMessage("GetAnomalyReportsForAnomalyId"));
            _logger.LogInformation(logHelp.getMessage("GetAnomalyReportsForAnomalyId"));

            try
            {
                var query = _db.AnomalyReports.Where(r => r.AnomalyId == anomalyId)
                                            .Include(r => r.AnomalyReportImage)
                                            .ThenInclude(aReportImage => aReportImage.Image);
                var result = new
                {
                    response = query.ToList(),
                    totalCount = query.ToArray().Length
                };
                logHelp.Log(logHelp.getMessage("GetAnomalyReportsForAnomalyId", 200));
                _logger.LogInformation(logHelp.getMessage("GetAnomalyReportsForAnomalyId", 200));

                return Ok(result);
            }
            catch (Exception ex)
            {

                logHelp.Log(logHelp.getMessage("GetAnomalyReportsForAnomalyId", 500));
                logHelp.Log(logHelp.getMessage("GetAnomalyReportsForAnomalyId", ex.Message));
                _logger.LogError(logHelp.getMessage("GetAnomalyReportsForAnomalyId", 500));
                _logger.LogError(logHelp.getMessage("GetAnomalyReportsForAnomalyId", ex.Message));

                return StatusCode(500);
            }
            
        }


        [HttpGet("{id}")]
        public IActionResult GetAnomalyReportById([FromRoute] int id)
        {
            var methodName = "GetAnomalyReportById";
            logHelp.Log(logHelp.getMessage(methodName));
            _logger.LogInformation(logHelp.getMessage("GetAnomalyReportById"));
            try
            {
                var result = new
                {
                    response = _db.AnomalyReports.Where(r => r.Id == id)
                                    .Include(r => r.AnomalyReportImage)
                                    .ThenInclude(rImage => rImage.Image)
                                    .FirstOrDefault()
                };
                logHelp.Log(logHelp.getMessage(methodName, 200));
                _logger.LogInformation(logHelp.getMessage(methodName, 200));

                return Ok(result);
            }
            catch (Exception ex)
            {

                logHelp.Log(logHelp.getMessage(methodName, 500));
                logHelp.Log(logHelp.getMessage(methodName, ex.Message));
                _logger.LogError(logHelp.getMessage(methodName, 500));
                _logger.LogError(logHelp.getMessage(methodName, ex.Message));
                return StatusCode(500);
            }
            
        }

        [HttpPost]
        public async Task<IActionResult> AddAnomalyReport([FromBody] AnomalyReport objAnomalyReport)
        {
            var methodName = "AddAnomalyReport";
            logHelp.Log(logHelp.getMessage(methodName));
            _logger.LogInformation(logHelp.getMessage(methodName));

            if (!ModelState.IsValid)
            {
                logHelp.Log(logHelp.getMessage(methodName, 500));
                logHelp.Log(logHelp.getMessage(methodName, "Not Valid"));
                _logger.LogError(logHelp.getMessage(methodName, 500));
                _logger.LogError(logHelp.getMessage(methodName, "Not Valid"));

                return new JsonResult(new
                {
                    response = "Not Valid",
                    statusCode = 500
                })
                {
                    StatusCode = 500
                };
            }
            try
            {
                var savedObjAnomalyReport = _db.AnomalyReports.Add(objAnomalyReport);
                await _db.SaveChangesAsync();

                anm.UpdateAnomalyLatLon(objAnomalyReport.AnomalyId);
                var updatedAnomaly = _db.Anomalys.Include(c => c.Reports).ToList().FirstOrDefault(x => x.Id == objAnomalyReport.AnomalyId);

                var jsonifiedAnomaly = JsonConvert.SerializeObject(updatedAnomaly);
                rbbit.SendMessage(jsonifiedAnomaly, "Report.Created");
                logHelp.Log(logHelp.getMessage(methodName, 200));
                _logger.LogInformation(logHelp.getMessage(methodName, 200));

                return new JsonResult(new
                {
                    response = objAnomalyReport,
                    statusCode = 200
                })
                {
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {

                logHelp.Log(logHelp.getMessage(methodName, 500));
                logHelp.Log(logHelp.getMessage(methodName, ex.Message));
                _logger.LogError(logHelp.getMessage(methodName, 500));
                _logger.LogError(logHelp.getMessage(methodName, ex.Message));
                return StatusCode(500);
            }
           
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAnomalyReport([FromRoute] int id, [FromBody] AnomalyReport objAnomalyReport)
        {
            var methodName = "UpdateAnomalyReport";
            logHelp.Log(logHelp.getMessage(methodName));
            _logger.LogInformation(logHelp.getMessage(methodName));


            try
            {
                if (objAnomalyReport == null || id != objAnomalyReport.Id)
                {
                    logHelp.Log(logHelp.getMessage(methodName, 500));
                    logHelp.Log(logHelp.getMessage(methodName, "Not Found"));
                    _logger.LogError(logHelp.getMessage(methodName, 500));
                    _logger.LogError(logHelp.getMessage(methodName, "Not found"));

                    return new JsonResult(new
                    {
                        response = "Not Found",
                        statusCode = 500
                    })
                    {
                        StatusCode = 500
                    };
                }
                else
                {
                    var changedObjAnomalyReport = _db.AnomalyReports.Update(objAnomalyReport);
                    await _db.SaveChangesAsync();

                    anm.UpdateAnomalyLatLon(objAnomalyReport.AnomalyId);
                    var updatedAnomaly = _db.Anomalys.Include(c => c.Reports).ToList().FirstOrDefault(x => x.Id == objAnomalyReport.AnomalyId);

                    var jsonifiedAnomaly = JsonConvert.SerializeObject(updatedAnomaly);
                    rbbit.SendMessage(jsonifiedAnomaly, "Report.Updated");

                    logHelp.Log(logHelp.getMessage(methodName, 200));
                    _logger.LogInformation(logHelp.getMessage(methodName, 200));


                    return new JsonResult(new
                    {
                        response = objAnomalyReport,
                        statusCode = 200
                    })
                    {
                        StatusCode = 200
                    };
                }
            }
            catch (Exception ex)
            {

                logHelp.Log(logHelp.getMessage(methodName, 500));
                logHelp.Log(logHelp.getMessage(methodName, ex.Message));
                _logger.LogError(logHelp.getMessage(methodName, 500));
                _logger.LogError(logHelp.getMessage(methodName, ex.Message));


                return StatusCode(500);
            }
            
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAnomalyReport([FromRoute] int id)
        {
            var methodName = "DeleteAnomalyReport";
            logHelp.Log(logHelp.getMessage(methodName));
            _logger.LogInformation(logHelp.getMessage(methodName));

            try
            {
                var findAnomalyReport = await _db.AnomalyReports.FindAsync(id);

                if (findAnomalyReport == null)
                {
                    logHelp.Log(logHelp.getMessage(methodName, 500));
                    logHelp.Log(logHelp.getMessage(methodName, "Not Found"));
                    _logger.LogError(logHelp.getMessage(methodName, 500));
                    _logger.LogError(logHelp.getMessage(methodName, "Not found"));
                    return NotFound();
                }
                else
                {

                    var updatedAnomaly = _db.Anomalys.Include(c => c.Reports).ToList().FirstOrDefault(x => x.Id == findAnomalyReport.AnomalyId);

                    _db.AnomalyReports.Remove(findAnomalyReport);
                    await _db.SaveChangesAsync();
                    anm.UpdateAnomalyLatLon(findAnomalyReport.AnomalyId);

                    var jsonifiedAnomaly = JsonConvert.SerializeObject(updatedAnomaly);
                    rbbit.SendMessage(jsonifiedAnomaly, "Report.Deleted");
                    logHelp.Log(logHelp.getMessage(methodName, 200));
                    _logger.LogInformation(logHelp.getMessage(methodName, 200));

                    return new JsonResult("Anomaly Deleted Successfully");

                }
            }
            catch (Exception ex)
            {

                logHelp.Log(logHelp.getMessage(methodName, 500));
                logHelp.Log(logHelp.getMessage(methodName, ex.Message));
                _logger.LogError(logHelp.getMessage(methodName, 500));
                _logger.LogError(logHelp.getMessage(methodName, ex.Message));

                return StatusCode(500);
            }
           
        }
    }
}

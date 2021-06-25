﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AnomalyService.Data;
using AnomalyService.Helpers;
using AnomalyService.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AnomalyService.Controllers
{
    [EnableCors("CorsApi")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class AnomalyReportController : ControllerBase
    {
        private readonly ApplicationDBContext _db;
        public AnomalyReportController(ApplicationDBContext db)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult GetAllAnomalyReports()
        {
            var result = new
            {
                response = _db.AnomalyReports.ToList(),
                totalCount = _db.AnomalyReports.ToList().ToArray().Length
            };

            return Ok(result);
        }


        [HttpGet("Anomaly/{anomalyId}")]
        public IActionResult GetAnomalyReportsForAnomalyId([FromRoute] int anomalyId)
        {
            var query = _db.AnomalyReports.Where(r => r.AnomalyId == anomalyId)
                                            .Include(r => r.AnomelyReportImage)
                                            .ThenInclude(aReportImage => aReportImage.Image);
            var result = new
            {
                response = query.ToList(),
                totalCount = query.ToArray().Length
            };

            return Ok(result);
        }


        [HttpGet("{id}")]
        public IActionResult GetAnomalyReportById([FromRoute] int id)
        {
            //var query = _db.AnomalyReports.FindAsync(id);
            var result = new
            {
                response = _db.AnomalyReports.Where(r => r.Id == id)
                                    .Include(r => r.AnomelyReportImage)
                                    .ThenInclude(rImage => rImage.Image)
                                    .FirstOrDefault()
            };

            return Ok(result);
        }

        [HttpGet("test")]
        public async Task<IActionResult> TestAsync()
        {
            AzureStorageHelper bName = new AzureStorageHelper();

            List<Uri> names = await bName.ListAsync();
            return Ok(names);

            //return new JsonResult("Tested");
        }

        [HttpPost]
        public async Task<IActionResult> AddAnomalyReport([FromBody] AnomalyReport objAnomalyReport)
        {
            if (!ModelState.IsValid)
            {
                return new JsonResult(new
                {
                    response = "Not Valid",
                    statusCode = 500
                })
                {
                    StatusCode = 500
                };
            }

            var savedObjAnomalyReport = _db.AnomalyReports.Add(objAnomalyReport);
            await _db.SaveChangesAsync();

            return new JsonResult(new
            {
                response = objAnomalyReport,
                statusCode = 200
            })
            {
                StatusCode = 200
            };
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAnomalyReport([FromRoute] int id, [FromBody] AnomalyReport objAnomalyReport)
        {
            if (objAnomalyReport == null || id != objAnomalyReport.Id)
            {
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


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAnomalyReport([FromRoute] int id)
        {
            var findAnomalyReport = await _db.AnomalyReports.FindAsync(id);

            if (findAnomalyReport == null)
            {
                return NotFound();
            }
            else
            {
                _db.AnomalyReports.Remove(findAnomalyReport);
                await _db.SaveChangesAsync();
                return new JsonResult("Anomaly Deleted Successfully");

            }
        }
    }
}
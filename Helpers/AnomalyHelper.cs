using AnomalyService.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnomalyService.Helpers
{
    public class AnomalyHelper
    {
        private readonly ApplicationDBContext _db;
        public AnomalyHelper(ApplicationDBContext db)
        {
            _db = db;
        }
        public void UpdateAnomalyLatLon()
        {
            var idCount = 0;
            var anomalyId = 0;
            var counter = 0;
            var sumOfLat = 0.0;
            var sumOfLon = 0.0;

            var reports = _db.AnomalyReports.ToList().OrderBy(x => x.AnomalyId);
            var anomalies = _db.Anomalys.ToList();
            foreach (var report in reports)
            {
                counter++;


                if (report.Latitude != "" && report.Longitude != "")
                {
                    if (anomalyId == 0)
                    {
                        idCount = idCount + 1;
                        anomalyId = report.AnomalyId;
                        sumOfLat = sumOfLat + Convert.ToDouble(report.Latitude);
                        sumOfLon = sumOfLon + Convert.ToDouble(report.Longitude);

                    }
                    else if (anomalyId == report.AnomalyId)
                    {
                        idCount = idCount + 1;
                        anomalyId = report.AnomalyId;

                        if (counter == reports.Count())
                        {

                            sumOfLat = sumOfLat + Convert.ToDouble(report.Latitude);
                            sumOfLon = sumOfLon + Convert.ToDouble(report.Longitude);
                            var lat = sumOfLat / idCount;
                            var lon = sumOfLon / idCount;

                            var anomaly = anomalies.FirstOrDefault(x => x.Id == anomalyId);
                            anomaly.Latitude = lat.ToString();
                            anomaly.Longitude = lon.ToString();

                            _db.Update(anomaly);
                            _db.SaveChanges();
                            _db.Entry(anomaly).State = EntityState.Detached;
                            continue;
                        }
                        sumOfLat = sumOfLat + Convert.ToDouble(report.Latitude);
                        sumOfLon = sumOfLon + Convert.ToDouble(report.Longitude);

                    }
                    else
                    {

                        var lat = sumOfLat / idCount;
                        var lon = sumOfLon / idCount;

                        var anomaly = anomalies.FirstOrDefault(x => x.Id == anomalyId);
                        anomaly.Latitude = lat.ToString();
                        anomaly.Longitude = lon.ToString();

                        _db.Update(anomaly);
                        _db.SaveChanges();
                        _db.Entry(anomaly).State = EntityState.Detached;

                        idCount = 1;
                        anomalyId = report.AnomalyId;
                        sumOfLon = Convert.ToDouble(report.Longitude);
                        sumOfLat = Convert.ToDouble(report.Latitude);

                        if (counter == reports.Count())
                        {

                            //sumOfLat = sumOfLat; //+ Convert.ToDouble(report.Latitude);
                            //sumOfLon = sumOfLon; //+ Convert.ToDouble(report.Longitude);
                            var latFinal = sumOfLat / idCount;
                            var lonFinal = sumOfLon / idCount;

                            var anomalyFinal = anomalies.FirstOrDefault(x => x.Id == anomalyId);
                            anomalyFinal.Latitude = lat.ToString();
                            anomalyFinal.Longitude = lon.ToString();

                            _db.Update(anomalyFinal);
                            _db.SaveChanges();
                            _db.Entry(anomalyFinal).State = EntityState.Detached;
                            
                        }
                    }
                }


            }
           

        }

        public void UpdateAnomalyLatLon(int AnomalyId)
        {
            var sumOfLat = 0.0;
            var sumOfLon = 0.0;
            var counter = 0;

            var reports=  _db.AnomalyReports.ToList().Where(x=>x.AnomalyId==AnomalyId);

            foreach (var report in reports)
            {
                if (report.Latitude != "" && report.Longitude != "")
                {
                    counter++;
                    sumOfLat = sumOfLat + Convert.ToDouble(report.Latitude);
                    sumOfLon = sumOfLon + Convert.ToDouble(report.Longitude);
                }
               

            }
            var lat = sumOfLat / counter;
            var lon = sumOfLon / counter;

            var anomaly = _db.Anomalys.ToList().FirstOrDefault(x => x.Id == AnomalyId);
            anomaly.Latitude = lat.ToString();
            anomaly.Longitude = lon.ToString();

            _db.Update(anomaly);
            _db.SaveChanges();
            _db.Entry(anomaly).State = EntityState.Detached;


        }
    }
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace AnomalyService.Models
{
    
    public class AnomalyReport
    {
        public AnomalyReport()
        {
        }

        public int  Id
        {
            get;
            set;
        }

        public string Title
        {
            get;
            set;
        }

        public string Comment
        {
            get;
            set;
        }

        public string Kp
        {
            get;
            set;
        }

        public string Latitude
        {
            get;
            set;
        }

        public string Longitude
        {
            get;
            set;
        }

        public string Road
        {
            get;
            set;
        }

        [DefaultValue(true)]
        public DateTime CreatedAt
        {
            get;
            set;
        }

        [DefaultValue(true)]
        public DateTime UpdatedAt
        {
            get;
            set;
        }

        public int AnomalyId { get; set; }
        //public Anomaly Anomaly { get; set; }

        public List<AnomalyReportImage> AnomelyReportImage { get; set; }

    }
}

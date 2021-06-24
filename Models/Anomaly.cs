﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace AnomalyService.Models
{
    public class Anomaly
    {
        public Anomaly()
        {
        }

        public int Id
        {
            get;
            set;
        }

        public string Title
        {
            get;
            set;
        }

        public string AnomalyType
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

        public IList<AnomalyReport> AnomelyReport { get; set; }
    }
}

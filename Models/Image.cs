using System;
using System.Collections.Generic;
using System.ComponentModel;
using AnomalyService.Helpers;

namespace AnomalyService.Models
{
    public class Image
    {
        public Image()
        {
        }

        public int Id
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public string Road
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

        public DateTime TakenAt
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


        public Uri Url
        {
            get
            {
                return new AzureStorageHelper().GetServiceSasUriForBlurBlob(this.Name);
            } 
        }

        public List<AnomalyReportImage> AnomelyReportImage { get; set; }

    }

   
}

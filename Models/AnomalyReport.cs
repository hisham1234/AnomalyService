using System;
namespace AnomalyService.Models
{
    public class AnomalyReport
    {
        public AnomalyReport()
        {
        }

        public int  id
        {
            get;
            set;
        }

        public string title
        {
            get;
            set;
        }

        public string comment
        {
            get;
            set;
        }

        public string position
        {
            get;
            set;
        }

        public string referred_anomaly
        {
            get;
            set;
        }

        public string image_url
        {
            get;
            set;
        }

        public DateTime created_at
        {
            get;
            set;
        }

        public DateTime updated_at
        {
            get;
            set;
        }


    }
}

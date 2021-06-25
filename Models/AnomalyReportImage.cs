using System;
using System.ComponentModel;

namespace AnomalyService.Models
{
    public class AnomalyReportImage
    {
        public AnomalyReportImage()
        {
        }

        public int Id
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

        public int AnomalyReportId { get; set; }
        public AnomalyReport AnomalyReport { get; set; }

        public int ImageId { get; set; }
        public Image Image { get; set; }
    }
}

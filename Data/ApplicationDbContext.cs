using System;
using AnomalyService.Models;
using Microsoft.EntityFrameworkCore;

namespace AnomalyService.Data
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext()
        {
        }

        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {
        }

        public DbSet<Anomaly> Anomalys { get; set; }
        public DbSet<AnomalyReport> AnomalyReports { get; set; }
        public DbSet<AnomalyReportImage> AnomalyReportImages { get; set; }
        public DbSet<Image> Images { get; set; }
    }
}

using System;
using AnomalyService.Models;
using Microsoft.EntityFrameworkCore;

namespace AnomalyService.Data
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {
        }

        public DbSet<AnomalyReport> AnomalyReports { get; set; }
    }
}

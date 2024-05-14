using DetectLicensePlate.Entities;
using Microsoft.EntityFrameworkCore;

namespace DetectLicensePlate.Data
{
    public class DataContext: DbContext
    {
        public DataContext(DbContextOptions<DataContext> options): base(options)
        {

        }
        public DbSet<PlateRecognition> PlateRecognition { get; set; }
        public DbSet<PlateResults> PlateResults { get; set; }
        public DbSet<PlateBox> PlateBox { get; set; }

    }
}

using DataAccessLayer.Entities;
using System.Data.Entity;

namespace DataAccessLayer
{
    public class ATTSDbContext : DbContext
    {
        public DbSet<DataTable> DataTables { get; set; }

        public ATTSDbContext()
            :base("ATTSDatabase")
        {

        }
    }
}
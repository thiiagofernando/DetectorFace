using DetectorDeFace.Models.Mapping;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DetectorDeFace.Models
{
    public class BdDataContext : DbContext
    {
        static BdDataContext()
        {
            Database.SetInitializer<BdDataContext>(null);
        }
        public BdDataContext()
            : base("Name=StrBd")
        {

        }
        public DbSet<Face> faces { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new FaceMapping());
        }
    }
}

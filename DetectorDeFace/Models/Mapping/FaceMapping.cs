using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DetectorDeFace.Models.Mapping
{
    public class FaceMapping : EntityTypeConfiguration<Face>
    {
        public FaceMapping()
        {
            this.HasKey(t => t.Id);
            this.Property(t => t.NomeFace).IsRequired().HasMaxLength(200);
            this.Property(t => t.DataCadastro).IsRequired();

            this.ToTable("Faces");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.NomeFace).HasColumnName("NomeFace");
            this.Property(t => t.DataCadastro).HasColumnName("DataCadastro");
        }
    }
}

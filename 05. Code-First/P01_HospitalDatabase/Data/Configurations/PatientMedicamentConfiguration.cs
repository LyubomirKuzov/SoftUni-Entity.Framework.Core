using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using P01_HospitalDatabase.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace P01_HospitalDatabase.Data.Configurations
{
    public class PatientMedicamentConfiguration : IEntityTypeConfiguration<PatientMedicament>
    {
        public void Configure(EntityTypeBuilder<PatientMedicament> pm)
        {
            pm.HasKey(pm => new { pm.PatientId, pm.MedicamentId });

            pm.HasOne(p => p.Patient)
                .WithMany(m => m.Prescriptions)
                .HasForeignKey(p => p.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            pm.HasOne(m => m.Medicament)
                .WithMany(p => p.Prescriptions)
                .HasForeignKey(m => m.MedicamentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

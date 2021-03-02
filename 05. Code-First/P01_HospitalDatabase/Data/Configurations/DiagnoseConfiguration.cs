using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using P01_HospitalDatabase.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace P01_HospitalDatabase.Data.Configurations
{
    public class DiagnoseConfiguration : IEntityTypeConfiguration<Diagnose>
    {
        public void Configure(EntityTypeBuilder<Diagnose> d)
        {
            d.HasOne(p => p.Patient)
                .WithMany(d => d.Diagnoses)
                .HasForeignKey(p => p.PatientId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

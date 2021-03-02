using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using P01_HospitalDatabase.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace P01_HospitalDatabase.Data.Configurations
{
    public class VisitationConfiguration : IEntityTypeConfiguration<Visitation>
    {
        public void Configure(EntityTypeBuilder<Visitation> v)
        {
            v.HasOne(p => p.Patient)
                .WithMany(v => v.Visitations)
                .HasForeignKey(p => p.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            v.HasOne(d => d.Doctor)
                .WithMany(v => v.Visitations)
                .HasForeignKey(d => d.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

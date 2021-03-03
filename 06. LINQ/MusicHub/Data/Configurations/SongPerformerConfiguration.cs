using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicHub.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MusicHub.Data.Configurations
{
    public class SongPerformerConfiguration : IEntityTypeConfiguration<SongPerformer>
    {
        public void Configure(EntityTypeBuilder<SongPerformer> sp)
        {
            sp.HasKey(sp => new { sp.SongId, sp.PerformerId });

            sp.HasOne(s => s.Song)
                .WithMany(p => p.SongPerformers)
                .HasForeignKey(s => s.SongId)
                .OnDelete(DeleteBehavior.Restrict);

            sp.HasOne(p => p.Performer)
                .WithMany(s => s.PerformerSongs)
                .HasForeignKey(p => p.PerformerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

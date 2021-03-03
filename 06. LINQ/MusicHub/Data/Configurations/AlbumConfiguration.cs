using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicHub.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MusicHub.Data.Configurations
{
    public class AlbumConfiguration : IEntityTypeConfiguration<Album>
    {
        public void Configure(EntityTypeBuilder<Album> a)
        {
            a.HasOne(p => p.Producer)
                .WithMany(a => a.Albums)
                .HasForeignKey(p => p.ProducerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

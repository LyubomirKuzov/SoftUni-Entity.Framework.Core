using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicHub.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MusicHub.Data.Configurations
{
    public class SongConfiguration : IEntityTypeConfiguration<Song>
    {
        public void Configure(EntityTypeBuilder<Song> s)
        {
            s.HasOne(w => w.Writer)
                .WithMany(s => s.Songs)
                .HasForeignKey(w => w.WriterId)
                .OnDelete(DeleteBehavior.Restrict);

            s.HasOne(a => a.Album)
                .WithMany(s => s.Songs)
                .HasForeignKey(a => a.AlbumId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

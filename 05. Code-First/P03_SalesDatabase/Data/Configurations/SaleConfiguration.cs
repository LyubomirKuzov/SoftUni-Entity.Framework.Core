using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using P03_SalesDatabase.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace P03_SalesDatabase.Data.Configurations
{
    public class SaleConfiguration : IEntityTypeConfiguration<Sale>
    {
        public void Configure(EntityTypeBuilder<Sale> s)
        {
            s.HasOne(s => s.Product)
                .WithMany(p => p.Sales)
                .HasForeignKey(s => s.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            s.HasOne(s => s.Customer)
                .WithMany(c => c.Sales)
                .HasForeignKey(s => s.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            s.HasOne(s => s.Store)
                .WithMany(st => st.Sales)
                .HasForeignKey(s => s.StoreId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

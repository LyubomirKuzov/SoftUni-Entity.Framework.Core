using RealEstate.Services.Models;
using RealEstates.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Services
{
    public class DistrictsService : IDistrictsService
    {
        private ApplicationDbContext dbContext;



        public DistrictsService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }


        public IEnumerable<DistrictInfoDto> GetMostExpensiveDistricts(int count)
        {
            var districts = dbContext.Districts
                .Select(d => new DistrictInfoDto()
                {
                    Name = d.Name,
                    PropertiesCount = d.Properties.Count(),
                    AveragePricePerSquareMeter = d.Properties.Where(p => p.Price.HasValue)
                                                 .Average(p => p.Price / (decimal)p.Size) ?? 0
                })
                .OrderByDescending(p => p.AveragePricePerSquareMeter)
                .Take(count)
                .ToList();

            return districts;
        }
    }
}

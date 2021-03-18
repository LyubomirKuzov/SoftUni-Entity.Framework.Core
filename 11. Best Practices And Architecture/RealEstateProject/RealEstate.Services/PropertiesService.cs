using RealEstate.Services.Models;
using RealEstates.Data;
using RealEstates.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Services
{
    public class PropertiesService : IPropertiesService
    {
        private ApplicationDbContext dbContext;



        public PropertiesService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }



        public void Add(string district, int floor, int maxFloor, int size, int yardSize, int year, string propertyType, string buildingType, int price)
        {
            var property = new Property()
            {
                Floor = floor <= 0 || floor > 255 ? null : (byte)floor,
                TotalFloors = maxFloor <= 0 || maxFloor > 255 ? null : (byte)maxFloor,
                Size = size,
                YardSize = yardSize <= 0 ? null : yardSize,
                Year = year <= 1800 ? null : year,
                Price = price <= 0 ? null : price
            };

            var dbDistrict = dbContext.Districts.FirstOrDefault(d => d.Name == district);

            if (dbDistrict == null)
            {
                dbDistrict = new District()
                {
                    Name = district
                };
            }

            property.District = dbDistrict;

            var dbPropertyType = dbContext.PropertyTypes.FirstOrDefault(p => p.Name == propertyType);

            if (dbPropertyType == null)
            {
                dbPropertyType = new PropertyType()
                {
                    Name = propertyType
                };
            }

            property.Type = dbPropertyType;

            var dbBuildingType = dbContext.BuildingTypes.FirstOrDefault(b => b.Name == buildingType);

            if (dbBuildingType == null)
            {
                dbBuildingType = new BuildingType()
                {
                    Name = buildingType
                };
            }

            property.BuildingType = dbBuildingType;

            dbContext.Properties.Add(property);
            dbContext.SaveChanges();
        }

        public decimal AveragePricePerSquareMeter()
        {
            return dbContext.Properties
                .Where(p => p.Price.HasValue)
                .Average(p => p.Price / (decimal)p.Size ?? 0); 
        }

        public IEnumerable<PropertyInfoDto> Search(int minPrice, int maxPrice, int minSize, int maxSize)
        {
            var properties = dbContext.Properties
                .Where(p => p.Price >= minPrice && p.Price <= maxPrice && p.Size >= minPrice && p.Size <= maxSize)
                .Select(p => new PropertyInfoDto()
                {
                    Size = p.Size,
                    Price = p.Price ?? 0,
                    BuildingType = p.BuildingType.Name,
                    DistrictName = p.District.Name,
                    PropertyType = p.Type.Name
                })
                .ToList();

            return properties;
        }
    }
}

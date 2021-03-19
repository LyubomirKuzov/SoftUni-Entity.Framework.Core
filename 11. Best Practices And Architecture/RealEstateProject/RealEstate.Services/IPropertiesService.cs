using Microsoft.EntityFrameworkCore.Metadata.Internal;
using RealEstate.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Services
{
    public interface IPropertiesService
    {
        void Add(string district, int floor, int maxFloor, int size,
            int yardSize, int year, string propertyType, string buildingType, int price);

        IEnumerable<PropertyInfoDto> Search(int minPrice, int maxPrice, int minSize, int maxSize);

        decimal AveragePricePerSquareMeter();

        decimal AveragePricePerSquareMeter(int districtId);

        public double AverageSize(int districtId);

        public IEnumerable<PropertyInfoFullDataDTO> GetFullData(int count);
    }
}

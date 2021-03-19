using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstates.ConsoleApplication.Profiler
{
    public class RealEstateProfiler : Profile
    {
        public RealEstateProfiler()
        {
            this.CreateMap<Property, PropertyInfoDto>()
                .ForMember(x => x.BuildingType, y => y.MapFrom(a => a.BuildingType.Name));

            this.CreateMap<District, DistrictInfoDto>()
                .ForMember(x => x.AveragePricePerSquareMeter, y => y.MapFrom(a => a.Properties
                                                                   .Where(p => p.HasValue)
                                                                   .Average(p => p.Price / (decimal)p.Size) ?? 0));

            this.CreateMap(Property, PropertyInfoFullDataDTO)
                .ForMember(x => x.BuildingType, y => y.MapFrom(a => a.BuildingType.Name))
                .ForMember(x => x.PropertyType, y => y.MapFrom(a => a.Type.Name));

            this.CreateMap(Tag, TagInfoDto);
        }
    }
}

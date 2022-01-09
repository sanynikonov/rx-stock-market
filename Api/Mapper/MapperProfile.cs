using AutoMapper;

namespace Api.Mapper;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        CreateMap<Business.Stock.StockTimeSeries, StockTimeSeries>();
    }
}
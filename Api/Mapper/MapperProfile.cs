using AutoMapper;
using Business.Stock.Price;

namespace Api.Mapper;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        CreateMap<Business.Stock.StockTimeSeries, StockTimeSeries>();
        CreateMap<PriceRequest, StreamRequest>();
    }
}
using AutoMapper;
using Business.Stock.Price;
using Business.Users;

namespace Api.Mapper;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        CreateMap<Business.Stock.StockTimeSeries, StockTimeSeries>();
        CreateMap<PriceRequest, StreamRequest>();
        CreateMap<UpdatePreferencesRequest, UserPreferences>();
        CreateMap<CompanyInfo, Business.Users.CompanyInfo>();
    }
}
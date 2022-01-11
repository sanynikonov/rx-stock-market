using AutoMapper;
using Business.News;
using Business.Stock.Price;
using Business.Stock.Trend;
using Business.Users;
using Google.Protobuf.WellKnownTypes;

namespace Api.Mapper;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        CreateMap<Business.Stock.StockTimeSeries, StockTimeSeries>();
        CreateMap<PriceRequest, StreamRequest>();
        CreateMap<UpdatePreferencesRequest, UserPreferences>();
        CreateMap<CompanyInfo, Business.Users.CompanyInfo>();
        CreateMap<TrendInfoModel, TrendSeries>();
        CreateMap<NewsModel, NewsMessage>()
            .ForMember(m => m.CreatedAt, c => c.MapFrom(e => Timestamp.FromDateTimeOffset(e.CreatedAt)));
    }
}
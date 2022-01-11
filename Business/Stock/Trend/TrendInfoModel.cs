using Business.News;

namespace Business.Stock.Trend;

public class TrendInfoModel
{
    public long Timestamp { get; set; }
    public double PriceChange { get; set; }
    public string Company { get; set; }
    public string Currency { get; set; }
    public IEnumerable<NewsModel> News { get; set; }
}
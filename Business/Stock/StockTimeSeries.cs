namespace Business.Stock;

public class StockTimeSeries
{
    public long Timestamp { get; set; }
    public double? High { get; set; }
    public double? Close { get; set; }
    public double? Low { get; set; }
    public double? Open { get; set; }
    public double? Volume { get; set; }
    public string Currency { get; set; }
    public string Company { get; set; }
}
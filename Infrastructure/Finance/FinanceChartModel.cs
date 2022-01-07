namespace Infrastructure.Finance;

public class FinanceChartModel
{
    public Chart Chart { get; set; }
}

public class Chart
{
    public Result[] Result { get; set; }
    public string Error { get; set; }
}

public class Result
{
    public Meta Meta { get; set; }
    public long[] Timestamp { get; set; }
    public Indicators Indicators { get; set; }
}

public class Indicators
{
    public Quote[] Quote { get; set; }
}

public class Quote
{
    public double?[] High { get; set; }
    public double?[] Close { get; set; }
    public double?[] Low { get; set; }
    public double?[] Open { get; set; }
    public long?[] Volume { get; set; }
}

public class Meta
{
    public string Currency { get; set; }
    public string Symbol { get; set; }
    public string ExchangeName { get; set; }
    public string InstrumentType { get; set; }
    public long FirstTradeDate { get; set; }
    public long RegularMarketTime { get; set; }
    public long Gmtoffset { get; set; }
    public string Timezone { get; set; }
    public string ExchangeTimezoneName { get; set; }
    public double RegularMarketPrice { get; set; }
    public double ChartPreviousClose { get; set; }
    public double PreviousClose { get; set; }
    public long Scale { get; set; }
    public long PriceHint { get; set; }
    public CurrentTradingPeriod CurrentTradingPeriod { get; set; }
    public Post[][] TradingPeriods { get; set; }
    public string DataGranularity { get; set; }
    public string Range { get; set; }
    public string[] ValidRanges { get; set; }
}

public class CurrentTradingPeriod
{
    public Post Pre { get; set; }
    public Post Regular { get; set; }
    public Post Post { get; set; }
}

public class Post
{
    public string Timezone { get; set; }
    public long End { get; set; }
    public long Start { get; set; }
    public long Gmtoffset { get; set; }
}

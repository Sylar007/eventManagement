namespace Celebratix.Common.Configs;

public class TicketScanningConfig
{
    public int ValidForSeconds { get; set; }

    public string AesKey { get; set; } = null!;
}

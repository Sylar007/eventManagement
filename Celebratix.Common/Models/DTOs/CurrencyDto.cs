using Celebratix.Common.Models.DbModels;

namespace Celebratix.Common.Models.DTOs;

public class CurrencyDto
{
    public CurrencyDto(Currency dbModel)
    {
        Code = dbModel.Code;
        Name = dbModel.Name;
        Symbol = dbModel.Symbol;
        DecimalPlaces = dbModel.DecimalPlaces;
        Enabled = dbModel.Enabled;
    }

    public string Code { get; set; }

    public string Name { get; set; }

    public string Symbol { get; set; }

    public int DecimalPlaces { get; set; }

    public bool Enabled { get; set; } = false;
}

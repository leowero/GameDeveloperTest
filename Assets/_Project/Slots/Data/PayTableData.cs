using Project.Slots.Domain.Configuration;
using Project.Slots.Domain.Symbols;

namespace Project.Slots.Data
{
    public static class PayTableData
    {
        public static PayTable payTable = new PayTableBuilder()
                .AddSymbol(new SymbolPayBuilder(SymbolsConstants.BellSymbol)
                .AddPayout(2, 25)
                .AddPayout(3, 50)
                .AddPayout(4, 75)
                .AddPayout(5, 100)
                )
                .AddSymbol(new SymbolPayBuilder(SymbolsConstants.CherrySymbol)
                .AddPayout(2, 1)
                .AddPayout(3, 2)
                .AddPayout(4, 5)
                .AddPayout(5, 10)
                )
                .AddSymbol(new SymbolPayBuilder(SymbolsConstants.GrapesSymbol)
                .AddPayout(2, 5)
                .AddPayout(3, 10)
                .AddPayout(4, 20)
                .AddPayout(5, 50)
                )
                .AddSymbol(new SymbolPayBuilder(SymbolsConstants.LemonSymbol)
                .AddPayout(2, 2)
                .AddPayout(3, 5)
                .AddPayout(4, 10)
                .AddPayout(5, 20)
                )
                .AddSymbol(new SymbolPayBuilder(SymbolsConstants.OrangeSymbol)
                .AddPayout(2, 5)
                .AddPayout(3, 10)
                .AddPayout(4, 15)
                .AddPayout(5, 30)
                )
                .AddSymbol(new SymbolPayBuilder(SymbolsConstants.PlumSymbol)
                .AddPayout(2, 5)
                .AddPayout(3, 10)
                .AddPayout(4, 20)
                .AddPayout(5, 40)
                )
                .AddSymbol(new SymbolPayBuilder(SymbolsConstants.WatermelonSymbol)
                .AddPayout(2, 10)
                .AddPayout(3, 20)
                .AddPayout(4, 35)
                .AddPayout(5, 60)
                )
                .Build();
    }
}
using Project.Slots.Domain.Configuration;
using Project.Slots.Domain.Symbols;

namespace Project.Slots.Data
{
    /// <summary>
    /// Provides a default pay table configuration for the slot machine.
    /// </summary>
    /// <remarks>
    /// This is a static, code-defined configuration used by the engine and validation rules.
    /// In a production setup, this would likely be loaded from ScriptableObjects or external data.
    /// </remarks>
    public static class PayTableData
    {
        /// <summary>
        /// Default pay table instance used by the project.
        /// </summary>
        /// <remarks>
        /// Exposed as a static readonly reference to prevent reassignment at runtime.
        /// </remarks>
        public static readonly PayTable PayTable = new PayTableBuilder()
            .AddSymbol(new SymbolPayBuilder(SymbolsConstants.BellSymbol.Type)
                .AddPayout(2, 25)
                .AddPayout(3, 50)
                .AddPayout(4, 75)
                .AddPayout(5, 100)
            )
            .AddSymbol(new SymbolPayBuilder(SymbolsConstants.CherrySymbol.Type)
                .AddPayout(2, 1)
                .AddPayout(3, 2)
                .AddPayout(4, 5)
                .AddPayout(5, 10)
            )
            .AddSymbol(new SymbolPayBuilder(SymbolsConstants.GrapesSymbol.Type)
                .AddPayout(2, 5)
                .AddPayout(3, 10)
                .AddPayout(4, 20)
                .AddPayout(5, 50)
            )
            .AddSymbol(new SymbolPayBuilder(SymbolsConstants.LemonSymbol.Type)
                .AddPayout(2, 2)
                .AddPayout(3, 5)
                .AddPayout(4, 10)
                .AddPayout(5, 20)
            )
            .AddSymbol(new SymbolPayBuilder(SymbolsConstants.OrangeSymbol.Type)
                .AddPayout(2, 5)
                .AddPayout(3, 10)
                .AddPayout(4, 15)
                .AddPayout(5, 30)
            )
            .AddSymbol(new SymbolPayBuilder(SymbolsConstants.PlumSymbol.Type)
                .AddPayout(2, 5)
                .AddPayout(3, 10)
                .AddPayout(4, 20)
                .AddPayout(5, 40)
            )
            .AddSymbol(new SymbolPayBuilder(SymbolsConstants.WatermelonSymbol.Type)
                .AddPayout(2, 10)
                .AddPayout(3, 20)
                .AddPayout(4, 35)
                .AddPayout(5, 60)
            )
            .Build();
    }
}
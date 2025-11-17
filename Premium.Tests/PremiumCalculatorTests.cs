using premium.Api.Services;
using Xunit;

public class PremiumCalculatorTests
{
    [Fact]
    public void CalculatesMonthlyPremium_Correctly()
    {

        var calc = new PremiumCalculator();
        decimal death = 100000m;
        decimal factor = 1.5m;
        int age = 30;

        // Act
        var monthly = calc.CalculateMonthlyPremium(death, factor, age);

        // With common interpretation (yearly=(death*factor*age)/1000; monthly=yearly/12), expected 375.00
        Assert.Equal(375.00m, monthly);


        //var calc = new PremiumCalculator();


        //// Example: death=100000, factor=1.5, age=30
        //// yearly = (100000 * 1.5 * 30)/1000 = 4500
        //// monthly = 4500/12 = 375
        //var monthly = calc.CalculateMonthlyPremium(100000m, 1.5m, 30);
        //Assert.Equal(375.00m, monthly);
    }
}

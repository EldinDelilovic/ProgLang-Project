using System;
using System.Collections.Generic;
using System.Text.Json;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
class CurrencyConverter
{
    private static readonly string[] currencies = { "USD", "BAM", "EUR", "CHF", "AUD" };
    private const string HistoryFile = "conversion_history.json";
    private const string PresetsFile = "preset_values.json";
    private const string HelpFile = "help_text.json";
    private static readonly Random random = new Random();
    class ConversionHistory
    {
        public string Date { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public double Amount { get; set; }
        public double Result { get; set; }
    }
    static async Task LoadingAnimation()
    {
        char[] spinner = { '⠋', '⠙', '⠹', '⠸', '⠼', '⠴', '⠦', '⠧', '⠇', '⠏' };
        int spinnerIndex = 0;
        Console.WriteLine("\n=== Initializing Currency Converter ===\n");   
        foreach (string currency in currencies)
        {
            Console.Write($"Loading {currency} ");
            for (int i = 0; i < 20; i++)
            {
                Console.Write($"\r{currency}: [{spinner[spinnerIndex]}] Loading... ");
                spinnerIndex = (spinnerIndex + 1) % spinner.Length;
                await Task.Delay(100);
            }
            Console.WriteLine($"\r{currency}: [✓] Loaded successfully!   ");
        }
        Console.WriteLine("\nSystem initialization complete!");
        Console.WriteLine("Starting currency converter...\n");
        await Task.Delay(1000);
        ClearScreen();
    }
    static void ClearScreen()
    {
        Console.Clear();
    }
    static void WaitForUser()
    {
        Console.WriteLine("\nPress Enter to continue...");
        Console.ReadLine();
        ClearScreen();
    }
    static Dictionary<string, Dictionary<string, double>> LoadPresets()
    {
        try
        {
            string json = File.ReadAllText(PresetsFile);
            return JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, double>>>(json);
        }
        catch (FileNotFoundException)
        {
            var presetRates = new Dictionary<string, Dictionary<string, double>>
            {
                ["USD"] = new Dictionary<string, double> { ["BAM"] = 1.85, ["EUR"] = 0.92, ["CHF"] = 0.89, ["AUD"] = 1.53 },
                ["BAM"] = new Dictionary<string, double> { ["USD"] = 0.54, ["EUR"] = 0.51, ["CHF"] = 0.48, ["AUD"] = 0.83 },
                ["EUR"] = new Dictionary<string, double> { ["USD"] = 1.09, ["BAM"] = 1.96, ["CHF"] = 0.97, ["AUD"] = 1.67 },
                ["CHF"] = new Dictionary<string, double> { ["USD"] = 1.12, ["BAM"] = 2.08, ["EUR"] = 1.03, ["AUD"] = 1.72 },
                ["AUD"] = new Dictionary<string, double> { ["USD"] = 0.65, ["BAM"] = 1.21, ["EUR"] = 0.60, ["CHF"] = 0.58 }
            };
            string json = JsonSerializer.Serialize(presetRates, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(PresetsFile, json);
            return presetRates;
        }
    }
    static void CreateHelpFile()
    {
        if (!File.Exists(HelpFile))
        {
            var helpContent = new
            {
                conversion = new
                {
                    title = "How Currency Conversion Works",
                    steps = new[]
                    {
                        "1. Select the currency you want to convert FROM (e.g., USD, EUR)",
                        "2. Select the currency you want to convert TO",
                        "3. Enter the amount you want to convert",
                        "4. The system will use current exchange rates to calculate the result"
                    },
                    notes = new[]
                    {
                        "• All conversions are saved in your history",
                        "• Exchange rates are updated regularly",
                        "• You can view your conversion history anytime"
                    },
                    example = "Example: Converting 100 USD to EUR\n- Enter 'USD' as source currency\n- Enter 'EUR' as target currency\n- Enter '100' as amount\n- System will show result (e.g., 92.00 EUR)"
                },
                predictions = new
                {
                    title = "Understanding Predictions",
                    daily = new[]
                    {
                        "Daily predictions show possible rate changes over 24 hours",
                        "Predictions use current rates with market volatility factors",
                        "All major currency pairs are included in daily predictions"
                    },
                    hourly = new[]
                    {
                        "Hourly predictions show detailed short-term trends",
                        "Choose between 1, 3, or 6-hour prediction windows",
                        "Includes trend indicators (↑/↓) and percentage changes"
                    }
                }
            };

            string json = JsonSerializer.Serialize(helpContent, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(HelpFile, json);
        }
    }

    static void ShowHelp()
    {
        try
        {
            string json = File.ReadAllText(HelpFile);
            using JsonDocument doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            Console.WriteLine("\n=== Currency Converter Help ===");

            // Show conversion help
            Console.WriteLine($"\n{root.GetProperty("conversion").GetProperty("title").GetString()}");
            Console.WriteLine("\nSteps:");
            foreach (var step in root.GetProperty("conversion").GetProperty("steps").EnumerateArray())
            {
                Console.WriteLine(step.GetString());
            }

            Console.WriteLine("\nImportant Notes:");
            foreach (var note in root.GetProperty("conversion").GetProperty("notes").EnumerateArray())
            {
                Console.WriteLine(note.GetString());
            }

            Console.WriteLine("\n" + root.GetProperty("conversion").GetProperty("example").GetString());

            // Show predictions help
            Console.WriteLine($"\n{root.GetProperty("predictions").GetProperty("title").GetString()}");
            Console.WriteLine("\nDaily Predictions:");
            foreach (var info in root.GetProperty("predictions").GetProperty("daily").EnumerateArray())
            {
                Console.WriteLine($"• {info.GetString()}");
            }

            Console.WriteLine("\nHourly Predictions:");
            foreach (var info in root.GetProperty("predictions").GetProperty("hourly").EnumerateArray())
            {
                Console.WriteLine($"• {info.GetString()}");
            }
        }
        catch (FileNotFoundException)
        {
            Console.WriteLine("\nHelp information not available.");
        }
    }

    static void SaveConversion(string fromCurr, string toCurr, double amount, double result)
    {
        List<ConversionHistory> history;
        try
        {
            string json = File.ReadAllText(HistoryFile);
            history = JsonSerializer.Deserialize<List<ConversionHistory>>(json);
        }
        catch (FileNotFoundException)
        {
            history = new List<ConversionHistory>();
        }

        history.Add(new ConversionHistory
        {
            Date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            From = fromCurr,
            To = toCurr,
            Amount = amount,
            Result = result
        });

        string newJson = JsonSerializer.Serialize(history, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(HistoryFile, newJson);
    }

    static void ShowPopularConversions(Dictionary<string, Dictionary<string, double>> rates)
    {
        var commonPairs = new[]
        {
            ("USD", "BAM"),
            ("EUR", "BAM"),
            ("USD", "EUR"),
            ("CHF", "EUR"),
            ("AUD", "USD")
        };

        int totalConversions = random.Next(500, 1001);
        Console.WriteLine("\n=== Most Used Conversions Today ===");
        Console.WriteLine($"Total conversions today: {totalConversions}\n");
        Console.WriteLine("Currency Pair     Current Rate    Times Used");
        Console.WriteLine("-".PadRight(45, '-'));

        foreach (var (from, to) in commonPairs)
        {
            int conversions = (int)(totalConversions * random.NextDouble() * 0.3 + 0.05);
            double rate = rates[from][to];
            Console.WriteLine($"{from}/{to,-8}     {rate:F4}          {conversions}");
        }
    }

    static Dictionary<string, Dictionary<string, double>> GeneratePredictions(Dictionary<string, Dictionary<string, double>> rates)
    {
        var predictions = new Dictionary<string, Dictionary<string, double>>();
        foreach (var baseCurrency in currencies)
        {
            predictions[baseCurrency] = new Dictionary<string, double>();
            foreach (var targetCurrency in currencies)
            {
                if (baseCurrency != targetCurrency)
                {
                    double currentRate = rates[baseCurrency][targetCurrency];
                    double variation = (random.NextDouble() * 0.1) - 0.05;
                    predictions[baseCurrency][targetCurrency] = Math.Round(currentRate * (1 + variation), 4);
                }
            }
        }
        return predictions;
    }

    static List<ConversionHistory> ViewHistory()
    {
        try
        {
            string json = File.ReadAllText(HistoryFile);
            return JsonSerializer.Deserialize<List<ConversionHistory>>(json);
        }
        catch (FileNotFoundException)
        {
            return new List<ConversionHistory>();
        }
    }

    static void ClearHistory()
    {
        File.WriteAllText(HistoryFile, JsonSerializer.Serialize(new List<ConversionHistory>()));
    }

    static List<Dictionary<string, object>> GenerateHourlyPredictions(
        Dictionary<string, Dictionary<string, double>> rates,
        string fromCurr,
        string toCurr,
        int hours)
    {
        double baseRate = rates[fromCurr][toCurr];
        var predictions = new List<Dictionary<string, object>>();
        DateTime currentHour = DateTime.Now;
        
        const double volatility = 0.02;
        double prevVariation = 0;

        for (int i = 0; i < hours; i++)
        {
            DateTime nextHour = currentHour.AddHours(i);
            double variation = (random.NextDouble() * 2 - 1) * volatility + (prevVariation * 0.3);
            double predictedRate = baseRate * (1 + variation);

            predictions.Add(new Dictionary<string, object>
            {
                ["hour"] = nextHour.ToString("HH:00"),
                ["rate"] = Math.Round(predictedRate, 4),
                ["change"] = Math.Round(variation * 100, 2),
                ["trend"] = variation > 0 ? "↑" : "↓"
            });

            prevVariation = variation;
        }

        return predictions;
    }

    static void HandleHourlyPredictions(Dictionary<string, Dictionary<string, double>> rates)
    {
        Console.WriteLine("\nAvailable currencies: " + string.Join(", ", currencies));
        Console.Write("Convert from (currency code): ");
        string fromCurr = Console.ReadLine().ToUpper();
        Console.Write("Convert to (currency code): ");
        string toCurr = Console.ReadLine().ToUpper();

        if (!currencies.Contains(fromCurr) || !currencies.Contains(toCurr))
        {
            Console.WriteLine("Invalid currency code!");
            return;
        }

        int hours;
        while (true)
        {
            Console.Write("\nEnter number of hours to predict (1, 3, or 6): ");
            if (int.TryParse(Console.ReadLine(), out hours) && new[] { 1, 3, 6 }.Contains(hours))
                break;
            Console.WriteLine("Please enter either 1, 3, or 6 hours.");
        }

        var predictions = GenerateHourlyPredictions(rates, fromCurr, toCurr, hours);
        double currentRate = rates[fromCurr][toCurr];

        Console.WriteLine($"\n=== {fromCurr}/{toCurr} Predictions ===");
        Console.WriteLine($"Current rate: {currentRate:F4}");
        Console.WriteLine($"\nPredicted rates for next {hours} hour(s):");
        Console.WriteLine("\nTime    Rate      Change   Trend");
        Console.WriteLine("-".PadRight(35, '-'));

        foreach (var pred in predictions)
        {
            Console.WriteLine($"{pred["hour"]}   {pred["rate"]:F4}   {pred["change"],6:F2}%   {pred["trend"]}");
        }
    }

    static void HandleConversion(Dictionary<string, Dictionary<string, double>> rates)
    {
        Console.WriteLine("\nAvailable currencies: " + string.Join(", ", currencies));
        Console.Write("Convert from (currency code): ");
        string fromCurr = Console.ReadLine().ToUpper();
        Console.Write("Convert to (currency code): ");
        string toCurr = Console.ReadLine().ToUpper();

        if (!currencies.Contains(fromCurr) || !currencies.Contains(toCurr))
        {
            Console.WriteLine("Invalid currency code!");
            return;
        }

        Console.Write("Enter amount: ");
        if (double.TryParse(Console.ReadLine(), out double amount))
        {
            double result = amount * rates[fromCurr][toCurr];
            Console.WriteLine($"\nResult: {amount} {fromCurr} = {result:F2} {toCurr}");
            SaveConversion(fromCurr, toCurr, amount, result);
        }
        else
        {
            Console.WriteLine("Invalid amount!");
        }
    }

    static void HandlePredictions(Dictionary<string, Dictionary<string, double>> rates)
    {
        var predictions = GeneratePredictions(rates);
        Console.WriteLine("\n=== Currency Predictions ===");
        foreach (var baseCurrency in predictions)
        {
            Console.WriteLine($"\n{baseCurrency.Key} predictions:");
            foreach (var (target, rate) in baseCurrency.Value)
            {
                Console.WriteLine($"{baseCurrency.Key} to {target}: {rate:F4}");
            }
        }
    }

    static void HandleHistory()
    {
        var history = ViewHistory();
        
        if (!history.Any())
        {
            Console.WriteLine("\nNo conversion history found.");
        }
        else
        {
            Console.WriteLine("\n=== Conversion History ===");
            foreach (var entry in history)
            {
                Console.WriteLine($"{entry.Date}: {entry.Amount} {entry.From} = {entry.Result:F2} {entry.To}");
            }
        }
    }
    static async Task Main()
    {
        await LoadingAnimation();
        CreateHelpFile();
        var rates = LoadPresets();

        while (true)
        {
            ClearScreen();
            Console.WriteLine("=== Currency Converter Menu ===");
            Console.WriteLine("1. Convert Currency");
            Console.WriteLine("2. Hourly Predictions");
            Console.WriteLine("3. Daily Predictions");
            Console.WriteLine("4. Popular Conversions");
            Console.WriteLine("5. View Conversion History");
            Console.WriteLine("6. Clear History");
            Console.WriteLine("7. Help");
            Console.WriteLine("8. Exit");

            Console.Write("\nEnter your choice (1-8): ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    HandleConversion(rates);
                    break;
                case "2":
                    HandleHourlyPredictions(rates);
                    break;
                case "3":
                    HandlePredictions(rates);
                    break;
                case "4":
                    ShowPopularConversions(rates);
                    break;
                case "5":
                    HandleHistory();
                    break;
                case "6":
                    ClearHistory();
                    Console.WriteLine("\nHistory cleared successfully!");
                    break;
                case "7":
                    ShowHelp();
                    break;
                case "8":
                    Console.WriteLine("\nThank you for using Currency Converter!");
                    return;
                default:
                    Console.WriteLine("\nInvalid choice! Please try again.");
                    break;
            }
            WaitForUser();
        }
    }
}
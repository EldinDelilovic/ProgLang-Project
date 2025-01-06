import json
import random
import os
import time
import sys
from itertools import cycle
from datetime import datetime, timedelta
from typing import Dict, List

currencies = ['USD', 'BAM', 'EUR', 'CHF', 'AUD']

# history_file: Stores all conversions for user analytics and transaction history
# presets_file: Contains exchange rate matrices - updated periodically for rate changes
# help_file: Stores user documentation and guides in JSON format for easy updates

history_file = 'conversion_history.json'
presets_file = 'preset_values.json'
help_file = 'help_text.json'

def loading_animation():
    """Display a fancy currency loading animation"""
    currencies_to_load = currencies
    # pattern sequence creates a smooth clockwise spinning effect
    spinner = cycle(['⠋', '⠙', '⠹', '⠸', '⠼', '⠴', '⠦', '⠧', '⠇', '⠏'])
    
    print("\n=== Initializing Currency Converter ===\n")
    
    for currency in currencies_to_load:
        # Loading bar
        sys.stdout.write(f'Loading {currency} ')
        for _ in range(20):
            sys.stdout.write(f'\r{currency}: [{next(spinner)}] Loading... ')
            sys.stdout.flush()
            time.sleep(0.1)
        sys.stdout.write(f'\r{currency}: [✓] Loaded successfully!   \n')
        sys.stdout.flush()
    
    print("\nSystem initialization complete!")
    print("Starting currency converter...\n")
    time.sleep(1)
    clear_screen()

def clear_screen():
    os.system('cls' if os.name == 'nt' else 'clear')

def wait_for_user():
    input("\nPress Enter to continue...")
    clear_screen()

def load_presets():
    try:
        with open(presets_file, 'r') as f:
            return json.load(f)
    except FileNotFoundError:
        preset_rates = {
            'USD': {'BAM': 1.85, 'EUR': 0.92, 'CHF': 0.89, 'AUD': 1.53},
            'BAM': {'USD': 0.54, 'EUR': 0.51, 'CHF': 0.48, 'AUD': 0.83},
            'EUR': {'USD': 1.09, 'BAM': 1.96, 'CHF': 0.97, 'AUD': 1.67},
            'CHF': {'USD': 1.12, 'BAM': 2.08, 'EUR': 1.03, 'AUD': 1.72},
            'AUD': {'USD': 0.65, 'BAM': 1.21, 'EUR': 0.60, 'CHF': 0.58}
        }
        with open(presets_file, 'w') as f:
            json.dump(preset_rates, f, indent=2)
        return preset_rates

def create_help_file():
    """Create the help file if it doesn't exist"""
    if not os.path.exists(help_file):
        help_content = {
            "conversion": {
                "title": "How Currency Conversion Works",
                "steps": [
                    "1. Select the currency you want to convert FROM (e.g., USD, EUR)",
                    "2. Select the currency you want to convert TO",
                    "3. Enter the amount you want to convert",
                    "4. The system will use current exchange rates to calculate the result"
                ],
                "notes": [
                    "• All conversions are saved in your history",
                    "• Exchange rates are updated regularly",
                    "• You can view your conversion history anytime"
                ],
                "example": "Example: Converting 100 USD to EUR\n- Enter 'USD' as source currency\n- Enter 'EUR' as target currency\n- Enter '100' as amount\n- System will show result (e.g., 92.00 EUR)"
            },
            "predictions": {
                "title": "Understanding Predictions",
                "daily": [
                    "Daily predictions show possible rate changes over 24 hours",
                    "Predictions use current rates with market volatility factors",
                    "All major currency pairs are included in daily predictions"
                ],
                "hourly": [
                    "Hourly predictions show detailed short-term trends",
                    "Choose between 1, 3, or 6-hour prediction windows",
                    "Includes trend indicators (↑/↓) and percentage changes"
                ]
            }
        }
        with open(help_file, 'w') as f:
            json.dump(help_content, f, indent=2)

def show_help():
    """Display help information from JSON file"""
    try:
        with open(help_file, 'r') as f:
            help_content = json.load(f)
        
        print("\n=== Currency Converter Help ===")
        
        # Show conversion help
        print(f"\n{help_content['conversion']['title']}")
        print("\nSteps:")
        for step in help_content['conversion']['steps']:
            print(step)
        
        print("\nImportant Notes:")
        for note in help_content['conversion']['notes']:
            print(note)
        
        print("\n" + help_content['conversion']['example'])
        
        # Show predictions help
        print(f"\n{help_content['predictions']['title']}")
        print("\nDaily Predictions:")
        for info in help_content['predictions']['daily']:
            print(f"• {info}")
            
        print("\nHourly Predictions:")
        for info in help_content['predictions']['hourly']:
            print(f"• {info}")
            
    except FileNotFoundError:
        print("\nHelp information not available.")

def save_conversion(from_curr, to_curr, amount, result):
    try:
        with open(history_file, 'r') as f:
            history = json.load(f)
    except FileNotFoundError:
        history = []

    history.append({
        'date': datetime.now().strftime('%Y-%m-%d %H:%M:%S'),
        'from': from_curr,
        'to': to_curr,
        'amount': amount,
        'result': result
    })

    with open(history_file, 'w') as f:
        json.dump(history, f, indent=2)

def show_popular_conversions(rates):
    """Display simple popular conversions of the day with conversion counts"""
    # Most common currency pairs using only available currencies
    common_pairs = [
        ('USD', 'BAM'),
        ('EUR', 'BAM'),
        ('USD', 'EUR'),
        ('CHF', 'EUR'),
        ('AUD', 'USD')
    ]
    
    # Generate total conversions for the day
    total_conversions = random.randint(500, 1000)
    
    print("\n=== Most Used Conversions Today ===")
    print(f"Total conversions today: {total_conversions}\n")
    print("Currency Pair     Current Rate    Times Used")
    print("-" * 45)
    
    remaining_total = total_conversions
    
    # Distribute random conversions among pairs
    for i, (from_curr, to_curr) in enumerate(common_pairs):
        # Last pair gets remaining conversions to ensure total adds up
        if i == len(common_pairs) - 1:
            pair_conversions = remaining_total
        else:
            # Generate random number of conversions for this pair
            max_possible = remaining_total - (len(common_pairs) - i - 1)
            pair_conversions = random.randint(50, max_possible)
            remaining_total -= pair_conversions
            
        rate = rates[from_curr][to_curr]
        print(f"{from_curr}/{to_curr:<8}     {rate:.4f}          {pair_conversions}")

        
# Generates short-term exchange rate predictions using market volatility modeling
def generate_predictions(rates):
    predictions = {}
    for base in currencies:
        predictions[base] = {}
        for target in currencies:
            if base != target:
                current_rate = rates[base][target]
                variation = random.uniform(-0.05, 0.05)
                predictions[base][target] = round(current_rate * (1 + variation), 4)
    return predictions

def view_history():
    try:
        with open(history_file, 'r') as f:
            return json.load(f)
    except FileNotFoundError:
        return []

def clear_history():
    with open(history_file, 'w') as f:
        json.dump([], f)

def generate_hourly_predictions(rates: Dict, from_curr: str, to_curr: str, hours: int) -> List[Dict]:
    """Generate simple hourly predictions"""
    base_rate = rates[from_curr][to_curr]
    predictions = []
    current_hour = datetime.now()
    
    volatility = 0.02
    prev_variation = 0
    
    for i in range(hours):
        next_hour = current_hour + timedelta(hours=i)
        
        # Include some trend continuity
        variation = random.uniform(-volatility, volatility) + (prev_variation * 0.3)
        predicted_rate = base_rate * (1 + variation)
        
        predictions.append({
            'hour': next_hour.strftime('%H:00'),
            'rate': round(predicted_rate, 4),
            'change': round(variation * 100, 2),
            'trend': '↑' if variation > 0 else '↓'
        })
        
        prev_variation = variation
    
    return predictions

def handle_hourly_predictions(rates: Dict):
    """Handle the hourly predictions interface"""
    print("\nAvailable currencies:", ', '.join(currencies))
    from_curr = input("Convert from (currency code): ").upper()
    to_curr = input("Convert to (currency code): ").upper()
    
    if from_curr not in currencies or to_curr not in currencies:
        print("Invalid currency code!")
        return
    
    while True:
        try:
            hours = int(input("\nEnter number of hours to predict (1, 3, or 6): "))
            if hours not in [1, 3, 6]:
                print("Please enter either 1, 3, or 6 hours.")
                continue
            break
        except ValueError:
            print("Please enter a valid number.")
    
    predictions = generate_hourly_predictions(rates, from_curr, to_curr, hours)
    current_rate = rates[from_curr][to_curr]
    
    print(f"\n=== {from_curr}/{to_curr} Predictions ===")
    print(f"Current rate: {current_rate:.4f}")
    print(f"\nPredicted rates for next {hours} hour(s):")
    print("\nTime    Rate      Change   Trend")
    print("-" * 35)
    for pred in predictions:
        print(f"{pred['hour']}   {pred['rate']:.4f}   {pred['change']:>6.2f}%   {pred['trend']}")

def handle_conversion(rates):
    print("\nAvailable currencies:", ', '.join(currencies))
    from_curr = input("Convert from (currency code): ").upper()
    to_curr = input("Convert to (currency code): ").upper()
    
    if from_curr not in currencies or to_curr not in currencies:
        print("Invalid currency code!")
        return
        
    try:
        amount = float(input("Enter amount: "))
        result = amount * rates[from_curr][to_curr]
        print(f"\nResult: {amount} {from_curr} = {result:.2f} {to_curr}")
        save_conversion(from_curr, to_curr, amount, result)
    except ValueError:
        print("Invalid amount!")

def handle_predictions(rates):
    predictions = generate_predictions(rates)
    print("\n=== Currency Predictions ===")
    for base in predictions:
        print(f"\n{base} predictions:")
        for target, rate in predictions[base].items():
            print(f"{base} to {target}: {rate:.4f}")

def handle_history():
    history = view_history()
    if not history:
        print("\nNo conversion history found.")
    else:
        print("\n=== Conversion History ===")
        for entry in history:
            print(f"{entry['date']}: {entry['amount']} {entry['from']} = {entry['result']:.2f} {entry['to']}")

def main():
    loading_animation()
    create_help_file()
    rates = load_presets()
    
    while True:
        clear_screen()
        print("=== Currency Converter Menu ===")
        print("1. Convert Currency")
        print("2. Hourly Predictions")
        print("3. Daily Predictions")
        print("4. Popular Conversions")
        print("5. View Conversion History")
        print("6. Clear History")
        print("7. Help")
        print("8. Exit")
        
        choice = input("\nEnter your choice (1-8): ")
        
        match choice:
            case "1":
                handle_conversion(rates)         # Basic conversion - most used feature
            case "2":
                handle_hourly_predictions(rates) # Short-term predictions
            case "3":
                handle_predictions(rates)        # Daily predictions
            case "4":
                show_popular_conversions(rates)  # Popular conversion patterns
            case "5":
                handle_history()                 # View history
            case "6":
                clear_history()                  # Clear history
                print("\nHistory cleared successfully!")
            case "7":
                show_help()                      # Help documentation
            case "8":
                print("\nThank you for using Currency Converter!")
                break
            case _:
                print("\nInvalid choice! Please try again.")
        wait_for_user()

if __name__ == "__main__":
    main()
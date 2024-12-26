# Currency converter

A currency converter application in three different languages (Python, JavaScript, C#) with each having a separate implementation.

## Table of Contents

- [About the Application](#about-the-application)
- [Features](#features)
  - [Single Currency Conversion](#single-currency-conversion)
  - [Pre-set Exchange Rates](#pre-set-exchange-rates)
  - [Hourly Predictions](#hourly-predictions)
  - [Daily Predictions](#daily-predictions)
  - [Popular Conversions](#popular-conversions)
  - [Conversion History](#conversion-history)
  - [Clear History](#clear-history)
  - [Error Handling](#error-handling)
  - [Help Documentation](#help-documentation)
- [How to Use](#how-to-use)
    
## ABOUT THE APPLICATION:
The main goal of the application is to allow users to convert certain amounts from one currency to another, using pre-set values, for 5 different currencies (EUR, USD, GBP, BAM, CHF). Users can select currencies and enter amounts to get instant conversion results. Additionaly, the programm alows users to see transaction histoy and see predictions for potential fluctuations, making it a tool with many flexibilities in different situations.

## FEATURES:

### Single Currency Conversion:
Allows users to input an amount in one currency and receive the converted amount in the selected currency. Additionally, users have access to an easy, user-friendly menu with options to view conversion history and explore 24-hour currency predictions.
    
### Pre-set values:
Allows users to convert currencies using pre-set exchange rates for five different currencies (EUR, USD, GBP, BAM, CHF), enabling the program to function offline. The pre-set values are closely aligned with real time values to ensure the user recives more precise results. 

Sample for euro (EUR) JSON file:
```json
 "USD": {
    "BAM": 1.89,
    "EUR": 0.92,
    "CHF": 0.89,
    "AUD": 1.53
  },
```
### Hourly Predictions
The application generates hourly exchange rate predictions for selected currencies over 1, 3, or 6 hours. It calculates potential trends and variations to give users an idea of short-term fluctuations.

### Daily Predictions
Explore potential exchange rate trends for the next 24 hours. These predictions are based on randomized variations around current rates, giving users insight into possible currency movements.

### Popular Conversions
View the most commonly used currency pairs and their exchange rates. The application displays a summary of the day’s most popular conversions, including times used and current rates.

### See conversion history:
After every conversion, the application saves the conversion details to a JSON file, this file stores information such as the base currency, the target currency, the conversion rate, amount and the converted value.
   
#### Sample JSON Output:

```json
[
  {
    "date": "2024-12-26 17:01:40",
    "from": "USD",
    "to": "BAM",
    "amount": 100.0,
    "result": 189.0
  }
]
```

### Clear History
Users can easily clear the stored transaction history, ensuring privacy or decluttering the saved data.

### Error handling:
The application/project handles errors such as invalid currency codes, missing exchange rate data and calculation issues. Users recieve prompts which lead to correct inputs while ensuring smooth user experience.

### Menu Example
The menu allows users to select the option which he wants to use in our project with selecting an option using numbers 1 to 5 (as shown below in python).
  
```python
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
        
        choice = input("\nEnter your choice (1-8): ")def main_menu():
    while True:
        print("\n--- Currency Converter Menu ---")
        print("1. Conversion")
        print("2. See Transaction History")
        print("3. Delete Transaction History")
        print("4. See Predictions for Tomorrow")
        print("5. End Program")

        choice = input("Select an option (1-5): ")
```
### Help Documentation:
Comprehensive help documentation is included in the application, guiding users through features like currency conversion, predictions, and troubleshooting. The help content is stored in JSON format and displayed upon request.

## How to Use:
1. Run the application in your preferred programming language (Python, JavaScript, or C#).
2. Follow the menu options to:
    - Convert currencies.
    - View predictions.
    - Access or clear transaction history.
    - Access help documentation.

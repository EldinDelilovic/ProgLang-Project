# Currency converter

A currency converter application in three different languages (Python, JavaScript, C#) with each having a separate implementation.

## Table of Contents

- [About the Application](#about-the-application)
- [Features](#features)
  - [Single Currency Conversion](#single-currency-conversion)
  - [Pre-set Values](#pre-set-values)
  - [Currency Prediction](#currency-prediction)
  - [Conversion History](#conversion-history)
  - [Error Handling](#error-handling)
  - [Menu Example](#menu-example)
    
## ABOUT THE APPLICATION:
The main goal of the application is to allow users to convert certain amounts from one currency to another, using pre-set values, for 5 different currencies (EUR, USD, GBP, BAM, CHF). Users can select currencies and enter amounts to get instant conversion results. Additionaly, the programm alows users to see transaction histoy and see predictions for potential fluctuations, making it a tool with many flexibilities in different situations.

## FEATURES:

### Single Currency Conversion:
Allows users to input an amount in one currency and receive the converted amount in the selected currency. Additionally, users have access to an easy, user-friendly menu with options to view conversion history and explore 24-hour currency predictions.
    
### Pre-set values:
Allows users to convert currencies using pre-set exchange rates for five different currencies (EUR, USD, GBP, BAM, CHF), enabling the program to function offline. The pre-set values are closely aligned with real time values to ensure the user recives more precise results.

### Currency prediction:
The programm generates random predictions for currency values over the next 24 hours, allowing users to explore potential fluctuations in exchange rates within a 24-hour timeframe. 

### See conversion history:
After every conversion, the application saves the conversion details to a JSON file, this file stores information such as the base currency, the target currency, the conversion rate, amount and the converted value.
   
#### Sample JSON Output:

```json
[
    {
        "base_currency": "USD",
        "target_currency": "EUR",
        "conversion_rate": 0.92,
        "amount": 100,
        "converted_value": 92.0,
        "timestamp": "2024-11-13T18:54:32+01:00"
    }
]
```
### Error handling:
The application/project handles errors such as invalid currency codes, missing exchange rate data and calculation issues. Users recieve prompts which lead to correct inputs while ensuring smooth user experience.

### Menu Example
The menu allows users to select the option which he wants to use in our project with selecting an option using numbers 1 to 5 (as shown below in python).
  
```python
def main_menu():
    while True:
        print("\n--- Currency Converter Menu ---")
        print("1. Conversion")
        print("2. See Transaction History")
        print("3. Delete Transaction History")
        print("4. See Predictions for Tomorrow")
        print("5. End Program")

        choice = input("Select an option (1-5): ")
```

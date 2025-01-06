const readline = require('readline').createInterface({
    input: process.stdin,
    output: process.stdout
});

const fs = require('fs');

const currencies = ['USD', 'BAM', 'EUR', 'CHF', 'AUD'];
const historyFile = 'conversion_history.json';
const presetsFile = 'preset_values.json';
const helpFile = 'help_text.json';

const question = (query) => new Promise((resolve) => readline.question(query, resolve));


const sleep = (ms) => new Promise(resolve => setTimeout(resolve, ms));
const clearScreen = () => console.clear();

async function waitForUser() {
    await question("\nPress Enter to continue...");
    clearScreen();
}

async function loadingAnimation() {
    const spinnerChars = ['⠋', '⠙', '⠹', '⠸', '⠼', '⠴', '⠦', '⠧', '⠇', '⠏'];
    let spinnerIndex = 0;

    console.log("\n=== Initializing Currency Converter ===\n");

    for (const currency of currencies) {
        process.stdout.write(`Loading ${currency} `);
        for (let i = 0; i < 20; i++) {
            process.stdout.write(`\r${currency}: [${spinnerChars[spinnerIndex]}] Loading... `);
            spinnerIndex = (spinnerIndex + 1) % spinnerChars.length;
            await sleep(100);
        }
        process.stdout.write(`\r${currency}: [✓] Loaded successfully!   \n`);
    }

    console.log("\nSystem initialization complete!");
    console.log("Starting currency converter...\n");
    await sleep(1000);
    clearScreen();
}

function createHelpFile() {
    if (!fs.existsSync(helpFile)) {
        const helpContent = {
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
        };
        fs.writeFileSync(helpFile, JSON.stringify(helpContent, null, 2));
    }
}

function loadPresets() {
    try {
        return JSON.parse(fs.readFileSync(presetsFile, 'utf8'));
    } catch (error) {
        const presetRates = {
            'USD': {'BAM': 1.85, 'EUR': 0.92, 'CHF': 0.89, 'AUD': 1.53},
            'BAM': {'USD': 0.54, 'EUR': 0.51, 'CHF': 0.48, 'AUD': 0.83},
            'EUR': {'USD': 1.09, 'BAM': 1.96, 'CHF': 0.97, 'AUD': 1.67},
            'CHF': {'USD': 1.12, 'BAM': 2.08, 'EUR': 1.03, 'AUD': 1.72},
            'AUD': {'USD': 0.65, 'BAM': 1.21, 'EUR': 0.60, 'CHF': 0.58}
        };
        fs.writeFileSync(presetsFile, JSON.stringify(presetRates, null, 2));
        return presetRates;
    }
}

function showHelp() {
    try {
        const helpContent = JSON.parse(fs.readFileSync(helpFile, 'utf8'));
        
        console.log("\n=== Currency Converter Help ===");
        
        console.log(`\n${helpContent.conversion.title}`);
        console.log("\nSteps:");
        helpContent.conversion.steps.forEach(step => console.log(step));
        
        console.log("\nImportant Notes:");
        helpContent.conversion.notes.forEach(note => console.log(note));
        
        console.log("\n" + helpContent.conversion.example);
        
        console.log(`\n${helpContent.predictions.title}`);
        console.log("\nDaily Predictions:");
        helpContent.predictions.daily.forEach(info => console.log(`• ${info}`));
        
        console.log("\nHourly Predictions:");
        helpContent.predictions.hourly.forEach(info => console.log(`• ${info}`));
    } catch (error) {
        console.log("\nHelp information not available.");
    }
}

function saveConversion(fromCurr, toCurr, amount, result) {
    let history = [];
    try {
        history = JSON.parse(fs.readFileSync(historyFile, 'utf8'));
    } catch (error) {
    }

    history.push({
        date: new Date().toISOString().replace('T', ' ').slice(0, 19),
        from: fromCurr,
        to: toCurr,
        amount: amount,
        result: result
    });

    fs.writeFileSync(historyFile, JSON.stringify(history, null, 2));
}

function showPopularConversions(rates) {
    const commonPairs = [
        ['USD', 'BAM'],
        ['EUR', 'BAM'],
        ['USD', 'EUR'],
        ['CHF', 'EUR'],
        ['AUD', 'USD']
    ];
    
    const totalConversions = Math.floor(Math.random() * (1000 - 500) + 500);
    console.log("\n=== Most Used Conversions Today ===");
    console.log(`Total conversions today: ${totalConversions}\n`);
    console.log("Currency Pair     Current Rate    Times Used");
    console.log("-".repeat(45));
    
    commonPairs.forEach(([fromCurr, toCurr]) => {
        const conversions = Math.floor(totalConversions * (Math.random() * 0.3 + 0.05));
        const rate = rates[fromCurr][toCurr];
        console.log(`${fromCurr}/${toCurr.padEnd(8)}     ${rate.toFixed(4)}          ${conversions}`);
    });
}

function generatePredictions(rates) {
    const predictions = {};
    currencies.forEach(base => {
        predictions[base] = {};
        currencies.forEach(target => {
            if (base !== target) {
                const currentRate = rates[base][target];
                const variation = (Math.random() * 0.1) - 0.05;
                predictions[base][target] = Number((currentRate * (1 + variation)).toFixed(4));
            }
        });
    });
    return predictions;
}

function generateHourlyPredictions(rates, fromCurr, toCurr, hours) {
    const baseRate = rates[fromCurr][toCurr];
    const predictions = [];
    const currentHour = new Date();
    
    const volatility = 0.02;
    let prevVariation = 0;
    
    for (let i = 0; i < hours; i++) {
        const nextHour = new Date(currentHour);
        nextHour.setHours(currentHour.getHours() + i);
        
        const variation = (Math.random() * volatility * 2 - volatility) + (prevVariation * 0.3);
        const predictedRate = baseRate * (1 + variation);
        
        predictions.push({
            hour: nextHour.getHours().toString().padStart(2, '0') + ':00',
            rate: Number(predictedRate.toFixed(4)),
            change: Number((variation * 100).toFixed(2)),
            trend: variation > 0 ? '↑' : '↓'
        });
        
        prevVariation = variation;
    }
    
    return predictions;
}

async function handleHourlyPredictions(rates) {
    console.log("\nAvailable currencies:", currencies.join(', '));
    
    const fromCurr = (await question("Convert from (currency code): ")).toUpperCase();
    const toCurr = (await question("Convert to (currency code): ")).toUpperCase();
    
    if (!currencies.includes(fromCurr) || !currencies.includes(toCurr)) {
        console.log("Invalid currency code!");
        return;
    }
    
    let hours;
    while (true) {
        hours = parseInt(await question("\nEnter number of hours to predict (1, 3, or 6): "));
        if ([1, 3, 6].includes(hours)) break;
        console.log("Please enter either 1, 3, or 6 hours.");
    }
    
    const predictions = generateHourlyPredictions(rates, fromCurr, toCurr, hours);
    const currentRate = rates[fromCurr][toCurr];
    
    console.log(`\n=== ${fromCurr}/${toCurr} Predictions ===`);
    console.log(`Current rate: ${currentRate.toFixed(4)}`);
    console.log(`\nPredicted rates for next ${hours} hour(s):`);
    console.log("\nTime    Rate      Change   Trend");
    console.log("-".repeat(35));
    predictions.forEach(pred => {
        console.log(`${pred.hour}   ${pred.rate.toFixed(4)}   ${pred.change.toFixed(2).padStart(6)}%   ${pred.trend}`);
    });
}

async function handleConversion(rates) {
    console.log("\nAvailable currencies:", currencies.join(', '));
    
    const fromCurr = (await question("Convert from (currency code): ")).toUpperCase();
    const toCurr = (await question("Convert to (currency code): ")).toUpperCase();
    
    if (!currencies.includes(fromCurr) || !currencies.includes(toCurr)) {
        console.log("Invalid currency code!");
        return;
    }
    
    const amount = parseFloat(await question("Enter amount: "));
    if (isNaN(amount)) {
        console.log("Invalid amount!");
        return;
    }
    
    const result = amount * rates[fromCurr][toCurr];
    console.log(`\nResult: ${amount} ${fromCurr} = ${result.toFixed(2)} ${toCurr}`);
    saveConversion(fromCurr, toCurr, amount, result);
}

function handlePredictions(rates) {
    const predictions = generatePredictions(rates);
    console.log("\n=== Currency Predictions ===");
    Object.entries(predictions).forEach(([base, targets]) => {
        console.log(`\n${base} predictions:`);
        Object.entries(targets).forEach(([target, rate]) => {
            console.log(`${base} to ${target}: ${rate.toFixed(4)}`);
        });
    });
}

function handleHistory() {
    try {
        const history = JSON.parse(fs.readFileSync(historyFile, 'utf8'));
        if (history.length === 0) {
            console.log("\nNo conversion history found.");
        } else {
            console.log("\n=== Conversion History ===");
            history.forEach(entry => {
                console.log(`${entry.date}: ${entry.amount} ${entry.from} = ${entry.result.toFixed(2)} ${entry.to}`);
            });
        }
    } catch (error) {
        console.log("\nNo conversion history found.");
    }
}

function clearHistory() {
    fs.writeFileSync(historyFile, JSON.stringify([]));
}

async function showMenu() {
    clearScreen();
    console.log("=== Currency Converter Menu ===");
    console.log("1. Convert Currency");
    console.log("2. Hourly Predictions");
    console.log("3. Daily Predictions");
    console.log("4. Popular Conversions");
    console.log("5. View Conversion History");
    console.log("6. Clear History");
    console.log("7. Help");
    console.log("8. Exit");
    
    return await question("\nEnter your choice (1-8): ");
}

async function main() {
    try {
        await loadingAnimation();
        createHelpFile();
        const rates = loadPresets();
        
        while (true) {
            const choice = await showMenu();
            
            switch (choice) {
                case "1":
                    await handleConversion(rates);
                    break;
                case "2":
                    await handleHourlyPredictions(rates);
                    break;
                case "3":
                    handlePredictions(rates);
                    break;
                case "4":
                    showPopularConversions(rates);
                    break;
                case "5":
                    handleHistory();
                    break;
                case "6":
                    clearHistory();
                    console.log("\nHistory cleared successfully!");
                    break;
                case "7":
                    showHelp();
                    break;
                case "8":
                    console.log("\nThank you for using Currency Converter!");
                    readline.close();
                    process.exit(0);
                default:
                    console.log("\nInvalid choice! Please try again.");
            }
            await waitForUser();
        }
    } catch (error) {
        console.error("An error occurred:", error);
        readline.close();
    }
}

main().catch(console.error);
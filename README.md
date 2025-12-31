# P002: Currency Converter

A microservice built using the **cloops.microservices SDK** that converts an amount from one currency to another.  
It listens on the NATS subject `currency.convert` and uses the [ExchangeRate-API](https://www.exchangerate-api.com/) for conversion rates.

---

## 🚀 Features
- Listens on `currency.convert` subject via NATS.
- Converts between currencies using live exchange rates.
- Rounds converted amounts to 2 decimal places.
- Handles errors gracefully (invalid codes, API failures, negative amounts).
- Tested using the official NATS CLI.

---

## 📦 Setup

Clone the repo and restore dependencies:

```bash
git clone <your-repo-url>
cd currency-converter-service
dotnet restore CurrencyService
dotnet build CurrencyService

Run the Service
Start the service:
./run.sh


You should see:
CurrencyService listening on 'currency.convert'



🖧 Start NATS Server
Download and run the NATS Server:
nats-server -DV


Keep this terminal open while the service runs.

🧪 Testing with NATS CLI
Download the NATS CLI and run:
nats.exe request currency.convert "{\"amount\":100,\"from\":\"USD\",\"to\":\"EUR\"}"

Sample response:
{
  "amount": 85.10,
  "from": "USD",
  "to": "EUR",
  "rate": 0.851
}


⚠️ Error Cases
- Invalid currency code:
{ "error": "Rate not available" }
- Negative amount:
{ "error": "Invalid input" }



📂 Project Structure
currency-converter-service/
├── CurrencyService/          # Main microservice implementation
│   ├── Controllers/          # NATS handlers (CurrencyController)
│   ├── Services/             # ExchangeRateService and business logic
│   └── Program.cs            # Entry point
├── CurrencyService.Tests/    # Unit tests
├── run.sh                    # Script to run the service
└── README.md                 # Documentation


---



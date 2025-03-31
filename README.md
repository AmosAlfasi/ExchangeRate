# Exchange Rate Fetcher & API

This project consists of two services:

1. **RateFetcher**: A background worker that fetches real-time exchange rates and saves them to a file.
2. **ExchangeRateApi**: A REST API that exposes endpoints to retrieve the latest exchange rates.
   
## Endpoints
### **GET** /api/rate   
### **GET** /api/rate/{pairName}

## How to Run the Project

### Running Both Services from Visual Studio

1. Open the solution in Visual Studio.
2. Right-click on the solution in **Solution Explorer** and select **"Set Startup Projects..."**.
3. Set both `RateFetcher` and `ExchangeRateApi` to **"Start"**.
4. Press **F5** to run both services simultaneously.

### Running Both Services from Command Line

#### Option 1: Run in Separate Windows

1. Open a **Command Prompt** or **PowerShell** window.
2. Navigate to the `RateFetcher` project directory and run:

   ```
   dotnet run
3. Open another window and run the ExchangeRateApi:
   ```
   dotnet run

#### Option 2: Run in a Single Window

   ```
   start dotnet run --project Path\To\RateFetcher
   start dotnet run --project Path\To\ExchangeRateApi
   ```
# Enjoy :)

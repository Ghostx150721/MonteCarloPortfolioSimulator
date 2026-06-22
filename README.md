# Monte Carlo Portfolio Simulator

A professional financial simulation tool designed to help investors project the future value of their portfolios. By utilizing Monte Carlo methods, the simulator generates thousands of possible future scenarios, providing a probabilistic range of outcomes rather than a single linear projection.

## 🚀 Key Features

### Dual Simulation Models
- **GBM (Geometric Brownian Motion)**: A standard mathematical model for simulating asset prices, using mean annual returns and volatility.
- **Real-World Data**: Simulations based on actual historical monthly returns (e.g., S&P 500), capturing real-market patterns and distributions.

### Realistic Market Dynamics
- **Crash Simulation**: Models systemic market crashes with a configurable probability and impact, simulating "black swan" events and their prolonged recovery periods.
- **Monthly Contributions**: Accounts for regular periodic additions to the portfolio.

### Comprehensive Analytics
- **Path Visualization**: Generates full price paths for each simulation to visualize volatility and outcome dispersion.
- **Probabilistic Outcomes**: Calculates key percentiles (**P10, P50, P90**) to provide a realistic range of potential portfolio values.
- **Risk Metrics**: Computes the **Maximum Drawdown** for each simulation to help users understand the worst-case peak-to-trough declines.

## 🏗️ Architecture

The project follows a decoupled N-tier architecture:

- **`MonteCarlo.Web`**: An ASP.NET Core MVC web application providing a user-friendly interface for simulation configuration and interactive result visualization.
- **`MonteCarlo.Api`**: A RESTful API that serves as the bridge between the UI and the simulation engine.
- **`MonteCarlo.Core`**: The high-performance business logic layer containing the Monte Carlo engine, mathematical models, and data processing services.

## 🛠️ Tech Stack

- **Framework**: .NET 8
- **Language**: C#
- **Frontend**: ASP.NET Core MVC, HTML5, CSS3, JavaScript
- **API**: ASP.NET Core Web API
- **Simulation Logic**: Mathematical modeling (GBM) & Historical data analysis

## 🏁 Getting Started

### Prerequisites
- .NET 8 SDK
- A modern web browser

### Installation & Running
1. Clone the repository:
   ```bash
   git clone <repository-url>
   ```
2. Open the solution in Visual Studio or VS Code:
   ```bash
   dotnet sln MonteCarloPortfolioSimulator.sln
   ```
3. Run the `MonteCarlo.Api` project first.
4. Run the `MonteCarlo.Web` project.
5. Navigate to the local URL provided by the web application.

## 📈 How it Works

The simulator takes investment parameters (initial amount, monthly contribution, expected return, volatility) and runs $N$ simulations. 

For each simulation:
1. It iterates through the total number of months.
2. It determines if a "Crash" occurs based on the configured annual probability.
3. If in a normal state, it generates a monthly return using either the **GBM** formula or by sampling **Historical Data**.
4. If in a crash state, it applies a severe negative impact.
5. It updates the portfolio value and tracks the path to calculate the maximum drawdown.
6. Finally, it aggregates the results of all $N$ simulations to produce the probabilistic distribution.

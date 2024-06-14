# Dotnet Spice SDK

Dotnet SDK for Spice.ai

## Installation

Add Spice SDK

[//]: # (update when published to nuget)
```bash 
```

## Usage

<!-- NOTE: If you're changing the code examples below, make sure you update `tests/readme_test.rs`. -->

### Usage with locally running [spice runtime](https://github.com/spiceai/spiceai)

Follow the [quickstart guide](https://github.com/spiceai/spiceai?tab=readme-ov-file#%EF%B8%8F-quickstart-local-machine) to install and run spice locally

```csharp
using Spice;

var client = new SpiceClientBuilder().Build();
```

### New client with https://spice.ai cloud

```csharp
using Spice;

var client = new SpiceClientBuilder()
            .WithSpiceCloud("API_KEY")
            .Build();
```

### Arrow Query

SQL Query

```csharp
using Spice;

var client = new SpiceClientBuilder()
            .WithSpiceCloud("API_KEY")
            .Build();

var data = await client.Query("SELECT * FROM eth.recent_blocks LIMIT 10;");
```

## Documentation

Check out our [Documentation](https://docs.spice.ai/sdks/dotnet-sdk) to learn more about how to use the Dotnet SDK.

# Todos

This repository is a sample application built using ASP.NET Core 8 and React.

## Requirements

This repository requires the following software to be installed:

- [.NET SDK 9.0](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
- [Node.js](https://nodejs.org/en/download)

## Running the code

The code in this repository is composed by a backend application written using ASP.NET Core 9.0 and a frontend application written using React.

The repository uses Aspire to start all the needed components for the application.

You can launch the whole application using the following command:

```bash
$ dotnet run --project ./tools/AppHost

Using launch settings from .\tools\AppHost\Properties\launchSettings.json...
Building...
info: Aspire.Hosting.DistributedApplication[0]
      Aspire version: 9.2.1+b590865a294feaff82f06c4fadef62ba1fad2271
info: Aspire.Hosting.DistributedApplication[0]
      Distributed application starting.
info: Aspire.Hosting.DistributedApplication[0]
      Application host directory is: C:\Users\rg1844\Development\Labs\Todos\tools\AppHost
info: Aspire.Hosting.DistributedApplication[0]
      Now listening on: https://localhost:17047
info: Aspire.Hosting.DistributedApplication[0]
      Login to the dashboard at https://localhost:17047/login?t=0d7214b0cdc7b04be2a1d7185fdcc2b3
info: Aspire.Hosting.DistributedApplication[0]
      Distributed application started. Press Ctrl+C to shut down.
```

The first thing we want to look at is the Aspire dashboard. You can reach it by clicking the link in the application log.

In the sample above, you want to click <https://localhost:17047/login?t=0d7214b0cdc7b04be2a1d7185fdcc2b3> to open the dashboard.

Please note that before you can launch the application, you need to restore the dependencies of the client application. You can do this by running the following command:

```bash
$ cd ./client
$ npm install
```

This will install all the dependencies needed to run the client application.

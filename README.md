# Todos

This repository is a sample application built using ASP.NET Core 8 and React.

## Requirements

This repository requires the following software to be installed:

- [.NET SDK 9.0](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
- [Node.js](https://nodejs.org/en/download)
- [Docker](https://www.docker.com/)

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

You can then navigate to:

- Client: <http://localhost:3182/>
- Api: <http://localhost:5182>

Please note that before you can launch the application, you need to restore the dependencies of the client application. You can do this by running the following command:

```bash
$ cd ./client
$ npm install
```

This will install all the dependencies needed to run the client application.

## Testing

The solution includes three test projects:

- `API.Tests` is a unit test project
- `API.Integration.Tests` is a test project built using the `WebApplicationFactory` to test the whole ASP.NET Core application. It uses `TestContainers` and `Respawn` to give each test a pristine database.
- `EndToEnd.Tests` is a test project built using [Playwright](https://playwright.dev/dotnet/) and [Aspire Testing Hosting](https://devblogs.microsoft.com/dotnet/getting-started-with-testing-and-dotnet-aspire/).

All test projects are built using:

- [NUnit 3](https://nunit.org/)
- [FakeItEasy](https://github.com/FakeItEasy/FakeItEasy)
- [AutoFixture](https://github.com/AutoFixture/AutoFixture)

Additional testing packages are:

- [Testcontainers for .NET](https://dotnet.testcontainers.org/)
- [Respawn](https://github.com/jbogard/Respawn)

Note, to run the Playwright test, you need to install the browsers. You can do so by running the following commands in a Powershell console.

```bash
$ dotnet restore
$ ./e2e/EndToEnd.Tests/bin/Debug/net9.0/playwright.ps1 install
```

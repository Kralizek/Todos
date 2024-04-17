# Todos

This repository is a sample application built using ASP.NET Core 8 and React.

## Requirements

This repository requires the following software to be installed:

- [.NET SDK 8.0](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)

## Running the code

The code in this repository is composed by a backend application written using ASP.NET Core 8.0 and a frontend application written using React.

### Backend

To execute the backend, simply run the following command from a terminal in the root of the repository

```bash
$ dotnet run --project ./backend/API
```

Alternatively,

If you want the application to restart automatically when the source code is changed, use

```bash
$ dotnet watch --project ./backend/API run
```

The backend is configured to bind port 5182. Make sure the port is available.

Once the project is running you can navigate to [http://localhost:5182] to access the Swagger interface.

The backend project comes with a `API.http` file that shows an example of each valid request.

### Frontend

This repository requires the following software to be installed:

 [.Node.js](https://nodejs.org/en/download)

This project was bootstrapped with [Create React App](https://github.com/facebook/create-react-app).

## Available Scripts

In the project directory (./client/), you can run following commands :

### `npm install`

### `npm start`

Runs the app in the development mode.\
Open [http://localhost:3000](http://localhost:3000) to view it in your browser.

The page will reload when you make changes.\
You may also see any lint errors in the console.


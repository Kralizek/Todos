FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY *.sln .
COPY backend/API/*.csproj ./backend/API/
COPY backend/API.Integration.Tests/*.csproj ./backend/API.Integration.Tests/
COPY backend/API.Tests/*.csproj ./backend/API.Tests/
COPY backend/ServiceDefaults/*.csproj ./backend/ServiceDefaults/
COPY e2e/EndToEnd.Tests/*.csproj ./e2e/EndToEnd.Tests/
COPY tools/AppHost/*.csproj ./tools/AppHost/
# If you have other projects in the backend folder that are part of the solution, copy their .csproj files too
# e.g., COPY backend/ServiceDefaults/*.csproj ./backend/ServiceDefaults/
RUN dotnet restore ./Todos.sln

# copy everything else and build app
COPY backend/ ./backend/
WORKDIR /source/backend/API
RUN dotnet publish -c release -o /app --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "API.dll"] # Assumes your main backend project output is API.dll 
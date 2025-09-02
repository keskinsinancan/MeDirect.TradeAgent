# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and csproj files
COPY ["MeDirect.TradeAgent.sln", "./"]
COPY ["TradeAgent.API/TradeAgent.API.csproj", "TradeAgent.API/"]
COPY ["TradeAgent.Application/TradeAgent.Application.csproj", "TradeAgent.Application/"]
COPY ["TradeAgent.Infrastructure/TradeAgent.Infrastructure.csproj", "TradeAgent.Infrastructure/"]
COPY ["TradeAgent.Domain/TradeAgent.Domain.csproj", "TradeAgent.Domain/"]
COPY ["TradeAgent.Logging/TradeAgent.Logging.csproj", "TradeAgent.Logging/"]

# Restore dependencies
RUN dotnet restore "TradeAgent.API/TradeAgent.API.csproj"

# Copy everything else and publish
COPY . .
WORKDIR "/src/TradeAgent.API"
RUN dotnet publish -c Release -o /app/out

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out ./
ENTRYPOINT ["dotnet", "TradeAgent.API.dll"]

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy solution and csproj
COPY ["MeDirect.TradeAgent.sln", "./"]
COPY ["TradeAgent.Consumer/TradeAgent.Consumer.csproj", "TradeAgent.Consumer/"]
COPY ["TradeAgent.Logging/TradeAgent.Logging.csproj", "TradeAgent.Logging/"]

# Restore
RUN dotnet restore "TradeAgent.Consumer/TradeAgent.Consumer.csproj"

# Copy everything
COPY . .

# Publish self-contained Linux-x64
RUN dotnet publish "TradeAgent.Consumer/TradeAgent.Consumer.csproj" -c Release -r linux-x64 --self-contained true -o /app/out

# Runtime stage
FROM mcr.microsoft.com/dotnet/runtime-deps:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./

ENTRYPOINT ["./TradeAgent.Consumer"]

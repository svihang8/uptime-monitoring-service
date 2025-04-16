# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy only what's needed to build the dashboard
COPY uptime-monitoring-service.sln ./
COPY UptimeMonitoringDashboard/UptimeMonitoringDashboard.csproj UptimeMonitoringDashboard/

RUN dotnet restore UptimeMonitoringDashboard/UptimeMonitoringDashboard.csproj

# Copy the rest of the dashboard source code
COPY UptimeMonitoringDashboard/ UptimeMonitoringDashboard/

# Build the dashboard
WORKDIR /src/UptimeMonitoringDashboard
RUN dotnet build -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

# Final stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Development

ENTRYPOINT ["dotnet", "UptimeMonitoringDashboard.dll"]

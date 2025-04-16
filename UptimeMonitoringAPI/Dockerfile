# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy the solution file and restore dependencies
COPY ["uptime-monitoring-service.sln", "./"]
COPY ["UptimeMonitoringAPI/UptimeMonitoringAPI.csproj", "UptimeMonitoringAPI/"]
RUN dotnet restore

# Copy the rest of the code
COPY . .

# Build the application
WORKDIR "/src/UptimeMonitoringAPI"
RUN dotnet build "UptimeMonitoringAPI.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "UptimeMonitoringAPI.csproj" -c Release -o /app/publish

# Final stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Expose HTTP only
EXPOSE 80

# Set environment variables
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Development

# Start the application
ENTRYPOINT ["dotnet", "UptimeMonitoringAPI.dll"]

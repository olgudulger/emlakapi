# Use the official .NET SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

# Set the working directory
WORKDIR /app

# Copy solution file
COPY EmlakTakip.sln ./

# Copy project files
COPY Entity/Entity.csproj ./Entity/
COPY Business/Business.csproj ./Business/
COPY DataAccess/DataAccess.csproj ./DataAccess/
COPY WebAPI/WebAPI.csproj ./WebAPI/

# Restore dependencies
RUN dotnet restore

# Copy all source code
COPY . .

# Build and publish the application
RUN dotnet publish WebAPI/WebAPI.csproj -c Release -o /app/publish

# Use the official .NET runtime image for running
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime

# Set the working directory
WORKDIR /app

# Copy the published output from the build stage
COPY --from=build /app/publish .

# Create a non-root user
RUN adduser --disabled-password --gecos '' appuser && chown -R appuser /app
USER appuser

# Expose the port that the application listens on
EXPOSE 8080

# Set environment variables
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080

# Run the application
ENTRYPOINT ["dotnet", "WebAPI.dll"] 
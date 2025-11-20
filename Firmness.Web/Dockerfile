#  Base Image 
# This stage sets up the runtime environment (Linux) for the application.
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

#  Build Image (SDK)
# This stage uses the full .NET SDK to build the application.
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# CRITICAL FIX: Copy ALL project files 
# I must copy the csproj files for all layers so 'dotnet restore' can resolve dependencies.
COPY ["Firmness.Web/Firmness.Web.csproj", "Firmness.Web/"]
COPY ["Firmness.Application/Firmness.Application.csproj", "Firmness.Application/"]
COPY ["Firmness.Domain/Firmness.Domain.csproj", "Firmness.Domain/"]
COPY ["Firmness.Infraestructure/Firmness.Infraestructure.csproj", "Firmness.Infraestructure/"]

# Restore dependencies for the main web project
RUN dotnet restore "Firmness.Web/Firmness.Web.csproj"

# Copy the rest of the source code into the container
COPY . .

# Switch to the Web project folder to build it
WORKDIR "/src/Firmness.Web"
RUN dotnet build "./Firmness.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

#  Publish Stage
# This publishes the app to a folder, optimizing it for production.
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Firmness.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final Stage
# This creates the final, lightweight image with the published app.
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Firmness.Web.dll"]

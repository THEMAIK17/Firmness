
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

# dependencies for the pdf
USER root
RUN apt-get update \
    && apt-get install -y --no-install-recommends \
       libfontconfig1 \
       libfreetype6 \
       libxml2 \
    && rm -rf /var/lib/apt/lists/* \
    
USER $APP_UID
USER root
RUN apt-get update \
    && apt-get install -y --no-install-recommends \
       libfontconfig1 \
       libfreetype6 \
       libxml2 \
    && rm -rf /var/lib/apt/lists/*

USER $APP_UID


FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src


COPY ["Firmness.Api/Firmness.Api.csproj", "Firmness.Api/"]
COPY ["Firmness.Application/Firmness.Application.csproj", "Firmness.Application/"]
COPY ["Firmness.Domain/Firmness.Domain.csproj", "Firmness.Domain/"]
COPY ["Firmness.Infraestructure/Firmness.Infraestructure.csproj", "Firmness.Infraestructure/"]


RUN dotnet restore "Firmness.Api/Firmness.Api.csproj"


COPY . .

WORKDIR "/src/Firmness.Api"
RUN dotnet build "Firmness.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build


FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Firmness.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false


FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Firmness.Api.dll"]
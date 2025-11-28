
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Firmness.Tests/Firmness.Tests.csproj", "Firmness.Tests/"]
COPY ["Firmness.Web/Firmness.Web.csproj", "Firmness.Web/"]
COPY ["Firmness.Api/Firmness.Api.csproj", "Firmness.Api/"]
COPY ["Firmness.Application/Firmness.Application.csproj", "Firmness.Application/"]
COPY ["Firmness.Domain/Firmness.Domain.csproj", "Firmness.Domain/"]
COPY ["Firmness.Infraestructure/Firmness.Infraestructure.csproj", "Firmness.Infraestructure/"]

RUN dotnet restore "Firmness.Tests/Firmness.Tests.csproj"

COPY . .
WORKDIR "/src/Firmness.Tests"

ENTRYPOINT ["dotnet", "test", "--logger:trx"]
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
ARG GHPAT
ARG VERSION
WORKDIR /src
COPY ["Texnokaktus.ProgOlymp.Api/Texnokaktus.ProgOlymp.Api.csproj", "Texnokaktus.ProgOlymp.Api/"]
COPY ["Texnokaktus.ProgOlymp.Api.DataAccess/Texnokaktus.ProgOlymp.Api.DataAccess.csproj", "Texnokaktus.ProgOlymp.Api.DataAccess/"]
COPY ["Texnokaktus.ProgOlymp.Api.Logic/Texnokaktus.ProgOlymp.Api.Logic.csproj", "Texnokaktus.ProgOlymp.Api.Logic/"]
COPY ["Texnokaktus.ProgOlymp.Api.Domain/Texnokaktus.ProgOlymp.Api.Domain.csproj", "Texnokaktus.ProgOlymp.Api.Domain/"]
COPY ["Texnokaktus.ProgOlymp.Api.Infrastructure/Texnokaktus.ProgOlymp.Api.Infrastructure.csproj", "Texnokaktus.ProgOlymp.Api.Infrastructure/"]
COPY ["modules/Common.Contracts/Texnokaktus.ProgOlymp.Common.Contracts/Texnokaktus.ProgOlymp.Common.Contracts.csproj", "modules/Common.Contracts/Texnokaktus.ProgOlymp.Common.Contracts/"]
RUN dotnet nuget add source --username kav128 --password $GHPAT --store-password-in-clear-text --name "GitHub Texnokaktus-Progolymp" https://nuget.pkg.github.com/Texnokaktus-ProgOlymp/index.json
RUN dotnet restore "Texnokaktus.ProgOlymp.Api/Texnokaktus.ProgOlymp.Api.csproj"
COPY . .
WORKDIR "/src/Texnokaktus.ProgOlymp.Api"
RUN dotnet build "Texnokaktus.ProgOlymp.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Texnokaktus.ProgOlymp.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false /p:Version=$VERSION

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Texnokaktus.ProgOlymp.Api.dll"]

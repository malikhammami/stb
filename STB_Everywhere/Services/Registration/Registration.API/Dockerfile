#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Services/Registration/Registration.API/Registration.API.csproj", "Services/Registration/Registration.API/"]
COPY ["BuildingBlocks/Common.Logging/Common.Logging.csproj", "BuildingBlocks/Common.Logging/"]
COPY ["Services/Registration/Registration.Application/Registration.Application.csproj", "Services/Registration/Registration.Application/"]
COPY ["Services/Registration/Registration.Domain/Registration.Domain.csproj", "Services/Registration/Registration.Domain/"]
COPY ["Services/Registration/Registration.Infrastructure/Registration.Infrastructure.csproj", "Services/Registration/Registration.Infrastructure/"]
RUN dotnet restore "./Services/Registration/Registration.API/./Registration.API.csproj"
COPY . .
WORKDIR "/src/Services/Registration/Registration.API"
RUN dotnet build "./Registration.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Registration.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Registration.API.dll"]
# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
# For more information, please see https://aka.ms/containercompat

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0-nanoserver-1809 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0-nanoserver-1809 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["1-SPR25_SWD392_ClothingCustomization/1-SPR25_SWD392_ClothingCustomization.csproj", "1-SPR25_SWD392_ClothingCustomization/"]
COPY ["2-Service/2-Service.csproj", "2-Service/"]
COPY ["3-Repository/3-Repository.csproj", "3-Repository/"]
COPY ["4-BusinessObject/4-BusinessObject.csproj", "4-BusinessObject/"]
RUN dotnet restore "./1-SPR25_SWD392_ClothingCustomization/1-SPR25_SWD392_ClothingCustomization.csproj"
COPY . .
WORKDIR "/src/1-SPR25_SWD392_ClothingCustomization"
RUN dotnet build "./1-SPR25_SWD392_ClothingCustomization.csproj" -c %BUILD_CONFIGURATION% -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./1-SPR25_SWD392_ClothingCustomization.csproj" -c %BUILD_CONFIGURATION% -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "1-SPR25_SWD392_ClothingCustomization.dll"]
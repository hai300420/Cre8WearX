# Use Linux-based .NET 8 SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files
COPY ["1-SPR25_SWD392_ClothingCustomization/1-SPR25_SWD392_ClothingCustomization.csproj", "1-SPR25_SWD392_ClothingCustomization/"]
COPY ["2-Service/2-Service.csproj", "2-Service/"]
COPY ["3-Repository/3-Repository.csproj", "3-Repository/"]
COPY ["4-BusinessObject/4-BusinessObject.csproj", "4-BusinessObject/"]

# Restore dependencies
RUN dotnet restore "1-SPR25_SWD392_ClothingCustomization/1-SPR25_SWD392_ClothingCustomization.csproj"

# Copy the rest of the code
COPY . .

# Build the application
WORKDIR "/src/1-SPR25_SWD392_ClothingCustomization"
RUN dotnet build "1-SPR25_SWD392_ClothingCustomization.csproj" -c Release -o /app/build

# Publish the application
RUN dotnet publish "1-SPR25_SWD392_ClothingCustomization.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final runtime image using Linux-based ASP.NET image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 80
EXPOSE 443
ENTRYPOINT ["dotnet", "1-SPR25_SWD392_ClothingCustomization.dll"]

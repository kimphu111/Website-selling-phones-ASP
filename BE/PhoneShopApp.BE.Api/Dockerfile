# Sử dụng base image cho runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

# Build image
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy các file .csproj để restore packages trước (giúp tận dụng Docker cache)
COPY ["BE/PhoneShopApp.BE.Api/PhoneShopApp.BE.Api.csproj", "BE/PhoneShopApp.BE.Api/"]
COPY ["BE/PhoneShopApp.BE.Core/PhoneShopApp.BE.Core.csproj", "BE/PhoneShopApp.BE.Core/"]
COPY ["BE/PhoneShopApp.BE.Infrastructure/PhoneShopApp.BE.Infrastructure.csproj", "BE/PhoneShopApp.BE.Infrastructure/"]
COPY ["BE/PhoneShopApp.BE.Services/PhoneShopApp.BE.Services.csproj", "BE/PhoneShopApp.BE.Services/"]

RUN dotnet restore "BE/PhoneShopApp.BE.Api/PhoneShopApp.BE.Api.csproj"

# Copy toàn bộ source code và tiến hành build
COPY . .
WORKDIR "/src/BE/PhoneShopApp.BE.Api"
RUN dotnet build "PhoneShopApp.BE.Api.csproj" -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "PhoneShopApp.BE.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Chạy app
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PhoneShopApp.BE.Api.dll"]
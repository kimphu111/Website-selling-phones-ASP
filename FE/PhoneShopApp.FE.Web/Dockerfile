FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["FE/PhoneShopApp.FE.Web/PhoneShopApp.FE.Web.csproj", "FE/PhoneShopApp.FE.Web/"]
RUN dotnet restore "FE/PhoneShopApp.FE.Web/PhoneShopApp.FE.Web.csproj"

COPY . .
WORKDIR "/src/FE/PhoneShopApp.FE.Web"
RUN dotnet build "PhoneShopApp.FE.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PhoneShopApp.FE.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PhoneShopApp.FE.Web.dll"]
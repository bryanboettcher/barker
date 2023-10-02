FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Pingring.Barker.Web/Pingring.Barker.Web.csproj", "Pingring.Barker.Web/"]
RUN dotnet restore "Pingring.Barker.Web/Pingring.Barker.Web.csproj"
COPY . .
WORKDIR "/src/Pingring.Barker.Web"
RUN dotnet build "Pingring.Barker.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Pingring.Barker.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Pingring.Barker.Web.dll"]
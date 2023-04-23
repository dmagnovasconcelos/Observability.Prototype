#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Observability.Prototype.Api/Observability.Prototype.Api.csproj", "Observability.Prototype.Api/"]
RUN dotnet restore "Observability.Prototype.Api/Observability.Prototype.Api.csproj"
COPY . .
WORKDIR "/src/Observability.Prototype.Api"
RUN dotnet build "Observability.Prototype.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Observability.Prototype.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Observability.Prototype.Api.dll"]
CMD ["dotnet-monitor collect --urls http://0.0.0.0:52323 --metricUrls http://0.0.0.0:52325 --no-auth"]
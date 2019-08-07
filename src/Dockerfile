FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["HostedService.csproj", "./"]
RUN dotnet restore "HostedService.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "HostedService.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "HostedService.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "HostedService.dll"]

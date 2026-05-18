FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/PayrollManagementSystem.API/PayrollManagementSystem.API.csproj", "src/PayrollManagementSystem.API/"]
COPY ["src/PayrollManagementSystem.Application/PayrollManagementSystem.Application.csproj", "src/PayrollManagementSystem.Application/"]
COPY ["src/PayrollManagementSystem.Domain/PayrollManagementSystem.Domain.csproj", "src/PayrollManagementSystem.Domain/"]
COPY ["src/PayrollManagementSystem.Infrastructure/PayrollManagementSystem.Infrastructure.csproj", "src/PayrollManagementSystem.Infrastructure/"]

RUN dotnet restore "src/PayrollManagementSystem.API/PayrollManagementSystem.API.csproj"

COPY . .

WORKDIR "/src/src/PayrollManagementSystem.API"

RUN dotnet build "PayrollManagementSystem.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PayrollManagementSystem.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 8080

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "PayrollManagementSystem.API.dll"]
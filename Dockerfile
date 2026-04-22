FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY Frete.Api/Frete.Api.csproj Frete.Api/
COPY Frete.Application/Frete.Application.csproj Frete.Application/
COPY Frete.Domain/Frete.Domain.csproj Frete.Domain/
COPY Frete.Infrastructure/Frete.Infrastructure.csproj Frete.Infrastructure/

RUN dotnet restore Frete.Api/Frete.Api.csproj

COPY . .
RUN dotnet publish Frete.Api/Frete.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "Frete.Api.dll"]

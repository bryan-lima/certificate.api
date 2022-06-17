#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/Certificates.Services.Api/Certificates.Services.Api.csproj", "src/Certificates.Services.Api/"]
RUN dotnet restore "src/Certificates.Services.Api/Certificates.Services.Api.csproj"
COPY . .
WORKDIR "/src/src/Certificates.Services.Api"
RUN dotnet build "Certificates.Services.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Certificates.Services.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Certificates.Services.Api.dll"]
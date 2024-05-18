#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/Certificate.Services.Api/Certificate.Services.Api.csproj", "src/Certificate.Services.Api/"]
RUN dotnet restore "src/Certificate.Services.Api/Certificate.Services.Api.csproj"
COPY . .
WORKDIR "/src/src/Certificate.Services.Api"
RUN dotnet build "Certificate.Services.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Certificate.Services.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Certificate.Services.Api.dll"]

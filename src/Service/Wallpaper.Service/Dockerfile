#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
#COPY ["Service/Wallpaper.Service/Wallpaper.Service.csproj", "Service/Wallpaper.Service/"]
#COPY ["Wallpaper.Common/Wallpaper.Common.csproj", "Wallpaper.Common/"]
COPY . .
RUN dotnet restore "Service/Wallpaper.Service/Wallpaper.Service.csproj"
WORKDIR "/src/Service/Wallpaper.Service"
RUN dotnet build "Wallpaper.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Wallpaper.Service.csproj" -c Release -o /app/publish

FROM base AS final
RUN mkdir /logs
RUN mkdir /configuration
VOLUME /logs /configuration
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Wallpaper.Service.dll"]
#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
#FastReport need
RUN apt-get update && apt-get install -y libgdiplus
WORKDIR /app
EXPOSE 4200
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["src/Wimi.BtlCore.Web.Mvc/NuGet.Config","src/Wimi.BtlCore.Web.Mvc/"]
COPY ["src/Wimi.BtlCore.Web.Mvc/Wimi.BtlCore.Web.Mvc.csproj", "src/Wimi.BtlCore.Web.Mvc/"]
COPY ["src/Wimi.BtlCore.Web.Core/Wimi.BtlCore.Web.Core.csproj", "src/Wimi.BtlCore.Web.Core/"]
COPY ["src/Wimi.BtlCore.Core/Wimi.BtlCore.Core.csproj", "src/Wimi.BtlCore.Core/"]
COPY ["src/Wimi.BtlCore.Application/Wimi.BtlCore.Application.csproj", "src/Wimi.BtlCore.Application/"]
COPY ["src/Wimi.BtlCore.EntityFrameworkCore/Wimi.BtlCore.EntityFrameworkCore.csproj", "src/Wimi.BtlCore.EntityFrameworkCore/"]
RUN dotnet restore "src/Wimi.BtlCore.Web.Mvc/Wimi.BtlCore.Web.Mvc.csproj" --configfile "src/Wimi.BtlCore.Web.Mvc/NuGet.Config"
COPY . .
WORKDIR "/src/src/Wimi.BtlCore.Web.Mvc"
RUN dotnet build "Wimi.BtlCore.Web.Mvc.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Wimi.BtlCore.Web.Mvc.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Wimi.BtlCore.Web.Mvc.dll"]
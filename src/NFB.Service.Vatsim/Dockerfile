#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["src/NFB.Service.Vatsim/NFB.Service.Vatsim.csproj", "src/NFB.Service.Vatsim/"]
COPY ["src/NFB.Domain/NFB.Domain.csproj", "src/NFB.Domain/"]
COPY ["src/NFB.Infrastructure.CrossCutting/NFB.Infrastructure.CrossCutting.csproj", "src/NFB.Infrastructure.CrossCutting/"]
RUN dotnet restore "src/NFB.Service.Vatsim/NFB.Service.Vatsim.csproj"
COPY . .
WORKDIR "/src/src/NFB.Service.Vatsim"
RUN dotnet build "NFB.Service.Vatsim.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "NFB.Service.Vatsim.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NFB.Service.Vatsim.dll"]
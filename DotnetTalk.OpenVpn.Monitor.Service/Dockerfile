#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["DotnetTalk.OpenVpn.Telnet.Service/DotnetTalk.OpenVpn.Telnet.Service.csproj", "DotnetTalk.OpenVpn.Telnet.Service/"]
RUN dotnet restore "DotnetTalk.OpenVpn.Telnet.Service/DotnetTalk.OpenVpn.Telnet.Service.csproj"
COPY . .
WORKDIR "/src/DotnetTalk.OpenVpn.Telnet.Service"
RUN dotnet build "DotnetTalk.OpenVpn.Telnet.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DotnetTalk.OpenVpn.Telnet.Service.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DotnetTalk.OpenVpn.Telnet.Service.dll"]
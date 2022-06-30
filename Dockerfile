#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 1234
EXPOSE 1235

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["PosStoneNfce.API.Portal.csproj", "PosStoneNfce.API.Portal/"]
RUN dotnet restore "PosStoneNfce.API.Portal/PosStoneNfce.API.Portal.csproj"
COPY . .
WORKDIR "/src/PosStoneNfce.API.Portal"
RUN dotnet build "PosStoneNfce.API.Portal.csproj" -c Release -o /app/build -p:StartupObject=PosStoneNfce.DockerProgram

FROM build AS publish
RUN dotnet publish "PosStoneNfce.API.Portal.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "run", "PosStoneNfce.API.Portal"]
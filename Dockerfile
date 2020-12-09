# remove the build stage
FROM mcr.microsoft.com/dotnet/sdk:5.0-alpine AS publish
WORKDIR /workdir

COPY src/MOnGoL.Common/MOnGoL.Common.csproj src/MOnGoL.Common/MOnGoL.Common.csproj
RUN dotnet restore src/MOnGoL.Common/MOnGoL.Common.csproj
RUN dotnet build src/MOnGoL.Common/MOnGoL.Common.csproj --no-restore -c Release

COPY src/MOnGoL.Client/MOnGoL.Client.csproj src/MOnGoL.Client/MOnGoL.Client.csproj
RUN dotnet restore src/MOnGoL.Client/MOnGoL.Client.csproj
RUN dotnet build src/MOnGoL.Client/MOnGoL.Client.csproj --no-restore -c Release

COPY src/MOnGoL.Server/MOnGoL.Server.csproj src/MOnGoL.Server/MOnGoL.Server.csproj
RUN dotnet restore src/MOnGoL.Server/MOnGoL.Server.csproj
RUN dotnet build src/MOnGoL.Server/MOnGoL.Server.csproj --no-restore -c Release

COPY src/MOnGoL.Backend/MOnGoL.Backend.csproj src/MOnGoL.Backend/MOnGoL.Backend.csproj
RUN dotnet restore src/MOnGoL.Backend/MOnGoL.Backend.csproj
RUN dotnet build src/MOnGoL.Backend/MOnGoL.Backend.csproj --no-restore -c Release

COPY src/MOnGoL.Frontend/MOnGoL.Frontend.csproj src/MOnGoL.Frontend/MOnGoL.Frontend.csproj
RUN dotnet restore src/MOnGoL.Frontend/MOnGoL.Frontend.csproj
RUN dotnet build src/MOnGoL.Frontend/MOnGoL.Frontend.csproj --no-restore -c Release

COPY src/MOnGoL.ServerSide/MOnGoL.ServerSide.csproj src/MOnGoL.ServerSide/MOnGoL.ServerSide.csproj
RUN dotnet restore src/MOnGoL.ServerSide/MOnGoL.ServerSide.csproj
RUN dotnet build src/MOnGoL.ServerSide/MOnGoL.ServerSide.csproj --no-restore -c Release

COPY src/MOnGoL.WebAssembly/MOnGoL.WebAssembly.csproj src/MOnGoL.WebAssembly/MOnGoL.WebAssembly.csproj
RUN dotnet restore src/MOnGoL.WebAssembly/MOnGoL.WebAssembly.csproj
RUN dotnet build src/MOnGoL.WebAssembly/MOnGoL.WebAssembly.csproj -c Release

COPY src/MOnGoL.Host.Combi/MOnGoL.Host.Combi.csproj src/MOnGoL.Host.Combi/MOnGoL.Host.Combi.csproj
RUN dotnet restore src/MOnGoL.Host.Combi/MOnGoL.Host.Combi.csproj
RUN dotnet build src/MOnGoL.Host.Combi/MOnGoL.Host.Combi.csproj --no-restore -c Release

# add --no-restore flag to dotnet publish
RUN dotnet publish src/MOnGoL.Host.Combi/MOnGoL.Host.Combi.csproj -c Release -o /app/publish --no-build
#  /p:PublishTrimmed=true

FROM mcr.microsoft.com/dotnet/aspnet:5.0-alpine as final
EXPOSE 80
EXPOSE 443
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet MOnGoL.Host.Combi.dll"]

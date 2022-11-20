FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Services/BackendForFrontend/BackendForFrontend.csproj", "Services/BackendForFrontend/"]
COPY ["Core/Core/Core.csproj", "Core/Core/"]
RUN dotnet restore "Services/BackendForFrontend/BackendForFrontend.csproj"
COPY . .
WORKDIR "/src/Services/BackendForFrontend"
RUN dotnet build "BackendForFrontend.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BackendForFrontend.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BackendForFrontend.dll"]

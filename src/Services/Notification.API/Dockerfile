FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Services/Notification.API/Notification.API.csproj", "Services/Notification.API/"]
COPY ["Core/Core/Core.csproj", "Core/Core/"]
RUN dotnet restore "Services/Notification.API/Notification.API.csproj"
COPY . .
WORKDIR "/src/Services/Notification.API"
RUN dotnet build "Notification.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Notification.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Notification.API.dll"]

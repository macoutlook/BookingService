FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Service/Service.csproj", "Service/"]
COPY ["Bootstrapper/Bootstrapper.csproj", "Bootstrapper/"]
COPY ["Core.Application.Contract/Core.Application.Contract.csproj", "Core.Application.Contract/"]
COPY ["Core.Domain/Core.Domain.csproj", "Core.Domain/"]
COPY ["Core.Application/Core.Application.csproj", "Core.Application/"]
COPY ["Core.Persistence.Contract/Core.Persistence.Contract.csproj", "Core.Persistence.Contract/"]
COPY ["Persistence.Repository/Persistence.BookRepository.csproj", "Persistence.Repository/"]
COPY ["Core.Exceptions/Core.Exceptions.csproj", "Core.Exceptions/"]
RUN dotnet restore "Service/Service.csproj"
COPY . .
WORKDIR "/src/Service"
RUN dotnet build "Service.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Service.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Service.dll"]

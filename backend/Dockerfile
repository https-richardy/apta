# use ASP.NET Core 10.0 runtime image (alpine) as base
FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS base
WORKDIR /artifacts
EXPOSE 8080

# use SDK image (Alpine) for build
FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
WORKDIR /workdir

ARG BUILD_CONFIGURATION=Release
ARG SOLUTION=Apta

# copy only sln and csproj files to restore as early as possible
COPY ["${SOLUTION}.sln", "./"]
COPY ["Source/${SOLUTION}.Domain/${SOLUTION}.Domain.csproj", "Source/${SOLUTION}.Domain/"]
COPY ["Source/${SOLUTION}.Application/${SOLUTION}.Application.csproj", "Source/${SOLUTION}.Application/"]
COPY ["Source/${SOLUTION}.Infrastructure/${SOLUTION}.Infrastructure.csproj", "Source/${SOLUTION}.Infrastructure/"]
COPY ["Source/${SOLUTION}.Infrastructure.IoC/${SOLUTION}.Infrastructure.IoC.csproj", "Source/${SOLUTION}.Infrastructure.IoC/"]
COPY ["Source/${SOLUTION}.WebApi/${SOLUTION}.WebApi.csproj", "Source/${SOLUTION}.WebApi/"]

# restore dependencies
RUN dotnet restore "Source/${SOLUTION}.WebApi/${SOLUTION}.WebApi.csproj"

# copy the rest of the source code
COPY Source/ ./Source/

WORKDIR "/workdir/Source/${SOLUTION}.WebApi"

# build in Release mode
RUN dotnet build "${SOLUTION}.WebApi.csproj" -c $BUILD_CONFIGURATION -o /artifacts/build

# publish the project for production
RUN dotnet publish "${SOLUTION}.WebApi.csproj" -c $BUILD_CONFIGURATION -o /artifacts/publish /p:UseAppHost=false

# final image to run the app
FROM base AS final
WORKDIR /artifacts

ENV ASPNETCORE_URLS=http://+:8080
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=true
ENV DOTNET_RUNNING_IN_CONTAINER=true

# copy published files from the build stage
COPY --from=build /artifacts/publish .

ENTRYPOINT ["dotnet", "Apta.WebApi.dll"]

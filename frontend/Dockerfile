# use the .NET SDK image (Alpine) to build and publish the wasm app
FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
WORKDIR /workdir

ARG BUILD_CONFIGURATION=Release
ARG PROJECT=Apta.WebUI

# copy only sln and csproj files to restore as early as possible
COPY ["${PROJECT}.sln", "./"]
COPY ["Source/${PROJECT}.csproj", "Source/"]

# restore dependencies
RUN dotnet restore "Source/${PROJECT}.csproj"

# copy the rest of the source code
COPY Source/ ./Source/

WORKDIR "/workdir/Source"

# publish the wasm app for production (static assets land under wwwroot)
RUN dotnet publish "${PROJECT}.csproj" -c $BUILD_CONFIGURATION -o /artifacts/publish

# use nginx (Alpine) to serve the static single page application
FROM nginx:1.27-alpine AS final

EXPOSE 80

# replace the default server config with ours (spa fallback + long cache)
COPY nginx.conf /etc/nginx/conf.d/default.conf

# copy the published static files into the nginx web root
COPY --from=build /artifacts/publish/wwwroot /usr/share/nginx/html

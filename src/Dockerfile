FROM mcr.microsoft.com/dotnet/sdk:5.0 AS PUBLISH_STAGE
WORKDIR /build
ADD ChatApi ./
RUN dotnet publish -r linux-x64 -o bin -c Release

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS RUNTIME_STAGE
WORKDIR /app
COPY --from=PUBLISH_STAGE /build/bin ./
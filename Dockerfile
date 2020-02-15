FROM microsoft/dotnet:2.1-sdk AS build-env
COPY src/DoIt.WebApi /app
WORKDIR /app

RUN dotnet restore
RUN dotnet publish -c Release -o out

# Build runtime image
FROM microsoft/dotnet:2.1-aspnetcore-runtime
WORKDIR /app
COPY --from=build-env /out .
ENV ASPNETCORE_URLS http://*:5000
ENTRYPOINT ["dotnet", "ProcastinationKiller.dll"]
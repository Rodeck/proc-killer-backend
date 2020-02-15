FROM microsoft/dotnet:2.1-sdk AS build-env
COPY src/DoItWebApi /app
WORKDIR /app

RUN dotnet restore
RUN dotnet publish -c Release -o out

# Build runtime image
FROM microsoft/dotnet:2.1-aspnetcore-runtime
WORKDIR /app
RUN pwd
COPY --from=build-env /app/DoItWebApi/out .
ENV ASPNETCORE_URLS http://*:5000
ENTRYPOINT ["dotnet", "ProcastinationKiller.dll"]
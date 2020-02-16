FROM microsoft/dotnet:2.1-sdk AS build
COPY ProcastinationKiller.csproj /build/

RUN dotnet restore ./build/ProcastinationKiller.csproj

COPY . ./build/
WORKDIR /build/

RUN dotnet publish  -c Release -o out

# Build runtime image
FROM microsoft/dotnet:2.1-aspnetcore-runtime
WORKDIR /app
COPY --from=build /build/out .
ENV ASPNETCORE_URLS http://*:5000
ENTRYPOINT ["dotnet", "ProcastinationKiller.dll"]
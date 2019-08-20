FROM microsoft/dotnet:2.2-sdk AS build-env
WORKDIR /sln

# copy everything else and build
COPY . .
WORKDIR /sln/src/Light.Fass
RUN dotnet restore
RUN dotnet publish -c Release -o /app

# build runtime image
FROM aquilahkj/dotnet:2.2-aspnetcore-runtime-gdi
ENV TZ "Asia/Shanghai"
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone
ENV KEY "lightfass"
ENV MAX_UPLOAD_SIZE "5M"
ENV USE_LOG "false"
ENV FILE_DIRECTORY "file"
ENV THUMBNAIL_DIRECTORY "thumbnail"
WORKDIR /app
COPY --from=build-env /app .
ENTRYPOINT ["dotnet", "Light.Fass.dll"]
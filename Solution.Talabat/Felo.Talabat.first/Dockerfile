# مرحلة البناء
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# نسخ الملفات و restore عشان caching أفضل
COPY ["Talabat.API/Talabat.API.csproj", "Talabat.API/"]
COPY ["Talabat.Core/Talabat.Core.csproj", "Talabat.Core/"]     # لو في مشاريع تانية
COPY ["Talabat.Repository/Talabat.Repository.csproj", "Talabat.Repository/"]
# أضف أي csproj تاني موجود في الحل

RUN dotnet restore "Talabat.API/Talabat.API.csproj"

# نسخ كل الكود
COPY . .

# بناء الـ Release
WORKDIR "/src/Talabat.API"
RUN dotnet publish "Talabat.API.csproj" -c Release -o /app/publish

# مرحلة التشغيل (runtime فقط، أخف)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "Talabat.API.dll"]
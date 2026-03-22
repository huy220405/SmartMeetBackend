# Sử dụng môi trường .NET 10 để biên dịch code
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

# Copy toàn bộ code từ GitHub vào máy chủ
COPY . ./

# Biên dịch dự án
RUN dotnet publish "SmartMeetBackend/SmartMeetBackend.csproj" -c Release -o out

# Chạy ứng dụng
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/out .

# Cấu hình cổng mạng cho Render
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

# Lệnh khởi động Server
ENTRYPOINT ["dotnet", "SmartMeetBackend.dll"]

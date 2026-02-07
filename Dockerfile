# --- Giai đoạn 1: Build (Dùng SDK Image nặng để compile code) ---
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 1. Copy file .csproj của TẤT CẢ các lớp (Layer)
# Docker cần file này trước để restore dependencies
COPY ["HoaVanEcommerce.BE/HoaVanEcommerce.BE.csproj", "HoaVanEcommerce.BE/"]
COPY ["HoaVanEcommerce.Application/HoaVanEcommerce.Application.csproj", "HoaVanEcommerce.Application/"]
COPY ["HoaVanEcommerce.Domain/HoaVanEcommerce.Domain.csproj", "HoaVanEcommerce.Domain/"]
COPY ["HoaVanEcommerce.Infrastructure/HoaVanEcommerce.Infrastructure.csproj", "HoaVanEcommerce.Infrastructure/"]

# 2. Restore thư viện (chỉ cần restore project chính, nó tự kéo các project con)
RUN dotnet restore "HoaVanEcommerce.BE/HoaVanEcommerce.BE.csproj"

# 3. Copy toàn bộ source code còn lại vào
COPY . .

# 4. Chuyển vào thư mục chứa code chạy chính để Build
WORKDIR "/src/HoaVanEcommerce.BE"
RUN dotnet publish "HoaVanEcommerce.BE.csproj" -c Release -o /app/publish /p:UseAppHost=false

# --- Giai đoạn 2: Run (Dùng Runtime Image nhẹ để chạy thật) ---
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
# Copy kết quả build từ giai đoạn 1 sang
COPY --from=build /app/publish .

# Mở cổng 8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "HoaVanEcommerce.BE.dll"]
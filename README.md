# 📦 Stok Takip Sistemi (Inventory Management System)

> **KOBİler için kapsamlı stok yönetim sistemi** - Modern teknolojilerle geliştirilmiş, tam özellikli bir envanter takip uygulaması

[![.NET](https://img.shields.io/badge/.NET-8.0-blue.svg)](https://dotnet.microsoft.com/download)
[![Vue.js](https://img.shields.io/badge/Vue.js-3.4-4FC08D.svg)](https://vuejs.org/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

## 🚀 Proje Hakkında

Bu proje, küçük ve orta ölçekli işletmeler (KOBİ) için özel olarak tasarlanmış modern bir stok takip sistemidir. **Full-stack** mimaride geliştirilmiş olup, güçlü backend API'si ve kullanıcı dostu frontend arayüzü ile kapsamlı envanter yönetimi sağlar.

### 🎯 Temel Özellikler

- **📊 Dashboard & Raporlama**: Gerçek zamanlı istatistikler ve analizler
- **📦 Ürün Yönetimi**: Detaylı ürün kataloğu ve stok takibi
- **👥 Müşteri & Tedarikçi Yönetimi**: Kapsamlı iş ortağı bilgi sistemi
- **💰 Satış & Alış İşlemleri**: Tam entegre ticari işlem yönetimi
- **📈 Stok Hareket Takibi**: Otomatik stok güncelleme ve hareket kayıtları
- **⚠️ Stok Uyarıları**: Düşük stok seviyesi bildirimleri
- **🔍 Gelişmiş Arama**: Hızlı ve etkili veri arama
- **📱 Responsive Tasarım**: Mobil uyumlu modern arayüz

## 🏗️ Teknik Mimari

### Backend (.NET 8 Web API)

- **Framework**: ASP.NET Core 8.0
- **Veritabanı**: SQL Server / SQLite (Entity Framework Core)
- **ORM**: Entity Framework Core 8.0
- **Mapping**: AutoMapper
- **Dokümantasyon**: Swagger/OpenAPI
- **Validation**: FluentValidation
- **CORS**: Cross-Origin Resource Sharing desteği

### Frontend (Vue.js 3)

- **Framework**: Vue.js 3 (Composition API)
- **Routing**: Vue Router 4
- **State Management**: Pinia
- **UI Framework**: Element Plus
- **HTTP Client**: Axios
- **Build Tool**: Vite
- **Styling**: Sass
- **Date Handling**: Day.js

## 📁 Proje Yapısı

```
stokTakip/
├── 📁 stokTakip/                 # Backend (.NET 8 Web API)
│   ├── 📁 Controllers/           # API Controllers
│   │   ├── ProductsController.cs
│   │   ├── CustomersController.cs
│   │   ├── SuppliersController.cs
│   │   ├── SalesController.cs
│   │   ├── PurchasesController.cs
│   │   ├── StockMovementsController.cs
│   │   └── DashboardController.cs
│   ├── 📁 Models/                # Entity Models
│   │   ├── Product.cs
│   │   ├── Customer.cs
│   │   ├── Supplier.cs
│   │   ├── Sale.cs & SaleItem.cs
│   │   ├── Purchase.cs & PurchaseItem.cs
│   │   └── StockMovement.cs
│   ├── 📁 DTOs/                  # Data Transfer Objects
│   ├── 📁 Services/              # Business Logic Services
│   ├── 📁 Data/                  # DbContext
│   ├── 📁 Mappings/              # AutoMapper Profiles
│   └── 📁 Migrations/            # Database Migrations
├── 📁 frontend/                  # Frontend (Vue.js 3)
│   ├── 📁 src/
│   │   ├── 📁 views/             # Page Components
│   │   │   ├── Dashboard.vue
│   │   │   ├── Products.vue
│   │   │   ├── Customers.vue
│   │   │   ├── Suppliers.vue
│   │   │   ├── Sales.vue
│   │   │   ├── Purchases.vue
│   │   │   └── StockMovements.vue
│   │   ├── 📁 services/          # API Services
│   │   ├── 📁 router/            # Vue Router
│   │   └── App.vue
│   └── package.json
└── README.md
```

## 🛠️ Kurulum ve Çalıştırma

### Ön Gereksinimler

- **.NET 8.0 SDK** ([İndir](https://dotnet.microsoft.com/download))
- **Node.js 18+** ([İndir](https://nodejs.org/))
- **SQL Server** (LocalDB desteklenir) veya **SQLite**
- **Visual Studio 2022** veya **VS Code** (önerilen)

### 🚀 Hızlı Başlangıç

#### 1. Projeyi Klonlayın

```bash
git clone <repository-url>
cd stokTakip
```

#### 2. Backend Kurulumu

```bash
cd stokTakip

# Paketleri geri yükle
dotnet restore

# Veritabanını oluştur (Migration)
dotnet ef database update

# Backend'i çalıştır
dotnet run
```

Backend API: `https://localhost:7000` (Swagger UI otomatik açılır)

#### 3. Frontend Kurulumu

```bash
cd frontend

# Bağımlılıkları yükle
npm install

# Development server'ı başlat
npm run dev
```

Frontend: `http://localhost:3000`

## 📚 API Dokümantasyonu

Backend çalıştığında Swagger UI'ya `https://localhost:7000` adresinden erişebilirsiniz.

### 🔗 Ana Endpoint'ler

#### 📦 Ürünler (`/api/products`)

```http
GET    /api/products              # Tüm ürünleri listele
GET    /api/products/{id}         # Belirli ürünü getir
POST   /api/products              # Yeni ürün ekle
PUT    /api/products/{id}         # Ürün güncelle
DELETE /api/products/{id}         # Ürün sil
GET    /api/products/low-stock    # Düşük stoklu ürünler
GET    /api/products/search?q=    # Ürün ara
```

#### 👥 Müşteriler (`/api/customers`)

```http
GET    /api/customers             # Tüm müşterileri listele
GET    /api/customers/{id}        # Belirli müşteriyi getir
POST   /api/customers             # Yeni müşteri ekle
PUT    /api/customers/{id}        # Müşteri güncelle
DELETE /api/customers/{id}        # Müşteri sil
```

#### 🏢 Tedarikçiler (`/api/suppliers`)

```http
GET    /api/suppliers             # Tüm tedarikçileri listele
GET    /api/suppliers/{id}        # Belirli tedarikçiyi getir
POST   /api/suppliers             # Yeni tedarikçi ekle
PUT    /api/suppliers/{id}        # Tedarikçi güncelle
DELETE /api/suppliers/{id}        # Tedarikçi sil
```

#### 🛒 Satışlar (`/api/sales`)

```http
GET    /api/sales                 # Tüm satışları listele
GET    /api/sales/{id}            # Belirli satışı getir
POST   /api/sales                 # Yeni satış ekle
PUT    /api/sales/{id}            # Satış güncelle
DELETE /api/sales/{id}            # Satış sil
GET    /api/sales/date-range      # Tarih aralığına göre satışlar
```

#### 📥 Alışlar (`/api/purchases`)

```http
GET    /api/purchases             # Tüm alışları listele
GET    /api/purchases/{id}        # Belirli alışı getir
POST   /api/purchases             # Yeni alış ekle
PUT    /api/purchases/{id}        # Alış güncelle
DELETE /api/purchases/{id}        # Alış sil
```

#### 📊 Dashboard (`/api/dashboard`)

```http
GET    /api/dashboard/stats       # Genel istatistikler
GET    /api/dashboard/recent-sales # Son satışlar
GET    /api/dashboard/top-products # En çok satan ürünler
```

## 💡 Kullanım Örnekleri

### Yeni Ürün Ekleme

```json
POST /api/products
{
  "productName": "Gaming Laptop",
  "productCode": "LAP001",
  "description": "Yüksek performanslı oyun laptopu",
  "unitPrice": 25000.00,
  "stockQuantity": 15,
  "minStockLevel": 3,
  "unit": "Adet",
  "category": "Elektronik",
  "brand": "ASUS"
}
```

### Satış İşlemi

```json
POST /api/sales
{
  "customerId": 1,
  "saleDate": "2024-01-15T10:30:00Z",
  "paymentMethod": "Kredi Kartı",
  "isPaid": true,
  "saleItems": [
    {
      "productId": 1,
      "quantity": 2,
      "unitPrice": 25000.00,
      "discountAmount": 1000.00,
      "taxRate": 18.00
    }
  ]
}
```

## 🗄️ Veritabanı Yapısı

### Ana Tablolar

- **Products**: Ürün bilgileri, stok miktarları ve kategoriler
- **Customers**: Müşteri bilgileri ve kredi limitleri
- **Suppliers**: Tedarikçi bilgileri ve bakiye takibi
- **Sales**: Satış işlemleri ve faturalama
- **SaleItems**: Satış kalemleri detayları
- **Purchases**: Alış işlemleri ve tedarikçi faturaları
- **PurchaseItems**: Alış kalemleri detayları
- **StockMovements**: Tüm stok hareket kayıtları

### 🔗 İlişkiler

- Satışlar ↔ Müşteriler (One-to-Many)
- Alışlar ↔ Tedarikçiler (One-to-Many)
- Satış/Alış Kalemleri ↔ Ürünler (Many-to-One)
- Stok Hareketleri ↔ Ürünler (Many-to-One)
- Otomatik stok güncelleme sistemi

## 🎨 Frontend Özellikleri

### 🖥️ Modern Arayüz

- **Element Plus** UI framework ile modern tasarım
- **Responsive** mobil uyumlu arayüz
- **Dark/Light** tema desteği
- **Gradient** header tasarımı
- **Icon** tabanlı navigasyon

### 📊 Dashboard

- Gerçek zamanlı istatistikler
- Grafik ve chart'lar
- Son işlemler listesi
- En çok satan ürünler
- Düşük stok uyarıları

### 🔧 Gelişmiş Özellikler

- **Arama ve Filtreleme**: Hızlı veri bulma
- **Pagination**: Sayfalama desteği
- **Form Validation**: Gerçek zamanlı doğrulama
- **Toast Notifications**: Kullanıcı bildirimleri
- **Loading States**: Yükleme animasyonları

## 🔒 Güvenlik Özellikleri

- **CORS** koruması
- **Input Validation** (FluentValidation)
- **SQL Injection** koruması (Entity Framework)
- **HTTPS** zorunluluğu
- **Error Handling** ve logging

## 🚀 Deployment

### Production Build

#### Backend

```bash
dotnet publish -c Release -o ./publish
```

#### Frontend

```bash
npm run build
```

### Docker Support (Gelecek özellik)

```dockerfile
# Dockerfile örneği
FROM mcr.microsoft.com/dotnet/aspnet:8.0
COPY ./publish /app
WORKDIR /app
EXPOSE 80
ENTRYPOINT ["dotnet", "stokTakip.dll"]
```

## 🤝 Katkıda Bulunma

1. Fork edin
2. Feature branch oluşturun (`git checkout -b feature/AmazingFeature`)
3. Commit edin (`git commit -m 'Add some AmazingFeature'`)
4. Push edin (`git push origin feature/AmazingFeature`)
5. Pull Request açın

## 📝 Lisans

Bu proje **MIT License** altında lisanslanmıştır. Detaylar için [LICENSE](LICENSE) dosyasına bakın.

## 📞 Destek

- **Issues**: [GitHub Issues](https://github.com/your-repo/issues)
- **Discussions**: [GitHub Discussions](https://github.com/your-repo/discussions)
- **Email**: support@example.com

## 🙏 Teşekkürler

- [.NET Team](https://dotnet.microsoft.com/) - Harika framework
- [Vue.js Team](https://vuejs.org/) - Modern frontend framework
- [Element Plus](https://element-plus.org/) - Güzel UI bileşenleri
- [Entity Framework](https://docs.microsoft.com/en-us/ef/) - Güçlü ORM

---

<div align="center">

**⭐ Bu projeyi beğendiyseniz yıldız vermeyi unutmayın! ⭐**

Made with ❤️ by [Your Name]

</div>

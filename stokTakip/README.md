# Stok Takip API - KOBİler için Stok Yönetim Sistemi

Bu proje, küçük ve orta ölçekli işletmeler (KOBİ) için geliştirilmiş kapsamlı bir stok takip API'sidir.

## Özellikler

### 🏪 Temel Modüller

- **Ürün Yönetimi**: Ürün ekleme, düzenleme, silme ve stok takibi
- **Müşteri Yönetimi**: Müşteri bilgileri ve kredi limiti takibi
- **Tedarikçi Yönetimi**: Tedarikçi bilgileri ve kredi limiti takibi
- **Satış Yönetimi**: Satış işlemleri ve faturalama
- **Alış Yönetimi**: Alış işlemleri ve tedarikçi faturaları
- **Stok Hareketleri**: Tüm stok giriş/çıkış işlemlerinin takibi

### 📊 Raporlama ve Analiz

- **Dashboard**: Genel istatistikler ve özet bilgiler
- **Stok Uyarıları**: Düşük stok seviyesi uyarıları
- **Satış Raporları**: Günlük, aylık satış analizleri
- **En Çok Satan Ürünler**: Popüler ürün analizleri

### 🔧 Teknik Özellikler

- **RESTful API**: Modern API tasarım prensipleri
- **Entity Framework Core**: Veritabanı ORM
- **AutoMapper**: DTO dönüşümleri
- **Swagger UI**: API dokümantasyonu
- **CORS Desteği**: Frontend entegrasyonu için

## Kurulum

### Gereksinimler

- .NET 8.0 SDK
- SQL Server (LocalDB desteklenir)
- Visual Studio 2022 veya VS Code

### Adımlar

1. **Projeyi klonlayın**

```bash
git clone <repository-url>
cd stokTakip
```

2. **Paketleri yükleyin**

```bash
dotnet restore
```

3. **Veritabanını oluşturun**

```bash
dotnet ef database update
```

4. **Uygulamayı çalıştırın**

```bash
dotnet run
```

5. **Swagger UI'ya erişin**

```
https://localhost:7000
```

## API Endpoints

### Ürünler (Products)

- `GET /api/products` - Tüm ürünleri listele
- `GET /api/products/{id}` - Belirli ürünü getir
- `POST /api/products` - Yeni ürün ekle
- `PUT /api/products/{id}` - Ürün güncelle
- `DELETE /api/products/{id}` - Ürün sil
- `GET /api/products/low-stock` - Düşük stoklu ürünler
- `GET /api/products/search?q={term}` - Ürün ara
- `POST /api/products/{id}/stock` - Stok güncelle

### Müşteriler (Customers)

- `GET /api/customers` - Tüm müşterileri listele
- `GET /api/customers/{id}` - Belirli müşteriyi getir
- `POST /api/customers` - Yeni müşteri ekle
- `PUT /api/customers/{id}` - Müşteri güncelle
- `DELETE /api/customers/{id}` - Müşteri sil
- `GET /api/customers/search?q={term}` - Müşteri ara

### Tedarikçiler (Suppliers)

- `GET /api/suppliers` - Tüm tedarikçileri listele
- `GET /api/suppliers/{id}` - Belirli tedarikçiyi getir
- `POST /api/suppliers` - Yeni tedarikçi ekle
- `PUT /api/suppliers/{id}` - Tedarikçi güncelle
- `DELETE /api/suppliers/{id}` - Tedarikçi sil
- `GET /api/suppliers/search?q={term}` - Tedarikçi ara

### Satışlar (Sales)

- `GET /api/sales` - Tüm satışları listele
- `GET /api/sales/{id}` - Belirli satışı getir
- `POST /api/sales` - Yeni satış ekle
- `PUT /api/sales/{id}` - Satış güncelle
- `DELETE /api/sales/{id}` - Satış sil
- `GET /api/sales/date-range?startDate={date}&endDate={date}` - Tarih aralığına göre satışlar
- `GET /api/sales/customer/{customerId}` - Müşteriye göre satışlar
- `GET /api/sales/sale-number` - Yeni satış numarası oluştur

### Alışlar (Purchases)

- `GET /api/purchases` - Tüm alışları listele
- `GET /api/purchases/{id}` - Belirli alışı getir
- `POST /api/purchases` - Yeni alış ekle
- `PUT /api/purchases/{id}` - Alış güncelle
- `DELETE /api/purchases/{id}` - Alış sil
- `GET /api/purchases/date-range?startDate={date}&endDate={date}` - Tarih aralığına göre alışlar
- `GET /api/purchases/supplier/{supplierId}` - Tedarikçiye göre alışlar
- `GET /api/purchases/purchase-number` - Yeni alış numarası oluştur

### Stok Hareketleri (Stock Movements)

- `GET /api/stockmovements` - Tüm stok hareketlerini listele
- `GET /api/stockmovements/product/{productId}` - Ürüne göre stok hareketleri
- `GET /api/stockmovements/date-range?startDate={date}&endDate={date}` - Tarih aralığına göre hareketler
- `GET /api/stockmovements/type/{type}` - Hareket tipine göre listele
- `POST /api/stockmovements` - Yeni stok hareketi ekle

### Dashboard

- `GET /api/dashboard/stats` - Genel istatistikler
- `GET /api/dashboard/recent-sales?count={number}` - Son satışlar
- `GET /api/dashboard/top-products?count={number}` - En çok satan ürünler

## Veritabanı Yapısı

### Ana Tablolar

- **Products**: Ürün bilgileri ve stok miktarları
- **Customers**: Müşteri bilgileri ve kredi limitleri
- **Suppliers**: Tedarikçi bilgileri ve kredi limitleri
- **Sales**: Satış işlemleri
- **SaleItems**: Satış kalemleri
- **Purchases**: Alış işlemleri
- **PurchaseItems**: Alış kalemleri
- **StockMovements**: Stok hareket kayıtları

### İlişkiler

- Satışlar ve Alışlar, müşteri/tedarikçi ile ilişkilidir
- Satış/Alış kalemleri, ürünler ile ilişkilidir
- Stok hareketleri, ürünler ile ilişkilidir
- Tüm işlemler otomatik stok güncellemesi yapar

## Kullanım Örnekleri

### Yeni Ürün Ekleme

```json
POST /api/products
{
  "productName": "Laptop",
  "productCode": "LAP001",
  "description": "Gaming Laptop",
  "unitPrice": 15000.00,
  "stockQuantity": 10,
  "minStockLevel": 2,
  "unit": "Adet",
  "category": "Elektronik",
  "brand": "ASUS"
}
```

### Satış Yapma

```json
POST /api/sales
{
  "customerId": 1,
  "saleDate": "2024-01-15T10:30:00Z",
  "paymentMethod": "Nakit",
  "isPaid": true,
  "saleItems": [
    {
      "productId": 1,
      "quantity": 2,
      "unitPrice": 15000.00,
      "discountAmount": 1000.00,
      "taxRate": 18.00
    }
  ]
}
```

## Güvenlik

- API CORS ile korunmaktadır
- Tüm input validasyonları yapılmaktadır
- SQL injection koruması Entity Framework ile sağlanmaktadır

## Geliştirme

### Proje Yapısı

```
stokTakip/
├── Controllers/          # API Controllers
├── Data/                # DbContext ve veritabanı
├── DTOs/                # Data Transfer Objects
├── Models/              # Entity modelleri
├── Services/            # İş mantığı servisleri
├── Mappings/            # AutoMapper profilleri
└── Program.cs           # Uygulama konfigürasyonu
```

### Katman Mimarisi

- **Controllers**: HTTP isteklerini işler
- **Services**: İş mantığını içerir
- **Data**: Veritabanı erişimi
- **DTOs**: API veri transferi
- **Models**: Veritabanı entity'leri

## Lisans

Bu proje MIT lisansı altında lisanslanmıştır.

## Destek

Herhangi bir sorun veya öneri için issue açabilirsiniz.

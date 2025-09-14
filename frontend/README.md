# Stok Takip Sistemi - Frontend

Bu proje, .NET 8 Web API backend'i ile entegre çalışan Vue.js 3 frontend uygulamasıdır.

## Özellikler

- **Dashboard**: Genel istatistikler ve özet bilgiler
- **Ürün Yönetimi**: Ürün ekleme, düzenleme, silme ve stok takibi
- **Müşteri Yönetimi**: Müşteri bilgileri ve kredi limiti takibi
- **Tedarikçi Yönetimi**: Tedarikçi bilgileri ve bakiye takibi
- **Satış Yönetimi**: Satış işlemleri ve fatura yönetimi
- **Alış Yönetimi**: Alış işlemleri ve tedarikçi faturaları
- **Stok Hareketleri**: Tüm stok hareketlerinin takibi

## Teknolojiler

- Vue 3 (Composition API)
- Vue Router 4
- Pinia (State Management)
- Element Plus (UI Framework)
- Axios (HTTP Client)
- Vite (Build Tool)
- Sass (CSS Preprocessor)

## Kurulum

1. Proje dizinine gidin:

```bash
cd frontend
```

2. Bağımlılıkları yükleyin:

```bash
npm install
```

3. Backend API'nin çalıştığından emin olun (genellikle https://localhost:7000)

4. Development server'ı başlatın:

```bash
npm run dev
```

Uygulama http://localhost:3000 adresinde çalışacaktır.

## Build

Production build için:

```bash
npm run build
```

Build dosyaları `dist` klasöründe oluşturulacaktır.

## API Entegrasyonu

Frontend, backend API'si ile `/api` endpoint'i üzerinden iletişim kurar. Vite proxy konfigürasyonu sayesinde development sırasında CORS sorunları otomatik olarak çözülür.

## Yapı

```
src/
├── components/          # Yeniden kullanılabilir bileşenler
├── views/              # Sayfa bileşenleri
├── services/           # API servisleri
├── router/             # Vue Router konfigürasyonu
├── stores/             # Pinia store'ları
├── assets/             # Statik dosyalar
└── main.js             # Uygulama giriş noktası
```

## Kullanım

1. Dashboard'da genel durumu görüntüleyin
2. Ürünler sayfasından stok yönetimi yapın
3. Müşteri ve tedarikçi bilgilerini yönetin
4. Satış ve alış işlemlerini kaydedin
5. Stok hareketlerini takip edin

## Notlar

- Tüm para birimleri Türk Lirası (TRY) olarak gösterilir
- Tarihler DD.MM.YYYY formatında gösterilir
- KDV oranı %18 olarak hesaplanır
- Responsive tasarım ile mobil uyumludur

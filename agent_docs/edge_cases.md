# Edge Cases & Troubleshooting

Dokumen ini berisi catatan mengenai edge cases, error yang pernah terjadi, dan solusinya selama pengembangan project.

## 1. PFP Browser Caching & Animation Flickering (UI React)
* **Masalah:** Saat animasi berjalan di React (misal, pergantian frame dari Idle ke Walk), penukaran `src` pada satu tag `<img>` menyebabkan layar kedap-kedip atau gambar menghilang (`broken image` / `alt text`) karena latency network atau browser mengosongkan tag sebelum merender gambar baru. Selain itu, hardcode cache-busting seperti `?v=1` akan tetap kena cache kembali jika gambar diubah lagi nanti.
* **Solusi:** 
  1. **Dynamic Cache Busting:** Gunakan module-level timestamp (contoh: `const CACHE_BUST = Date.now();`) sehingga versioning aman dari cache browser tiap kali halaman di-refresh, tapi tidak me-reload gambar terus-terusan saat beranimasi.
  2. **Preload Opacity Stack:** JANGAN menukar `src` pada satu tag `<img>`. Map seluruh frame ke dalam beberapa tag `<img>` yang ditumpuk secara absolut, lalu gunakan `opacity: 1` untuk frame yang sedang aktif dan `opacity: 0` untuk sisanya. Ini memaksa browser mendownload dan men-decode semua frame sekaligus secara instan tanpa kedipan (flickering).

## 2. Gagal Build Menggunakan `build.ps1`
* **Masalah:** Terjadi error saat mencoba mem-build project atau UI menggunakan script `build.ps1` (kemungkinan karena execution policy PowerShell, masalah dependency, atau path).
* **Solusi:** 
  - Pastikan menggunakan command npm/vite build langsung di folder frontend (contoh: `npm run build` di dalam folder `web-ui`), alih-alih bergantung penuh pada script `.ps1` jika script tersebut bermasalah.
  - Dokumentasikan error build dari `build.ps1` agar agen atau developer bisa menggunakan metode manual atau memperbaiki script tersebut jika memang diperlukan.

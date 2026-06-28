# Edge Cases & Troubleshooting

Dokumen ini berisi catatan mengenai edge cases, error yang pernah terjadi, dan solusinya selama pengembangan project.

## 1. PFP Browser Caching Issue (UI React)
* **Masalah:** Saat sprite PFP diperbarui di server, browser sering menyimpan versi cache (cache 404 atau gambar lama) sehingga gambar yang baru tidak muncul.
* **Solusi:** Di sisi frontend (misal pada komponen React `PlayerSprite.jsx`), tambahkan parameter query string bypass cache pada URL gambar. Contoh: `?v=1` atau timestamp.
  ```javascript
  const src = `./sprites/${filename}?v=1`;
  ```

## 2. Gagal Build Menggunakan `build.ps1`
* **Masalah:** Terjadi error saat mencoba mem-build project atau UI menggunakan script `build.ps1` (kemungkinan karena execution policy PowerShell, masalah dependency, atau path).
* **Solusi:** 
  - Pastikan menggunakan command npm/vite build langsung di folder frontend (contoh: `npm run build` di dalam folder `web-ui`), alih-alih bergantung penuh pada script `.ps1` jika script tersebut bermasalah.
  - Dokumentasikan error build dari `build.ps1` agar agen atau developer bisa menggunakan metode manual atau memperbaiki script tersebut jika memang diperlukan.

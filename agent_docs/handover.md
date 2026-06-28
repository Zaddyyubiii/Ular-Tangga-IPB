# Handover Notes: Ular Tangga IPB WebGL

## Context
Fitur PFP Sprite (Dynamic Avatar) pada UI React telah selesai diimplementasikan. Avatar pemain sekarang menyesuaikan dengan tingkat evolusi karakter di dalam game.

## Status Saat Ini: Fitur Sprite PFP (SELESAI)
Sistem Avatar PFP kini sudah mendeteksi warna pemain (Mahasiswa 1/2/3/4) dan me-load *sprite sheet* animasi yang sesuai dengan tingkat evolusi mereka. 

Daftar Link Sprite berdasarkan Tingkat (Misal untuk Mahasiswa 1/Merah `p1`):
- **Tingkat 1 (Punk)**: `./sprites/p1_r1_c1.png?v=1` (Walk: `r2`, `r3`, `r4`)
- **Tingkat 2 (Mulai Belajar)**: `./sprites/p1_r1_c2.png?v=1` 
- **Tingkat 3 (Tertib)**: `./sprites/p1_r1_c3.png?v=1`
- **Tingkat 4 (Teladan)**: `./sprites/p1_r1_c4.png?v=1`
- **Tingkat 5 (Duta)**: `./sprites/p1_r1_c4.png?v=1` *(Menggunakan set sprite Teladan, karena kolom sprite dibatasi maksimal 4)*

*Catatan: Semua file di atas telah disesuaikan namanya (p1, p2, p3, p4) dan cache browser 404 telah di-bypass menggunakan parameter `?v=1`.*

## Pekerjaan Selanjutnya (To-Do List)
1. **Perbaikan Proporsi PFP (Cropping) & Mapping Sprite** `[SELESAI]`
   - Sprite PFP telah di-*crop* dan diletakkan pada kanvas persegi (1:1) yang konsisten.
   - *Pembaruan Aturan: Warna player telah dikunci mati berdasarkan urutan Player (1=Merah, 2=Biru, 3=Hijau, 4=Kuning).*
   - *Bugfix: Mapping sprite telah diubah dari pendeteksian string hex (yang rentan meleset akibat pembulatan) menjadi pengambilan parameter `playerId` absolut dari state Unity. (Player 3 kini menampilkan sprite hijau dengan benar).*

2. **Sprite Animatif untuk Token Ular Tangga di Papan** `[SELESAI]`
   - Pion di atas papan sudah berupa Sprite 2D dan bisa bergerak (animasi jalan).
   - *Bugfix UI React Blink/Hilang*: Sistem *React* diubah menggunakan metode **Preload Opacity Stack** (menumpuk 6 frame `<img/>` sekaligus dan menggeser *opacity*) dengan tambahan dinamis `Date.now()` untuk mengatasi kedipan *(flickering)* dan error *browser caching*.
   - *Bugfix Ilusi Depth 2D (Naik Level)*: Menambahkan logika **Dynamic Z-Sorting** di `GameManager.cs` yang memaksa karakter dengan nilai-Y lebih rendah (bawah) agar di-render di paling depan. Memecahkan ilusi visual kepala karakter yang memakan petak di atasnya.
   - *Penambahan Fitur Board*: Mengubah jumlah kotak kuis dari 6 menjadi **10 kotak kuis** di dalam `BoardConfig`.

3. **Animasi Idle Tambahan untuk Pion (Tugas Mendatang)**
   - Saat pion sedang diam *(idle)* di atas petak papan, buat agar karakternya terasa lebih "hidup".
   - **Target Implementasi**:
     - Tambahkan sesekali animasi tengok kiri dan kanan (misal dengan memutar balik atau *invert* scale horizontal sesekali).
     - Tambahkan animasi pantulan melompat kecil secara acak *(occasional small jump)* agar pion tidak kaku seperti patung.

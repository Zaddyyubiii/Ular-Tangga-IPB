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
1. **Perbaikan Proporsi PFP (Cropping)**
   - Sprite saat ini mungkin ukurannya tidak konsisten atau kurang pas ketika diletakkan di dalam bingkai bundar (PFP).
   - **Target**: Lakukan cropping/penyesuaian ukuran gambar sprite agar *fit* persis dan proporsional di dalam PFP (tidak kekecilan dan tidak kegedean).
2. **Sprite Animatif untuk Token Ular Tangga di Papan (Tugas Berat)**
   - Mengubah token pion kotak-kotak biasa di papan game (Unity) agar menjadi GameObject dengan Sprite Renderer / Animator yang menampilkan sprite 2D sesuai dengan yang ada di PFP.
   - **Target**: Pion di atas papan bisa bergerak (animasi jalan), menampilkan status idle, kaget saat kena ular, dan senang saat naik tangga sesuai tingkat evolusinya.
   - **PENTING:** Modifikasi untuk poin ini harus dikerjakan murni di dalam level/editor **Unity** (merombak logika `PlayerToken.cs` dan Prefab Pion), bukan di frontend React.

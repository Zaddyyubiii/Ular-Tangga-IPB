---
trigger: always_on
glob: "*"
description: Aturan penting bagi AI Agent untuk selalu memperbarui dokumen dan dilarang melakukan operasi git tanpa izin.
---

# AI Agent Guidelines

Sebagai AI Agent yang membantu pengembangan proyek ini, Anda wajib mematuhi peraturan di bawah ini secara ketat demi menjaga integritas kode dan alur kerja repositori.

## 📌 Peraturan Utama AI Agent

### 1. Pembaruan Dokumentasi Secara Berkala
*   **Wajib** memperbarui seluruh file dokumentasi yang berada di dalam folder `agent_docs/` secara berkala setiap kali ada perubahan struktur file, penambahan fitur, perubahan logika permainan, maupun penyuntingan arsitektur sistem.
*   Pastikan `agent_docs/file_desc.md` mencantumkan deskripsi berkas baru yang ditambahkan agar tidak membingungkan pengembang lain (baik manusia maupun AI).
*   Gunakan bahasa yang jelas, terstruktur, dan mudah dipahami.

### 2. DILARANG Melakukan Git Push / Git Add Tanpa Izin Tertulis
*   **DILARANG KERAS** menjalankan perintah `git add`, `git commit`, `git push`, `git checkout`, atau modifikasi status Git lainnya secara otomatis tanpa persetujuan eksplisit dan tertulis dari pengguna (*user*).
*   Jika perubahan kode telah selesai dan siap untuk dikomit/didorong ke repositori GitHub, berikan penjelasan perubahan terlebih dahulu dan minta persetujuan dari pengguna untuk langkah selanjutnya.

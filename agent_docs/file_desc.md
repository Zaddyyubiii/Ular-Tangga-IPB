# Deskripsi Berkas & Struktur Proyek (File Description)

Dokumen ini berisi pemetaan struktur direktori dan fungsi dari berkas-berkas utama di dalam proyek **Ular Tangga Tata Tertib IPB University** untuk membantu AI Agent dan pengembang memahami arsitektur proyek seiring berkembangnya kompleksitas game.

---

## 📁 Struktur Folder Utama

### 1. `Assets/Art/` (Aset Grafis)
Berisi seluruh gambar 2D, sprite, dan UI yang digunakan dalam game.
*   **MainMenu/**: Aset visual untuk menu utama (background, tombol, dll).
*   **Board/**: Gambar papan permainan ular tangga berukuran 10x10.
*   **Characters/**: Sprite untuk karakter pemain (termasuk sprite evolusi: gaya Punk, reguler, teladan, kemeja rapi, dan Duta IPB).
*   **UI/**: Aset untuk tombol, panel pop-up, meteran dadu, dan status bar.

### 2. `Assets/Audio/` (Aset Suara)
*   **BGM/**: Musik latar belakang (*Background Music*) yang berbeda untuk setiap level/fase permainan.
*   **SFX/**: Efek suara (*Sound Effects*) seperti bunyi pelemparan dadu, ledakan bom tengkorak, suara menaiki tangga, atau suara turun ular.

### 3. `Assets/Scenes/` (Adegan Permainan)
*   `MainMenuScene.unity`: Scene menu awal untuk memilih jumlah pemain (2-4), memasukkan nama, dan membaca prolog.
*   `GameScene.unity`: Scene utama tempat permainan ular tangga berlangsung.

### 4. `Assets/Scripts/` (Logika Pemrograman C#)
Seluruh logika permainan diatur oleh skrip-skrip yang terbagi secara modular:
*   **Audio/**: Pengatur pemutaran musik latar dan efek suara (`AudioManager.cs`).
*   **Board/**: Mengatur pembuatan petak papan permainan, posisi koordinat, dan jenis petak (`BoardManager.cs`, `Tile.cs`).
*   **Core/**: Logika pusat alur permainan dan inisialisasi state (`GameManager.cs`).
*   **Dice/**: Logika pengukur dadu interaktif (*Dice Gauge*) dan animasi dadu bergulir (`DiceController.cs`, `DiceGauge.cs`).
*   **Player/**: Mengatur posisi bidak pemain, pergerakan petak demi petak, serta perubahan sprite evolusi berdasarkan skor (`PlayerController.cs`, `PlayerEvolutionController.cs`).
*   **Quiz/**: Mengontrol jendela sembul kuis, membaca bank soal kuis, serta memvalidasi jawaban benar/salah (`QuizManager.cs`, `QuizUI.cs`).
*   **Turn/**: Mengelola sistem giliran pemain, timer giliran 10 detik, dan bot AI (`TurnManager.cs`, `BotAIController.cs`).
*   **UI/**: Mengatur pembaruan UI seperti papan skor, status bar, pop-up pesan, dan dialog prolog (`UIManager.cs`, `ScoreboardUI.cs`).
*   **Editor/**: Skrip pembantu editor Unity dan modul pengujian otomatis:
    *   `SceneSetupHelper.cs`, `WebGLBuildHelper.cs`: Berkas utilitas pembantu Unity Editor.
    *   **Tests/**: Modul berkas pengujian otomatis (*automated unit tests*) EditMode:
        *   `DiceRollResolverTests.cs`: Menguji akurasi pembagian zona dadu, persentase daya pelepasan, pendeteksian kualitas waktu (*Perfect/Good/Normal*), dan strategi mendekati finis.
        *   `PlayerEvolutionTests.cs`: Menguji ketepatan ambang batas skor petak evolusi karakter (Stage 1 s.d 5) dan alokasi visual sprite.
        *   `BoardConfigTests.cs`: Menguji keabsahan konfigurasi transisi ular/tangga, batasan petak khusus, serta perhitungan rumus *serpentine grid* 10x10.

### 5. `Assets/ScriptableObjects/` (Aset Konfigurasi Data)
Penyimpanan data statis berbasis ScriptableObject untuk memudahkan penyeimbangan tingkat kesulitan (*level balancing*):
*   `BoardConfig.asset`: Konfigurasi letak petak kuis, petak tengkorak, posisi tangga, serta ular beserta efeknya.
*   `QuizBank.asset`: Bank soal kuis yang berisi teks pertanyaan, pilihan ganda, kunci jawaban, dan feedback edukatif.
*   `MessageBank.asset`: Kumpulan teks cerita prolog, pesan positif di petak biasa, pesan ular, sanksi tengkorak, dan pesan tangga.

### 6. `docs/` (WebGL Deployment)
*   Berisi berkas hasil *build* WebGL permainan yang di-overlay oleh bundle web produksi React yang siap di-host menggunakan GitHub Pages.

### 7. `web-ui/` (Aplikasi React HUD Overlay)
*   Aplikasi frontend berbasis Vite + React + Tailwind CSS v4 + Framer Motion yang melayang di atas Canvas Unity.
*   **src/components/**: Komponen-komponen UI game yang playful:
    *   `PlayerCards.jsx`: 4 kartu pojok status pemain, menampilkan nama, bot tag, ubin aktif, pelanggaran tatib, tahapan evolusi (MABA s.d Duta Tatib), serta efek denyut gilirannya.
    *   `TopHUD.jsx`: Kapsul giliran aktif di bagian atas-tengah dengan timer lingkaran yang bergetar (*shake*) memerah jika sisa waktu kritis.
    *   `RollDiceBar.jsx`: Meteran dadu datar horizontal di bawah-tengah dengan reflex-charging 280%/detik dan label range/kebutuhan ubin.
    *   `QuizModal.jsx`: Pop-up kuis bubble dengan pilihan ganda dan feedback akademik edukatif instan.
    *   `PrologueModal.jsx`: Pop-up prolog pembuka bergaya lembaran buku dongeng petualangan.
    *   `GameOverModal.jsx`: Pop-up penobatan Duta Tata Tertib IPB University dan tombol navigasi akhir.
*   `index.html`: Pembungkus terpadu yang memuat Canvas WebGL Unity dan menumpangkan React Overlay `#root` di atasnya.
*   `vite.config.js`: Konfigurasi build Vite yang otomatis mengekspor bundle produksi ke folder `/docs/` tanpa mengganggu file `.wasm` Unity.
*   `src/index.css`: Konfigurasi theme modern Tailwind v4 `@theme` (font Outfit, game-dark/blue/cyan palette, rounded-cartoon, shadow-bubble).

### 8. `agent_docs/` (Dokumentasi Agent)
*   `file_desc.md`: Dokumen deskripsi file ini (wajib di-update berkala).
*   `rules.md`: Peraturan resmi permainan ular tangga tata tertib IPB.
*   `ui_architecture.md`: Dokumen arsitektur dan kontrak pertukaran data JSON antara Unity C# dan React JS.

### 9. Berkas Server & Script Otomatisasi di Root
*   `server.js`: Skrip server lokal menggunakan Node.js (tanpa dependensi eksternal) untuk memuat berkas WebGL game secara instan pada Port 3000 dengan header MIME type `.wasm` dan `.data` yang tepat.
*   `server-log.js`: Skrip server log pengembang independen menggunakan Node.js (tanpa dependensi eksternal) pada Port 3001 untuk menangkap, memfilter, dan melihat aliran log game secara terpisah secara real-time.
*   `server.py`: Skrip server lokal menggunakan Python (tanpa dependensi eksternal) dengan dukungan *multithreading* dan konfigurasi MIME type lengkap.
*   `build.ps1`: Skrip otomatisasi PowerShell untuk membangun ulang WebGL menggunakan batchmode headless Unity.exe secara langsung, dan otomatis menjalankan server Node.js saat kompilasi selesai sukses.

---

*Catatan: Selalu perbarui deskripsi berkas ini jika Anda menambahkan berkas baru atau memindahkan letak aset.*

# Handover Dokumen & Status Redesain Visual (Unity Visual Redesign)

Dokumen ini merangkum seluruh pencapaian **Redesain Visual UI/UX Native Unity** untuk menyelaraskan game **"Ular Tangga Tata Tertib IPB University"** dengan panduan estetika **cozy pixel-art** di `design.md`.

---

## 📌 Status Terkini (Update: 25 Juni 2026)

### ✅ Build WebGL Terbaru
- **Unity build**: SUKSES (Build Successful, batchmode)
- **React web-ui**: SUKSES (vite build berhasil, output ke `/docs/`)
- **Server lokal**: Berjalan di `http://localhost:3000`
- **Game bisa dimainkan**: Ya — Main Menu, Prologue, Gameplay semua terload

### ✅ Bug yang Telah Diperbaiki (Session Ini)
1. **Posisi Judul "ULAR TANGGA TATA TERTIB"** — Sebelumnya muncul di bawah tombol MULAI BERMAIN karena `MainMenuUI.cs` mencari `"Title"` (child dari TitleBoard) bukan `"TitleBoard"` (langsung child CenterCard). Sudah diperbaiki dengan `container.Find("TitleBoard") ?? container.Find("Title")`.
2. **RepairGeneratedLayout** — Diperbaiki untuk menggunakan `GetComponentInChildren<TMP>` dari TitleBoard agar bisa membaca teks di dalam panel kayu.
3. **build.ps1 project path** — Diupdate dari `C:\Users\LENOVO\Desktop\` ke `C:\Users\Ayubi\Documents\GitHub\`.

---

## 📌 Pencapaian Sistem (Completed Milestones)

Seluruh visual game (Main Menu, Board Game, Papan Skor, Pop-up, Dadu, Kuis, Giliran Bot, dan GameOver) kini telah sepenuhnya dirancang ulang secara prosedural dan Scriptable-Object-driven menggunakan palet warna kayu, kertas parchment hangat, garis tepi (outlines) bertekstur, serta teks kontras tinggi yang ramah di mata.

### 1. Sumber Desain Terpusat (`GameVisualTheme.cs` & `GameVisualTheme.asset`)
*   **GameVisualTheme** telah dibuat sebagai ScriptableObject di folder `Assets/Resources/` agar bisa diakses dinamis di Editor maupun Runtime (`Resources.Load`).
*   Menyimpan seluruh token visual: warna kayu (`woodBrown`, `darkWood`), kertas (`parchment`), teks (`darkText`, `creamText`), status (`successGreen`, `warningOrange`, `dangerRed`), serta fonts.
*   Menyediakan fungsi penataan prosedural instan:
    *   `StylePanelAsParchment(GameObject panel)`
    *   `StylePanelAsWood(GameObject panel)`
    *   `StyleButtonAsWood(Button btn, TMPro.TextMeshProUGUI label)`
    *   `StyleButtonAsParchment(Button btn, TMPro.TextMeshProUGUI label)`
    *   `StyleInputFieldAsParchment(TMPro.TMP_InputField inputField)`

### 2. Penataan Visual Komponen Utama (Cozy Redesign)
*   **Menu Utama (`MainMenuUI.cs` & Scene Generator)**: Tombol-tombol menu diatur sebagai papan kayu kokoh dengan teks krem kontras. Kolom input nama pemain ditata seperti kertas parchment bersudut lembut dengan placeholder teks yang mudah dibaca.
*   **Papan Permainan & Ubin (`BoardManager.cs`)**: Petak biasa digambar dengan warna tanah/rumput alami bergelombang hangat. Petak khusus (kuis, tengkorak, ular, tangga) diberi outline warna yang serasi dengan identitas visualnya. Garis ular dan tangga disesuaikan dengan warna tema visual terpusat.
*   **Papan Skor/Status (`PlayerStatusView.cs`)**: Kartu pemain yang aktif akan sedikit membesar (`1.05x`) dan memiliki outline krem menyala sesuai warna giliran aktif, sementara kartu tidak aktif meluncur ke abu-abu redup untuk fokus pemain yang jelas.
*   **Pop-up Edukasi & Sanksi (`PopupController.cs`)**: Panel pop-up diubah menjadi lembaran parchment kertas tua. Warna outline pop-up menyesuaikan konteks event secara dinamis (Hijau untuk prestasi/duta, Jingga untuk pelanggaran biasa/ular, Merah menyala untuk pelanggaran berat/skors/tengkorak).
*   **Kuis Tata Tertib (`QuizPopup.cs`)**: Tombol pilihan ganda A s.d D diubah menjadi papan kertas parchment tipis. Saat tombol ditekan, jawaban benar langsung menyala hijau sukses, dan jawaban salah yang dipilih menyala merah bahaya demi feedback edukatif instan.
*   **Indikator Dadu (`DiceRollPopupUI.cs`)**: Kartu hasil dadu ditata sebagai parchment panel melayang dengan outline warna penanda pemain yang sedang aktif melempar.

### 3. Sistem React Web-UI (React + Vite)
- React overlay berjalan di atas Unity WebGL canvas
- Menampilkan player cards, turn indicator, dice bar, dan prologue screen
- Komunikasi Unity ↔ React via `ReactBridge.jslib` / custom window events
- Di-serve dari `docs/` oleh `server.js` (Node.js)

### 4. Generator Scene yang Tangguh (`SceneSetupHelper.cs`)
*   Generator scene otomatis (`SetupPlayableScenes`) telah diperbarui total untuk meminimalkan error layout.
*   **Helper `FindChildComponent<T>`**: Menambahkan sistem pencarian komponen UI secara rekursif berbasis nama jika jalur langsung berubah di Editor.

---

## 🛠️ Panduan Menjalankan Lingkungan Lokal & Pengujian

### Cara Build Ulang (jika ada perubahan script C#):

1. **Tutup Unity Editor** jika sedang terbuka
2. Jalankan:
   ```powershell
   .\build.ps1
   ```
   Atau jika hanya rebuild React UI saja (tanpa perubahan Unity C#):
   ```powershell
   cd web-ui; npm run build
   node server.js
   ```

### Cara Menjalankan Server Lokal:
```bash
node server.js
```
Akses: **`http://localhost:3000`**

### Pengujian Alur:
1. Masukkan nama pemain, pilih jumlah pemain, klik **MULAI BERMAIN**
2. Perhatikan transisi cerita prolog dengan bingkai kayu dan parchment teks
3. Lakukan pelemparan dadu dan perhatikan perubahan status kartu di sebelah kanan
4. Kemunculan pop-up event kuis dengan outline warna dinamis

---

## ⚠️ Hal yang Perlu Ditest Manual

1. **Posisi judul "ULAR TANGGA TATA TERTIB"** — Seharusnya sekarang sudah di atas (antara dadu dan input player)
2. **Board size saat gameplay** — Pastikan board terlihat besar dan tidak terlalu kecil di tengah layar
3. **Popup dadu vs popup peraturan** — Pastikan tidak overlap/menumpuk
4. **Bot rolling indicator** — Pastikan ada tampilan ketika bot sedang roll dadu

---

## 📌 Update Terkini (Emergency Gameplay Restore: 25 Juni 2026)

### 🚨 Penyebab Masalah Gameplay Hilang (Hanya Board Terlihat)
Sistem build Unity WebGL (`build.ps1`) secara default menimpa file `docs/index.html` dengan templat Unity standar yang tidak memiliki pembungkus React (`#root` dan React asset scripts). Karena build Unity dijalankan terakhir, build React sebelumnya tertimpa sepenuhnya, sehingga HUD (kartu status, control panel, dsb) hilang dan game menjadi tidak bisa dimainkan.

### 🛠️ Solusi & Perubahan yang Dilakukan
1. **Urutan Build Terpadu**:
   - Unity WebGL harus di-build terlebih dahulu (menghasilkan binary dan menulis `docs/index.html` awal).
   - React `web-ui` di-build setelahnya (`npm run build`), sehingga Vite membaca `web-ui/index.html` dan menimpa `docs/index.html` dengan React-enabled HTML secara bersih.
2. **Perbaikan Script C#**:
   - Menambahkan namespace `using Board;` pada `GameplayUI.cs` untuk mengatasi compile error `BoardManager does not exist in the current context` yang sempat menghentikan build.
3. **Peningkatan Board & Popup**:
   - Board diperbesar menjadi `80%` dari tinggi canvas, dan pembatasan margin diselaraskan agar tidak menabrak player status cards di pojok layar.
   - Dice popup (React) disembunyikan secara otomatis saat kuis aktif untuk mencegah tumpang-tindih.
   - Pesan normal tile tidak lagi mencantumkan hasil angka dadu (angka dadu murni hanya ada di popup kocokan dadu).

---

## ⚠️ Pengingat Aturan Git (Git Integrity Warning)

Sesuai dengan berkas `RULE[agents.md]`:
*   **DILARANG** melakukan `git add`, `git commit`, atau `git push` secara otomatis.
*   Jika visual game sudah sesuai dengan keinginan Anda setelah diuji di browser, berikan instruksi tertulis agar saya bisa mempersiapkan commit perubahan ini ke repositori Anda.

Selamat menguji keindahan visual baru "Ular Tangga Tata Tertib IPB University"! 🎓🍃

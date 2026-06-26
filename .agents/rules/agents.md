---
trigger: always_on
description: Aturan penting bagi AI Agent untuk selalu memperbarui dokumen dan dilarang melakukan operasi git tanpa izin.
---

# AI Agent Guidelines

Dokumen ini adalah aturan utama untuk AI Agent yang membantu pengembangan project **Ular Tangga Tata Tertib IPB University**. Semua agent wajib mengikuti instruksi ini sebelum membaca, menulis, mengubah, atau menghapus kode.

Tujuan utama dokumen ini adalah menjaga konsistensi kode, desain, data, dokumentasi, dan flow game.

---

## 1. Wajib Baca File Penting Sebelum Coding

Sebelum mulai membuat atau mengubah kode, AI Agent wajib membaca file penting berikut:

1. `AGENTS.md`
   Untuk memahami aturan kerja agent.

2. `design.md`
   Untuk memahami gaya visual game: pixel-art cozy, warna, UI, popup, scoreboard, main menu, dice popup, dan prinsip UX.

3. `agent_docs/file_desc.md`
   Untuk memahami fungsi file/folder yang sudah ada.

4. Semua dokumen relevan di folder `agent_docs/`.

5. `README.md` jika tersedia.

6. File script yang berkaitan langsung dengan fitur yang ingin diubah.

Contoh:

* Ubah dice: baca `Scripts/Dice/`, `Scripts/Turn/`, `Scripts/UI/`.
* Ubah quiz: baca `Scripts/Quiz/`, `QuizPopup`, quiz bank/data lama.
* Ubah popup: baca `PopupController`, `DiceRollPopupUI`, `QuizPopup`, UI manager.
* Ubah board: baca `BoardManager`, `BoardRandomizer`, `TileDefinition`, board config.
* Ubah scoreboard: baca `PlayerStatusView`, `GameplayUI`, `PlayerData`, `TurnManager`.
* Ubah main menu: baca `MainMenuUI`, `GameSetup`, scene setup.

Agent dilarang langsung coding tanpa memahami konteks file yang sudah ada.

Jika file terkait belum ditemukan, lakukan pencarian dengan keyword seperti:
`Dice`, `Roll`, `Popup`, `Quiz`, `Question`, `Board`, `Tile`, `PlayerStatus`, `Scoreboard`, `MainMenu`, `GameSetup`, `MessageBank`.

---

## 2. Wajib Mengikuti `design.md`

Semua perubahan UI/UX dan visual wajib mengikuti `design.md`.

Arah desain utama:

* cozy pixel-art / 8-bit atau 16-bit inspired,
* warna hangat dan natural,
* panel kayu dan parchment,
* UI ramah, playful, dan readable,
* main menu, popup, scoreboard, board, dan dice UI harus konsisten.

Agent dilarang membuat desain yang bertentangan dengan `design.md`, misalnya:

* UI terlalu glossy seperti aplikasi bisnis,
* warna neon berlebihan,
* popup polos tanpa style,
* active player menjadi putih,
* active player memakai hijau universal,
* text terlalu kecil,
* layout overlap.

Jika instruksi user berkaitan dengan desain, tetap sesuaikan dengan `design.md`.

---

## 3. Jangan Merusak Format Data Lama

Jika project sudah punya format data tertentu, agent wajib mengikuti format lama.

Berlaku untuk:

* quiz bank,
* normal tile message bank,
* board config,
* player sprite set,
* game setup data,
* ScriptableObject,
* JSON,
* CSV,
* prefab reference,
* scene reference.

Sebelum menambah data:

1. Cari file data lama.
2. Pahami schema/field yang dipakai.
3. Tambahkan data baru dengan format yang sama.
4. Jangan hapus data lama.
5. Jangan rename field sembarangan.
6. Jangan merusak reference Inspector.

Contoh:

* Tambah soal kuis: cari quiz bank lama dulu.
* Tambah 100 pesan normal tile: cari message bank lama dulu.
* Tambah karakter: cari `PlayerSpriteSet` atau sistem sprite lama.

Jangan membuat sistem baru kalau sistem lama sudah ada.

---

## 4. Wajib Inspect Sebelum Mengubah

Sebelum edit file:

1. Cari file terkait.
2. Baca isi file.
3. Pahami hubungan antar class.
4. Cek dependency.
5. Baru ubah kode.

Hindari perubahan buta yang dapat menyebabkan:

* duplicate class,
* duplicate manager,
* duplicate singleton,
* broken prefab/scene reference,
* compile error,
* `NullReferenceException`,
* UI overlap,
* event listener dobel,
* gameplay freeze.

---

## 5. Update Dokumentasi

Setiap ada perubahan struktur, fitur, data, scene, prefab, ScriptableObject, logika gameplay, atau UI, agent wajib memperbarui dokumentasi di `agent_docs/`.

Minimal update:

* `agent_docs/file_desc.md`

Isi dokumentasi harus menjelaskan:

* file baru yang ditambahkan,
* fungsi file,
* lokasi file,
* hubungan dengan sistem lain,
* catatan penggunaan jika perlu.

Gunakan bahasa jelas dan ringkas agar mudah dipahami manusia maupun AI agent lain.

---

## 6. Larangan Git Tanpa Izin

Agent dilarang keras menjalankan perintah berikut tanpa izin tertulis user:

* `git add`
* `git commit`
* `git push`
* `git checkout`
* `git reset`
* `git rebase`
* `git merge`
* `git stash`
* operasi Git lain yang mengubah status repo

Jika perubahan siap dikomit:

1. Berikan ringkasan perubahan.
2. Sebutkan file yang berubah.
3. Jelaskan dampak perubahan.
4. Sebutkan risiko/manual check.
5. Minta izin user sebelum menjalankan perintah Git.

---

## 7. Jangan Membuat Sistem Duplikat

Agent harus memperbaiki dan melanjutkan sistem yang sudah ada.

Dilarang membuat class duplikat seperti:

* `GameManager2`
* `NewQuizManager`
* `BetterPopupController`
* `FinalDiceSystem`

Jika sistem lama perlu refactor:

1. Jelaskan alasannya.
2. Jaga compatibility.
3. Jangan merusak scene/prefab reference.
4. Update dokumentasi.
5. Hindari perubahan besar yang tidak diminta.

---

## 8. Kompatibilitas Unity dan WebGL

Project ditargetkan untuk Unity dan WebGL.

Pastikan:

* aman di Unity Editor,
* aman di WebGL,
* tidak memakai API yang bermasalah di WebGL,
* tidak bergantung pada file system runtime yang tidak aman,
* tidak memakai thread/platform-specific API tanpa alasan,
* UI responsive untuk desktop dan mobile.

---

## 9. Jaga Flow Gameplay Tidak Freeze

Setiap perubahan harus menjaga flow game tetap berjalan.

Flow yang wajib aman:

* main menu,
* input nama,
* human turn,
* bot turn,
* dice roll,
* bot rolling indicator,
* dice result popup,
* movement,
* normal tile popup,
* quiz popup,
* bot answer quiz,
* snake,
* ladder,
* skull,
* skip turn,
* finish,
* final ranking,
* return to main menu.

Jika memakai popup/coroutine/callback:

* callback hanya terpanggil sekali,
* tidak ada double close,
* bot tidak menunggu klik manual,
* human tetap bisa klik manual,
* auto close berjalan sesuai durasi,
* turn lanjut setelah flow selesai.

---

## 10. Konsistensi Data Player

Player harus punya identitas konsisten sepanjang game:

* nama,
* human/bot,
* warna,
* posisi tile,
* sprite stage,
* status finish,
* status drop out,
* skull count,
* skip turn,
* ranking.

Scoreboard, token, popup, dice UI, dan ranking harus membaca data dari sumber yang sama, misalnya `PlayerData`.

Jangan mengganti warna player secara random setelah game dimulai.

---

## 11. Aturan Scoreboard

Scoreboard harus mengikuti desain:

* inactive = warna player versi gelap/redup,
* active = warna player versi terang,
* active tidak boleh putih,
* active tidak boleh hijau universal,
* text harus readable,
* FINISH/DROP OUT/SKIP tidak boleh menghapus identitas warna player,
* highlight boleh memakai border, glow, label, atau scale up.

Jika mengubah scoreboard, baca:

* `design.md`
* `PlayerStatusView`
* `GameplayUI`
* `TurnManager`
* `PlayerData`

---

## 12. Aturan Popup

Popup dibagi menjadi:

### Dice Result Popup

* durasi 2 detik,
* tampil untuk human dan bot,
* token bergerak setelah popup selesai,
* desain modern pixel-art,
* memakai warna/accent player.

### Bot Rolling Indicator

* tampil sebelum hasil dadu bot,
* minimal 1 detik,
* harus tampil di UI, bukan hanya `Debug.Log`.

### Gameplay Popup

Meliputi normal tile, snake, ladder, skull, finish, bounce back.

* auto close default 5 detik,
* human boleh klik manual,
* bot auto close.

### Quiz Popup

* tidak auto close sebelum jawaban dipilih,
* feedback auto close 5 detik,
* bot harus bisa menjawab otomatis.

Jaga durasi dan behavior tiap popup agar tidak tercampur.

---

## 13. Aturan Quiz

Quiz harus:

* memakai quiz bank yang sudah ada,
* mendukung True/False,
* mendukung A/B,
* mendukung A-D,
* tidak memakai soal esai,
* dicek otomatis dengan `correctAnswerIndex` atau sistem validasi lama,
* bot bisa menjawab otomatis,
* human bisa menjawab manual,
* feedback benar/salah muncul,
* tidak freeze setelah selesai.

Saat menambah soal:

* cari quiz bank lama,
* append ke file/data yang sama,
* jangan hapus soal lama,
* jangan duplicate ID,
* pastikan `correctAnswerIndex` valid.

---

## 14. Aturan Normal Tile Message

Normal tile message harus:

* memakai message bank lama,
* tidak di-hardcode di `GameManager`,
* mendukung placeholder seperti `{points}`,
* bisa berdasarkan tile number atau random fallback,
* aman untuk board random,
* tampil untuk human dan bot,
* memakai gameplay popup.

Saat menambah pesan:

* cari message bank lama,
* append ke file/data yang sama,
* jangan hapus pesan lama,
* pastikan ID dan jumlah data valid.

---

## 15. Aturan Board

Board memakai:

* 100 tile,
* layout 10x10,
* serpentine path,
* tile 0 sebagai start di luar board,
* tile 100 sebagai finish,
* random special tile jika fitur aktif.

Pastikan:

* question/skull/snake/ladder tidak overlap,
* snake dan ladder valid,
* board tetap playable,
* visual board mengikuti runtime board config.

---

## 16. Aturan Dice System

Dice system harus:

* menampilkan bot rolling indicator,
* menampilkan hasil dadu human dan bot,
* memakai dice result popup 2 detik,
* token bergerak setelah popup selesai,
* mendukung dice gauge,
* mendukung bot dice strategy,
* mendukung bounce back jika melewati tile 100,
* tidak menyembunyikan hasil dadu terlalu cepat.

Semua jalur roll, human maupun bot, harus melewati UI dice yang konsisten.
---

## 17. Aturan Main Menu

Main menu harus:

* mengikuti `design.md`,
* tidak overlap,
* input nama tidak menutup tombol mulai game,
* tombol mulai game selalu terlihat dan bisa diklik,
* input nama mengikuti jumlah human player,
* nama kosong memakai default,
* data nama masuk ke `GameSetup` atau sistem setup lama.

Jika mengubah main menu, cek:

* layout group,
* safe area,
* input field,
* start button,
* raycast target,
* scene loading,
* data setup antar scene.

---

## 18. Return to Main Menu

Kembali ke main menu harus:

* reset gameplay state,
* stop coroutine,
* close popup,
* reset dice UI,
* reset quiz popup,
* mencegah duplicate singleton,
* tidak membawa state game lama,
* bisa start game baru setelah kembali.

Jika mengubah fitur ini, cek:

* `SceneManager.LoadScene`,
* `GameSetup`,
* singleton,
* event listener,
* coroutine,
* `Time.timeScale`.

## 19. Testing Wajib

Setelah perubahan, lakukan pengecekan:

### Compile

* tidak ada compile error,
* tidak ada missing namespace,
* tidak ada duplicate class,
* tidak ada method/reference hilang.

### Runtime Flow

Cek:

* start dari main menu,
* input nama,
* start game,
* human roll,
* bot roll,
* dice result popup,
* bot rolling indicator,
* movement,
* normal tile,
* quiz tile,
* bot answer quiz,
* snake/ladder/skull,
* finish,
* final ranking,
* return main menu,
* start game lagi.

### UI

Cek:

* tidak overlap,
* text readable,
* popup layer benar,
* scoreboard active jelas,
* dice popup terlihat,
* main menu button tidak tertutup,
* aman desktop/mobile.

## 20. Laporan Setelah Selesai

Setelah task selesai, agent wajib melaporkan:

1. File yang dibaca.
2. File yang diubah.
3. File baru yang dibuat.
4. Perubahan utama.
5. Cara kerja setelah perubahan.
6. Risiko atau manual check.
7. Dokumentasi yang diperbarui.
8. Catatan Git: belum menjalankan Git karena butuh izin user.

Contoh:

```text
Selesai.

File yang dibaca:
- design.md
- agent_docs/file_desc.md
- Assets/Scripts/Dice/DiceRollPopupUI.cs

File yang diubah:
- Assets/Scripts/Dice/DiceRollPopupUI.cs
- agent_docs/file_desc.md

Perubahan:
- Menambahkan indikator bot rolling.
- Mengatur dice result popup menjadi 2 detik.

Manual check:
- Pastikan reference DiceRollPopupUI sudah diassign di Inspector.
Git:
- Belum menjalankan git add/commit/push karena butuh izin user.

## 21. Larangan Tambahan

Agent dilarang:

* menghapus file tanpa alasan kuat,
* menghapus data lama tanpa izin,
* mengubah scene/prefab besar-besaran tanpa diminta,
* membuat duplicate manager,
* mengabaikan `design.md`,
* mengabaikan dokumentasi,
* memakai asset game lain secara langsung,
* membuat UI tidak readable,
* menjalankan Git tanpa izin tertulis
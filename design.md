# DESIGN.md

# Ular Tangga Tata Tertib IPB University

## 1. Design Vision

Game **Ular Tangga Tata Tertib IPB University** menggunakan gaya visual **cozy pixel-art board game** yang terinspirasi dari nuansa game pertanian klasik: hangat, ramah, natural, dan mudah dibaca. Visual game harus terasa seperti dunia kampus yang dibuat dalam bentuk papan permainan pixel-art, dengan elemen kayu, rumput, jalan setapak, ikon sederhana, dan panel pop-up bergaya kertas/parchment.

Tujuan desain bukan untuk meniru game lain secara langsung, tetapi mengambil prinsip visualnya:

* pixel-art yang nyaman dilihat,
* warna natural dan hangat,
* UI berbentuk panel kayu/kertas,
* ikon yang sederhana dan mudah dikenali,
* animasi ringan yang terasa playful,
* dan pengalaman bermain yang tetap jelas untuk user.

Game ini harus terasa edukatif, santai, dan menyenangkan, bukan seperti aplikasi formal atau sistem administrasi kampus.

---

## 2. Core Design Keywords

Gunakan kata kunci visual berikut sebagai acuan seluruh desain:

* Cozy
* Pixel-art
* 8-bit / 16-bit inspired
* Warm
* Friendly
* Campus board game
* Wooden UI
* Parchment pop-up
* Nature-based
* Educational but playful
* Clear readability
* Soft contrast
* Simple but polished

---

## 3. Visual Direction

### 3.1 Style Utama

Semua elemen visual menggunakan gaya **pixel-art modern**. Artinya:

* bentuk objek terlihat kotak-kotak/pixelated,
* tetapi tetap rapi dan nyaman dilihat,
* tidak terlalu kasar,
* tidak terlalu realistis,
* dan tetap terbaca jelas di layar desktop maupun mobile.

Gunakan pendekatan **pixel-art 16-bit inspired** dibanding 8-bit murni, karena game perlu menampilkan teks, popup, karakter, board, dan UI dengan detail yang cukup.

### 3.2 Jangan Meniru Langsung

Referensi visual yang digunakan hanya sebagai inspirasi. Jangan:

* menyalin logo,
* menyalin font khas game lain,
* menyalin asset langsung,
* menyalin layout persis,
* atau memakai sprite dari game lain tanpa izin.

Semua asset harus dibuat original dengan rasa visual yang mirip: hangat, pixelated, farming/cozy, dan natural.

---

## 4. Color Palette

### 4.1 Warna Utama

Gunakan warna yang hangat dan natural.

| Fungsi          | Warna                    | Hex       |
| --------------- | ------------------------ | --------- |
| Grass Green     | Hijau rumput utama       | `#5DBB63` |
| Deep Grass      | Hijau gelap untuk shadow | `#2E7D4F` |
| Soft Grass      | Hijau muda highlight     | `#8EDC74` |
| Dirt Path       | Tanah/jalan setapak      | `#D79B52` |
| Light Dirt      | Highlight jalan          | `#F0C070` |
| Wood Brown      | Kayu utama               | `#9A5A2E` |
| Dark Wood       | Border kayu              | `#5B321F` |
| Parchment       | Background popup         | `#F5C982` |
| Parchment Light | Isi panel terang         | `#FFD991` |
| Parchment Dark  | Shadow panel             | `#B86B36` |
| Sky Blue        | Background langit        | `#5EC7F2` |
| Deep Blue       | Langit/kontras malam     | `#1B4E89` |
| Cream Text      | Teks terang              | `#FFF1C1` |
| Dark Text       | Teks gelap               | `#4A2A1A` |

### 4.2 Warna Player

Setiap player harus punya warna identitas yang konsisten. Warna ini dipakai untuk:

* token/bidak,
* scoreboard,
* border pop-up,
* dice roll popup,
* highlight giliran,
* dan final ranking.

| Player   | Warna Normal                  | Warna Aktif                    |
| -------- | ----------------------------- | ------------------------------ |
| Player 1 | Merah gelap `#9E2F2F`         | Merah terang `#E84C4C`         |
| Player 2 | Biru gelap `#2F5DA8`          | Biru terang `#4F8CFF`          |
| Player 3 | Hijau gelap `#2F8B57`         | Hijau terang `#38D27A`         |
| Player 4 | Oranye/Kuning gelap `#B87822` | Oranye/Kuning terang `#FFC247` |

Catatan:

* Warna inactive scoreboard memakai versi gelap/redup.
* Warna active scoreboard memakai versi terang dari warna player.
* Jangan gunakan putih polos sebagai active state.
* Jangan gunakan hijau universal untuk semua active player.

---

## 5. Typography

### 5.1 Font Style

Gunakan font yang terasa pixel-art, tetapi tetap mudah dibaca.

Rekomendasi jenis font:

* Pixel-style readable font.
* Bitmap-style font.
* Rounded pixel font.
* Hindari font yang terlalu dekoratif sampai sulit dibaca.

Contoh karakteristik font:

* huruf tebal,
* bentuk kotak,
* spacing cukup lega,
* mudah dibaca pada ukuran kecil.

### 5.2 Hierarki Font

| Elemen            |   Ukuran | Style             |
| ----------------- | -------: | ----------------- |
| Judul Main Menu   | 48–72 px | Pixel bold, besar |
| Button            | 24–32 px | Pixel bold        |
| Popup Title       | 28–36 px | Pixel bold        |
| Popup Body        | 18–24 px | Pixel readable    |
| Scoreboard Name   | 18–24 px | Bold              |
| Scoreboard Detail | 14–18 px | Regular           |
| Dice Number       | 64–96 px | Bold besar        |
| Quiz Question     | 20–26 px | Bold/readable     |
| Quiz Answer       | 18–22 px | Regular           |

---

## 6. Main Menu Design

### 6.1 Konsep Main Menu

Main menu harus memberi kesan:

* hangat,
* menyenangkan,
* seperti game pixel board adventure,
* dan langsung memperlihatkan tema IPB/kampus.

Main menu menggunakan background pixel-art berupa:

* langit biru,
* pegunungan/hijau,
* pepohonan,
* area kampus/farm-like,
* jalan setapak,
* papan kayu besar untuk judul.

Judul game diletakkan pada papan kayu besar di bagian atas/tengah.

Judul:
**Ular Tangga Tata Tertib IPB**

Subjudul opsional:
**Petualangan Menjadi Mahasiswa Teladan**

### 6.2 Elemen Main Menu

Main menu minimal memiliki:

* title board kayu,
* tombol Start Game / Mulai Game,
* selector jumlah player,
* input nama player,
* tombol kembali/exit jika diperlukan.

### 6.3 Button Style

Button menggunakan gaya:

* papan kayu kecil,
* border coklat gelap,
* isi warna parchment/kayu terang,
* text pixel gelap,
* hover/active menjadi lebih terang.

State button:

* Normal: kayu terang.
* Hover: lebih cerah.
* Pressed: sedikit turun/gelap.
* Disabled: desaturated/abu kecoklatan.

### 6.4 Layout Main Menu

Gunakan layout vertikal:

1. Logo/judul papan kayu.
2. Subtitle kecil.
3. Player count selector.
4. Input nama player.
5. Tombol Mulai Game.
6. Tombol opsional.

Input nama tidak boleh menutupi tombol Mulai Game. Gunakan `VerticalLayoutGroup` dan `ContentSizeFitter` agar UI tidak overlap.

---

## 7. Board Design

### 7.1 Konsep Board

Board ular tangga harus tetap jelas sebagai papan 10x10, tetapi dibuat dengan visual pixel-art. Board dapat berbentuk:

* kotak rumput,
* jalur tanah,
* petak kayu,
* petak batu,
* atau campuran natural tile.

Papan harus terasa seperti area perjalanan mahasiswa dari awal sampai akhir.

Tile 1–100 harus tetap mudah dipahami. Nomor tile boleh kecil, tetapi tetap terbaca.

### 7.2 Layout Board

Board menggunakan layout:

* 10 x 10 petak,
* jalur serpentine/zig-zag,
* tile 1 di bawah,
* tile 100 di atas,
* start di luar board,
* finish di tile 100.

### 7.3 Tile Style

| Tipe Tile     | Visual                                     |
| ------------- | ------------------------------------------ |
| Normal Tile   | Rumput/jalan tanah/kayu terang             |
| Question Tile | Tile biru/cyan dengan ikon tanda tanya     |
| Skull Tile    | Tile gelap dengan ikon tengkorak           |
| Snake Tile    | Tile dengan tanda bahaya/jejak ular        |
| Ladder Tile   | Tile dengan ikon tangga/naik               |
| Finish Tile   | Tile spesial dengan bendera/piala/Duta IPB |

### 7.4 Random Board Visual

Karena posisi question, snake, ladder, dan skull bisa random setiap game:

* visual tile harus procedural,
* icon harus muncul sesuai tipe tile runtime,
* jangan hanya bergantung pada gambar board statis,
* snake/ladder harus dibuat sebagai overlay di atas board.

---

## 8. Character Design

### 8.1 Style Karakter

Karakter menggunakan pixel-art/chibi style agar mudah dilihat sebagai bidak.

Karakter punya 5 tahap evolusi:

1. Stage 1: Punk / mahasiswa bermasalah.
2. Stage 2: mulai rapi, rambut lebih tertata, pakaian masih berantakan.
3. Stage 3: pakaian lebih rapi, tetapi belum formal penuh.
4. Stage 4: kemeja rapi / mahasiswa tertib.
5. Stage 5: Duta IPB / winner sprite.

### 8.2 Fungsi Karakter

Karakter digunakan untuk:

* token di board,
* portrait scoreboard,
* popup status,
* final ranking.

Untuk board, karakter boleh dibuat versi token kecil:

* kepala/portrait,
* atau full body mini dengan outline warna player.

### 8.3 Player Marker

Jika karakter full-color sudah punya warna sendiri, jangan tint seluruh sprite. Gunakan:

* ring warna di bawah karakter,
* outline warna player,
* shadow glow,
* atau name tag kecil.

---

## 9. Popup Design

### 9.1 Konsep Popup

Semua popup menggunakan gaya **parchment + wooden frame**, mirip papan informasi pixel-art.

Popup harus:

* nyaman dibaca,
* tidak terlalu formal,
* punya border kayu,
* isi berwarna parchment,
* text gelap,
* dan icon kecil sesuai tipe popup.

### 9.2 Struktur Popup

Popup umum terdiri dari:

* title bar,
* content area,
* icon,
* body text,
* tombol Continue/OK,
* auto-close timer jika diperlukan.

Contoh struktur:

* Header kayu kecil.
* Isi panel parchment.
* Border coklat gelap.
* Shadow pixel sederhana.

### 9.3 Popup Normal Tile

Popup normal tile muncul ketika player menginjak tile biasa.

Title:
**Kegiatan Positif**

Body:
“Selamat, Anda mendapat X poin karena telah …”

Visual:

* icon daun/bintang kecil,
* border hijau/kayu,
* background parchment.

### 9.4 Popup Quiz

Popup quiz harus terasa seperti papan pertanyaan.

Title:
**Kuis Tata Tertib**

Elemen:

* pertanyaan,
* pilihan jawaban,
* tombol pilihan A/B/C/D atau Benar/Salah,
* feedback benar/salah.

Jawaban benar:

* aksen hijau,
* icon centang,
* sound positif.

Jawaban salah:

* aksen merah/oranye,
* icon silang,
* feedback edukatif.

### 9.5 Popup Snake / Pelanggaran

Title:

* Pelanggaran Ringan
* Pelanggaran Sedang
* Pelanggaran Berat

Visual:

* border merah/oranye,
* icon warning,
* screen shake kecil opsional,
* teks singkat dan jelas.

### 9.6 Popup Skull

Popup skull lebih dramatis:

* background lebih gelap,
* border merah tua,
* icon tengkorak/bom,
* animasi shake,
* sound impact.

Tetap jangan terlalu menyeramkan karena targetnya game edukatif.

---

## 10. Dice Roll Popup Design

### 10.1 Konsep

Dice roll popup harus modern tetapi tetap menyatu dengan pixel-art. Gunakan:

* card pixel modern,
* border kayu,
* angka dadu besar,
* accent warna player,
* animasi pop/fade.

### 10.2 Bot Rolling Indicator

Sebelum bot mendapatkan hasil dadu, tampilkan indikator:

“[Nama Bot] sedang melempar dadu…”

Visual:

* ikon dadu berputar,
* loading dots,
* panel kecil dengan accent warna bot.

Durasi minimal:

* 1 detik.

### 10.3 Hasil Dadu

Setelah roll:

* tampilkan hasil dadu selama 2 detik,
* berlaku untuk human dan bot,
* token baru bergerak setelah popup selesai.

Format:
“[Nama Player] mendapatkan”
“DADU 8”

Optional:

* Good Timing
* Perfect Timing
* Charge 72%

Dice number harus paling besar dan mudah terlihat.

---

## 11. Scoreboard Design

### 11.1 Fungsi Scoreboard

Scoreboard menampilkan:

* nama player,
* human/bot,
* posisi tile,
* status,
* sanksi,
* stage karakter,
* rank sementara jika finish.

### 11.2 Warna Scoreboard

Scoreboard harus mengikuti warna player.

Inactive:

* warna player versi redup/gelap.

Active:

* warna player versi terang.

Jangan:

* active menjadi putih,
* active menjadi hijau universal,
* mengganti warna identitas player.

### 11.3 Active Player Indicator

Saat player sedang jalan:

* card menjadi lebih terang,
* border lebih tebal,
* muncul label “GILIRAN” atau “SEDANG BERJALAN”,
* scale card naik sedikit,
* glow kecil sesuai warna player.

### 11.4 Readability

Text harus otomatis menyesuaikan background:

* background gelap → text putih/cream,
* background terang → text coklat gelap/hitam.

Gunakan helper luminance untuk menentukan warna text.

---

## 12. Ranking / Game Over UI

### 12.1 Konsep

Final ranking menggunakan panel besar parchment dengan frame kayu.

Title:
**Hasil Akhir Permainan**

Isi:

* Juara 1
* Juara 2
* Juara 3
* Belum Finish
* Drop Out jika ada

### 12.2 Visual Ranking

Juara 1:

* badge emas,
* icon piala,
* karakter winner/Duta IPB.

Juara 2:

* badge perak.

Juara 3:

* badge perunggu.

Gunakan icon pixel sederhana:

* piala,
* medali,
* bintang,
* daun IPB-style.

---

## 13. Icons and Decorations

### 13.1 Icon Style

Semua icon harus pixel-art:

* tanda tanya,
* tengkorak,
* tangga,
* ular,
* dadu,
* bintang,
* daun,
* piala,
* warning,
* centang,
* silang,
* KTM,
* buku,
* helm,
* sampah,
* rokok dilarang,
* shield keamanan.

### 13.2 Dekorasi

Dekorasi game bisa menggunakan:

* rumput,
* pohon,
* batu,
* pagar kayu,
* bunga,
* daun,
* papan penunjuk,
* lampu taman,
* bangku kampus,
* gedung kecil bergaya pixel,
* jalan tanah,
* awan,
* pegunungan,
* elemen khas kampus secara original.

Dekorasi tidak boleh mengganggu gameplay readability.

---

## 14. Animation Direction

Animasi harus ringan dan playful.

Gunakan:

* token move step-by-step,
* popup scale/fade,
* button bounce kecil,
* dice shake,
* bot rolling spinner,
* snake slide down,
* ladder climb up,
* skull shake/bomb,
* finish sparkle.

Durasi animasi:

* button press: 0.1–0.15 detik,
* popup in/out: 0.15–0.25 detik,
* token step: 0.12–0.2 detik per tile,
* dice result popup: 2 detik,
* gameplay popup: 5 detik.

---

## 15. Audio Direction

Audio mengikuti nuansa cozy pixel game.

### 15.1 BGM

BGM:

* ringan,
* cheerful,
* loopable,
* tidak mengganggu fokus.

Scene:

* Main Menu: musik santai dan cerah.
* Gameplay: musik playful.
* Final Ranking: musik kemenangan ringan.

### 15.2 SFX

SFX:

* button click: kayu kecil / pop.
* dice roll: dadu kayu.
* token step: tap ringan.
* ladder: ascending chime.
* snake: slide/down tone.
* quiz correct: bell positif.
* quiz wrong: soft buzzer.
* skull: impact kecil.
* finish: fanfare pendek.

---

## 16. UX Principles

### 16.1 Readability First

Walaupun pixel-art, UI harus tetap jelas.

Pastikan:

* text cukup besar,
* contrast cukup,
* button mudah diklik,
* popup tidak terlalu penuh,
* icon mudah dikenali,
* informasi gameplay tidak tersembunyi.

### 16.2 Mobile Friendly

UI harus aman untuk desktop dan mobile/WebGL.

Gunakan:

* Canvas Scaler: Scale With Screen Size.
* Button minimal 48 px height.
* Input field tidak overlap.
* Layout group untuk menu.
* ScrollView jika layar kecil.

### 16.3 Feedback Jelas

Setiap aksi harus punya feedback:

* siapa yang jalan,
* dadu berapa,
* kenapa naik/turun,
* kenapa kena sanksi,
* siapa yang finish,
* siapa yang sedang menjawab kuis.

---

## 17. Unity Implementation Notes

### 17.1 Pixel Perfect

Gunakan:

* Pixel Perfect Camera jika gameplay memakai sprite pixel-art.
* Filter Mode: Point untuk sprite pixel-art.
* Compression: None atau Low.
* Pixels Per Unit konsisten.

Untuk UI text, boleh gunakan TextMeshPro dengan pixel-style font agar tetap tajam.

### 17.2 Asset Import Setting

Untuk sprite pixel-art:

* Texture Type: Sprite (2D and UI)
* Filter Mode: Point (no filter)
* Compression: None
* Max Size: sesuai kebutuhan
* Pixels Per Unit: konsisten, misalnya 16, 32, atau 100 tergantung sistem board.

Untuk UI panel:

* Gunakan 9-slice sprite agar frame kayu/parchment bisa di-stretch tanpa rusak.

### 17.3 UI Scaling

Gunakan:

* Canvas Scaler: Scale With Screen Size
* Reference Resolution: 1920 x 1080
* Match: 0.5

Untuk mobile:

* pastikan layout tidak overlap,
* gunakan safe area,
* gunakan scroll jika konten panjang.

---

## 18. Asset Naming Convention

Gunakan naming yang rapi.

### 18.1 UI

```text
ui_panel_parchment_large.png
ui_panel_parchment_small.png
ui_frame_wood_01.png
ui_button_wood_normal.png
ui_button_wood_hover.png
ui_button_wood_pressed.png
ui_icon_dice.png
ui_icon_question.png
ui_icon_skull.png
ui_icon_warning.png
ui_icon_check.png
ui_icon_cross.png
```

### 18.2 Board

```text
tile_normal_grass_01.png
tile_normal_dirt_01.png
tile_question_01.png
tile_skull_01.png
tile_ladder_marker_01.png
tile_snake_marker_01.png
tile_finish_01.png
board_decoration_tree_01.png
board_decoration_rock_01.png
board_decoration_flower_01.png
```

### 18.3 Character

```text
character_01_stage_01.png
character_01_stage_02.png
character_01_stage_03.png
character_01_stage_04.png
character_01_winner.png
```

### 18.4 Audio

```text
bgm_main_menu_loop.wav
bgm_gameplay_loop.wav
sfx_button_click.wav
sfx_dice_roll.wav
sfx_token_step.wav
sfx_quiz_correct.wav
sfx_quiz_wrong.wav
sfx_finish.wav
```

---

## 19. Do and Don’t

### Do

* Gunakan warna hangat dan natural.
* Gunakan frame kayu dan panel parchment.
* Buat UI terasa cozy.
* Pastikan semua text readable.
* Gunakan icon pixel sederhana.
* Jaga konsistensi warna player.
* Buat popup informatif tapi singkat.
* Buat animasi ringan.
* Buat layout aman untuk mobile.

### Don’t

* Jangan menyalin asset game lain.
* Jangan membuat UI terlalu modern glossy seperti aplikasi bisnis.
* Jangan memakai warna neon berlebihan.
* Jangan membuat text terlalu kecil.
* Jangan membuat popup terlalu penuh.
* Jangan mengganti warna player aktif menjadi putih.
* Jangan membuat dekorasi mengganggu board.
* Jangan membuat main menu terlalu ramai.

---

## 20. Design Checklist

Sebelum build, cek:

* [ ] Main menu sudah terasa pixel-art/cozy.
* [ ] Tombol Mulai Game tidak tertutup input nama.
* [ ] Board 10x10 mudah dibaca.
* [ ] Nomor tile terlihat.
* [ ] Special tile mudah dikenali.
* [ ] Scoreboard warna player konsisten.
* [ ] Active player jelas terlihat.
* [ ] Dice popup muncul 2 detik.
* [ ] Bot rolling indicator terlihat.
* [ ] Popup gameplay memakai parchment/wood frame.
* [ ] Quiz popup mudah dibaca.
* [ ] Final ranking terlihat seperti hasil akhir game.
* [ ] Text readable di desktop dan mobile.
* [ ] Asset tidak memakai gambar/game lain secara langsung.
* [ ] Semua UI aman di WebGL.
* [ ] Semua asset punya nama file rapi.

---

## 21. Optional Additional Files

Selain `design.md`, disarankan membuat file tambahan:

```text
art-style-guide.md
ui-components.md
asset-list.md
animation-guide.md
audio-guide.md
```

Jika project masih kecil, semua bisa tetap digabung di `design.md`. Jika project makin besar, pecah menjadi beberapa file agar mudah dibaca oleh developer dan AI coding agent.

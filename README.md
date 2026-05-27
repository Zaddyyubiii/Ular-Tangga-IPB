# Ular Tangga Tata Tertib IPB University

Game Unity 2D edukasi berbasis ular tangga untuk mata kuliah KOM1304 - Grafika Komputer dan Visualisasi. Pemain bergerak dari petak 0 menuju petak 100 untuk menjadi Duta Tata Tertib IPB University.

## Cara Play

1. Buka project ini di Unity 6 atau versi yang kompatibel.
2. Jika scene belum tersusun, jalankan menu `Ular Tangga > Setup Playable Scenes`.
3. Buka `Assets/Scenes/MainMenuScene.unity`, lalu tekan Play.
4. Pilih jumlah pemain nyata 2 sampai 4. Slot kosong otomatis diisi Bot.
5. Klik `Start Game`, baca prolog, lalu klik `Mulai Perjalanan`.
6. Saat giliran human, tahan tombol `ROLL` atau tombol `Space`, lalu lepaskan untuk menentukan nilai dadu. Bot dan timer habis akan melakukan auto roll.

## Gameplay

- Target menang adalah mendarat tepat di petak 100.
- Jika roll melewati 100, pemain tetap di posisi semula.
- Tile normal menampilkan pesan kegiatan positif.
- Tile tanda tanya menampilkan kuis dan feedback edukatif.
- Tile tengkorak memberi 1 skip turn, menurunkan pemain 40 petak, dan menambah hit sanksi. Hit ketiga membuat pemain Drop Out.
- Tile ular menurunkan pemain sesuai konfigurasi.
- Tile tangga menaikkan pemain sesuai konfigurasi.
- Sprite evolusi berubah otomatis pada rentang 0-25, 26-50, 51-75, 76-99, dan 100.

## Mengganti Asset Main Menu

1. Masukkan gambar ke `Assets/Art/MainMenu/`.
2. Buka `MainMenuScene`.
3. Pilih `CanvasMainMenu/Background`.
4. Isi `Source Image` pada komponen `Image` dengan sprite background final.

## Mengganti Asset Papan

1. Masukkan gambar papan 10x10 ke `Assets/Art/Board/`.
2. Buka `GameScene`.
3. Pilih `CanvasGameplay/BoardPanel`.
4. Isi `Source Image` pada komponen `Image`.
5. Jika ukuran board berubah, sesuaikan `Tile Width`, `Tile Height`, `Start X`, dan `Start Y` pada `BoardManager`.

## Mengganti Sprite Karakter

1. Masukkan sprite karakter ke `Assets/Art/Characters/`.
2. Buat asset lewat `Create > UlarTangga > PlayerSpriteSet`.
3. Simpan di `Assets/ScriptableObjects/PlayerSpriteSets/`.
4. Isi `Stage 1`, `Stage 2`, `Stage 3`, `Stage 4`, dan `Winner Sprite`.
5. Pasang 4 set karakter ke list `Character Sets` pada `PlayerEvolutionController` di `GameScene`.

## Mengubah Board Config

Buka `Assets/ScriptableObjects/BoardConfig.asset`.

- `Question Tiles`: posisi tile kuis.
- `Skull Tiles`: posisi tile tengkorak.
- `Snakes Configurations`: start tile, target tile, severity, dan pesan ular.
- `Ladders Configurations`: start tile, target tile, severity, dan pesan tangga.

BoardConfig punya validasi runtime sederhana untuk memperingatkan tile dengan efek ganda atau transisi di luar rentang.

## Menambah Quiz

Buka `Assets/ScriptableObjects/QuizBank.asset`, lalu tambah item pada list `Questions`.

Field penting:

- `Question Text`
- `Choices`
- `Correct Answer Index`
- `Correct Feedback`
- `Incorrect Feedback`
- `Reward Tiles` dan `Penalty Tiles` untuk pengembangan berikutnya

## Menambah Pesan Tile

Buka `Assets/ScriptableObjects/MessageBank.asset`.

Edit:

- `Prologue Text`
- `Positive Messages`
- `Snake Messages`
- `Skull Messages`
- `Ladder Messages`

## Audio

Masukkan BGM ke `Assets/Audio/BGM/` dan SFX ke `Assets/Audio/SFX/`, lalu isi field pada `AudioManager`. Semua clip aman dibiarkan kosong.

## Build WebGL

1. Buka `File > Build Profiles` atau `File > Build Settings`.
2. Pilih platform `WebGL`.
3. Klik `Switch Platform`.
4. Pastikan scene `MainMenuScene` dan `GameScene` ada di Build Settings.
5. Klik `Build`.

Canvas sudah memakai `Scale With Screen Size`, dan input memakai UI pointer sehingga nyaman untuk mouse maupun touch.

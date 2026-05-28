# Aturan & Mekanisme Permainan (Game Rules)

Dokumen ini berisi rangkuman aturan main, mekanika kontrol dadu, sistem sanksi, dan evolusi karakter yang diimplementasikan dalam game **Ular Tangga Tata Tertib IPB University**.

---

## 🎮 Konsep Utama Game
Game ini menyimulasikan perjalanan karakter mahasiswa dari **Petak 0** (belum mematuhi tata tertib) menuju **Petak 100** (Duta Tata Tertib IPB University). Game dimainkan secara giliran bergantian (*turn-based local multiplayer*) oleh 2 hingga 4 pemain nyata. Jika slot tidak penuh, sistem secara otomatis akan memunculkan **Bot AI** untuk mengisi slot yang kosong.

---

## 🎲 Mekanika Pengocokan Dadu (Dice Gauge System)
Untuk meminimalisir faktor keberuntungan murni (*RNG*), game menggunakan **Dice Gauge System** (seperti pada game *LINE: Let's Get Rich!*). Pemain menahan tombol `ROLL` atau tombol `Space` untuk mengisi bar daya (*charge percent*), lalu melepasnya pada waktu yang tepat untuk menargetkan rentang angka dadu tertentu:
*   **Daya 0% - 25%**: Target dadu **2, 3** (Level 1: Status Percobaan)
*   **Daya 26% - 50%**: Target dadu **4, 5, 6** (Level 2: Mahasiswa Reguler)
*   **Daya 51% - 75%**: Target dadu **7, 8, 9** (Level 3: Mahasiswa Teladan)
*   **Daya 76% - 100%**: Target dadu **10, 11, 12** (Level 4: Duta Tata Tertib)

*Catatan: Setiap giliran dibatasi oleh **timer 10 detik**. Jika timer habis sebelum tombol dilepas, dadu akan dikocok secara otomatis oleh sistem.*

---

## 🏁 Kondisi Kemenangan (Winner Condition)
*   Pemenang adalah pemain pertama yang mendarat **tepat di Petak 100**.
*   Jika hasil dadu membuat langkah pemain **melebihi Petak 100**, sisa langkah akan hangus dan pemain **tetap diam di posisi semula** tanpa bergerak maju.

---

## 🗺️ Spesifikasi dan Fungsi Petak (Tiles)

### 1. Petak Biasa (Normal Tiles)
Menampilkan jendela sembul (*pop-up*) pesan mengenai kegiatan positif mahasiswa sehari-hari di kampus IPB dengan format:
> *"Selamat, Anda telah mendapatkan [jumlah dadu] poin dikarenakan [alasan]"*
*   **Poin +1**: Contoh: Membuang bungkus makanan berserakan di depan GWW ke tempat sampah.
*   **Poin +3**: Contoh: Membantu kakak tingkat mengangkut peralatan praktikum dari LSI ke Fakultas Pertanian.
*   **Poin +6**: Contoh: Mengembalikan dompet penuh uang milik dosen yang tertinggal di kantin Stekpi secara utuh.

### 2. Petak Tanda Tanya (Quiz Tiles - 6 Kotak)
Memicu jendela sembul berisi **pertanyaan pilihan ganda seputar Tata Tertib IPB**.
*   Bank soal berisi pertanyaan statis untuk menghindari galat logis.
*   Pemain mendapatkan feedback edukatif setelah menjawab.
*   *Reward* dan *Penalty* khusus dapat dikonfigurasi dalam berkas bank soal kuis.

### 3. Petak Tengkorak (Skull Tiles - 3 Kotak)
Merepresentasikan **pelanggaran akademik berat** (Skorsing).
*   **Sanksi**: Pemain dibekukan/dilewati (*skip*) sebanyak **1 kali giliran** dan diturunkan sebanyak **4 baris** (40 petak).
*   Mendatangkan visualisasi animasi bom dengan pesan *"Pelanggaran Berat! [Alasan]"*.
*   **Drop Out (DO)**: Jika pemain yang sama mendarat di petak tengkorak sebanyak **3 kali** (akumulatif), pemain tersebut langsung dinyatakan **Drop Out** (gugur/kalah seketika).

### 4. Petak Ular (Snakes)
Merepresentasikan hambatan studi atau pelanggaran ringan-sedang di kampus.
*   **Tingkat 1 (Ular Pendek)**: Turun 1 baris (10 petak). Contoh: Parkir sembarangan (Pelanggaran Ringan).
*   **Tingkat 2 (Ular Sedang)**: Turun 2 baris (20 petak). Contoh: Merusak fasilitas IPB (Pelanggaran Sedang).
*   **Tingkat 3 (Ular Panjang)**: Turun 4 baris (40 petak). Contoh: Membawa senjata tajam di lingkungan kampus (Pelanggaran Berat).

### 5. Petak Tangga (Ladders)
Merepresentasikan pencapaian akademik mahasiswa.
*   **Tingkat 1 (Tangga Pendek)**: Naik 1 baris (10 petak). Contoh: Juara 1 lomba debat departemen di tingkat Fakultas / Proposal proyek sosial lolos pendanaan BEM.
*   **Tingkat 2 (Tangga Sedang)**: Naik 2 baris (20 petak). Contoh: Karya tulis ilmiah memenangkan Juara 1 tingkat Nasional di kompetisi Inovasi Pertanian.
*   **Tingkat 3 (Tangga Panjang)**: Naik 3 baris (30 petak).

---

## 👕 Sistem Evolusi Karakter (Sprite Evolution)
Tampilan visual bidak karakter pemain akan berubah secara dinamis berdasarkan posisi petak saat ini untuk melambangkan perkembangan kedisiplinan dan profesionalisme:
1.  **Petak 0 - 25 (Tingkat 1)**: Gaya **Punk** (rambut acak-acakan, jaket penuh patch).
2.  **Petak 26 - 50 (Tingkat 2)**: Mulai rapi, gaya rambut rapi tetapi pakaian masih berantakan (kaos kerah/kemeja belum dimasukkan).
3.  **Petak 51 - 75 (Tingkat 3)**: Pakaian rapi, namun masih menggunakan kaos oblong (atau berjaket Almamater kebanggaan).
4.  **Petak 76 - 99 (Tingkat 4)**: Menggunakan **kemeja rapi secara penuh**.
5.  **Petak 100 (Tingkat 5 / Selesai)**: Karakter pemenang dengan jas rapi dan selempang **"Duta IPB"** (Menggunakan sprite pemenang khusus).

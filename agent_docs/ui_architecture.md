# Arsitektur Komunikasi & Integrasi React UI Overlay (WebGL)

Dokumen ini menjelaskan rancangan sistem, jembatan komunikasi, serta kontrak pertukaran data (*data contracts*) antara mesin game **Unity C#** dan pembungkus **React HUD Overlay** yang melayang di atas Canvas browser.

---

## 📌 Aliran Data Komunikasi (WebGL Bridge)

Komunikasi antara Unity dan React bersifat **asinkron dua-arah** memanfaatkan Javascript Native API di browser.

```mermaid
graph TD
    subgraph Browser DOM (HTML/JS)
        React[React HUD Overlay - Tailwind CSS]
        Canvas[Unity WebGL Canvas]
        BridgeJS[window.dispatchEvent & SendMessage]
    end

    subgraph Unity Engine (C#)
        CColor[Core Game Loop & Board]
        JSLib[ReactBridge.jslib Plugin]
        CReceiver[ReactReceiver.cs]
    end

    %% Communication flow
    CColor -- DllImport --> JSLib
    JSLib -- CustomEvent --> BridgeJS
    BridgeJS -- Listen & Update State --> React
    
    React -- Click Actions --> BridgeJS
    BridgeJS -- SendMessage --> CReceiver
    CReceiver -- Trigger Logic --> CColor
```

### 1. Unity Ke React (Mengirim State Game)
Unity mengirimkan data state permainan dalam format string JSON ke browser melalui plugin `.jslib` (`Assets/Plugins/WebGL/ReactBridge.jslib`). Skrip ini mengubah data mentah C# menjadi `CustomEvent` Javascript pada objek `window` browser. React menangkap event tersebut menggunakan React Hook `useEffect` dan memperbarui state lokal secara instan.

### 2. React Ke Unity (Memanggil Perintah Game)
React mengirim pesan aksi pemain (seperti mengocok dadu, menjawab kuis, memulai ulang game) kembali ke Unity menggunakan pemanggilan metode global Unity WebGL Instance:
```javascript
window.unityInstance.SendMessage("ReactReceiver", "NamaMetode", "ParameterStringAtauFloat");
```
Pesan ini diterima oleh objek `"ReactReceiver"` di Unity (`Assets/Scripts/UI/ReactReceiver.cs`) untuk dieksekusi secara instan pada game loop.

---

## 📝 Kontrak Pertukaran Data (JSON Schema)

### 1. Game State (`UnityStateUpdated`)
Dikirim setiap pergantian giliran pemain, pembaruan timer giliran, atau pergerakan bidak di papan.

*   **CustomEvent Name:** `"UnityStateUpdated"`
*   **JSON Payload Schema:**
```json
{
  "activePlayerId": 1,
  "timerRemaining": 10.0,
  "instructionText": "Tahan tombol ROLL, lepaskan untuk melempar dadu!",
  "showDiceResult": true,
  "diceRollerName": "Zaddy",
  "diceValue": 6,
  "diceTimingQuality": "Perfect Timing!",
  "diceChargePercent": 85.0,
  "players": [
    {
      "id": 1,
      "playerName": "Zaddy",
      "isBot": false,
      "currentTile": 15,
      "skullHitCount": 1,
      "skipTurns": 0,
      "isDroppedOut": false,
      "isFinished": false,
      "currentEvolutionStage": 1,
      "playerColorHex": "#00ccff",
      "status": "SEDANG BERJALAN",
      "isActive": true
    },
    {
      "id": 2,
      "playerName": "Bot Budi",
      "isBot": true,
      "currentTile": 5,
      "skullHitCount": 0,
      "skipTurns": 0,
      "isDroppedOut": false,
      "isFinished": false,
      "currentEvolutionStage": 0,
      "playerColorHex": "#e63e27",
      "status": "Menunggu",
      "isActive": false
    }
  ]
}
```

### 2. Quiz State (`UnityShowQuiz`)
Dikirim saat bidak mendarat di petak kuis akademik, memicu modal dialog kuis interaktif di React.

*   **CustomEvent Name:** `"UnityShowQuiz"`
*   **JSON Payload Schema:**
```json
{
  "questionText": "Apakah mahasiswa IPB yang terlambat menyerahkan KRS akan dikenai sanksi administratif skors akademik?",
  "choices": [
    "Benar / Setuju",
    "Salah / Tidak Setuju"
  ],
  "correctAnswerIndex": 0,
  "correctFeedback": "Benar! Menurut Pasal 12 Ketentuan Tata Tertib IPB, mahasiswa wajib mengumpulkan berkas KRS tepat waktu agar tidak terhambat administrasi kelas.",
  "incorrectFeedback": "Kurang tepat. Terlambat menyerahkan KRS tanpa alasan sah dapat berakibat pada pembatalan mata kuliah semester bersangkutan."
}
```

### 3. Prologue State (`UnityShowPrologue`)
Dikirim di awal permainan untuk menampilkan visual narasi cerita latar belakang.

*   **CustomEvent Name:** `"UnityShowPrologue"`
*   **JSON Payload Schema:**
```json
{
  "narrationText": "Selamat datang mahasiswa baru di IPB University! Kalian akan memulai perjalanan memahami Ketentuan Tata Tertib dan Disiplin Kemahasiswaan..."
}
```

### 4. Game Over State (`UnityShowGameOver`)
Dikirim ketika salah satu pemain berhasil menyentuh petak ke-100 (Finish) dan memenangkan permainan.

*   **CustomEvent Name:** `"UnityShowGameOver"`
*   **JSON Payload Schema:**
```json
{
  "winnerName": "Zaddy",
  "winnerColorHex": "#00ccff",
  "messageText": "Selamat! Kamu berhasil melewati semua tantangan, mematuhi tata tertib, dan dinobatkan menjadi Duta Tata Tertib IPB University!"
}
```

### 5. Quiz Answered State (`UnityQuizAnswered`)
Dikirim ketika kuis dijawab (terutama berguna untuk sinkronisasi otomatis giliran bot).

*   **CustomEvent Name:** `"UnityQuizAnswered"`
*   **JSON Payload Schema:**
```json
{
  "selectedIndex": 0
}
```

### 6. Quiz Closed State (`UnityCloseQuiz`)
Dikirim ketika panel penjelasan kuis ditutup oleh Unity untuk menghapus modal kuis di React.

*   **CustomEvent Name:** `"UnityCloseQuiz"`
*   **JSON Payload Schema:** *Tanpa Payload*

### 7. Main Menu Loaded State (`UnityMainMenuLoaded`)
Dikirim ketika scene menu utama berhasil dimuat untuk menginstruksikan React mereset seluruh overlay HUD permainan.

*   **CustomEvent Name:** `"UnityMainMenuLoaded"`
*   **JSON Payload Schema:** *Tanpa Payload*

---

## 🕹️ API Penerima Unity C# (`ReactReceiver.cs`)

Berikut adalah fungsi-fungsi publik di Unity yang dipanggil oleh React melalu `window.unityInstance.SendMessage("ReactReceiver", "Fungsi", Parameter)`:

| Nama Fungsi di Unity | Tipe Parameter | Deskripsi Fungsi |
| :--- | :--- | :--- |
| `OnRollDice` | `float` | Mengirim daya kocokan dadu (0 s.d 100) dan melempar dadu secara fisik di papan. |
| `OnAnswerQuiz` | `string` | Mengirim jawaban pilihan ganda pemain (`"A"` untuk indeks 0, atau `"B"` untuk indeks 1). |
| `OnCloseQuizFeedback` | *Tanpa Parameter* | Mengonfirmasi penutupan panel penjelasan kuis untuk melanjutkan giliran. |
| `OnStartJourney` | *Tanpa Parameter* | Mengonfirmasi selesainya pembacaan prolog cerita untuk melepaskan bidak di petak 0. |
| `OnPlayAgain` | *Tanpa Parameter* | Meminta restart game dari awal scene utama dengan pemain yang sama. |
| `OnReturnToMenu` | *Tanpa Parameter* | Meminta memuat ulang Scene Menu Utama (`MainMenuScene`). |

---

## 💅 Desain & Skema CSS (Pointer Events Overlay)

Agar pengguna tetap bisa mengklik papan Unity 3D di belakang layer web React HUD, kita menerapkan teknik **Pointer Events Overlay** terpadu:

1.  **Akar Panel React (`App.jsx`):**
    ```css
    pointer-events-none select-none
    ```
    Ini membuat area kosong di layar React menjadi tidak sensitif klik, sehingga seluruh input klik mouse menembus secara langsung ke papan Unity di bawahnya.
2.  **Komponen Interaktif React (Kartu Pemain, Tombol, Modal):**
    ```css
    pointer-events-auto
    ```
    Setiap tombol, form input kuis, atau kartu status yang membutuhkan klik/hover wajib memiliki kelas `pointer-events-auto` agar input mouse tertahan secara sempurna dan tidak bocor ke Unity canvas.

using System.Collections.Generic;
using UnityEngine;

namespace Quiz
{
    [CreateAssetMenu(fileName = "QuizBank", menuName = "UlarTangga/QuizBank")]
    public class QuizBank : ScriptableObject
    {
        public List<QuizQuestion> questions = new List<QuizQuestion>()
        {
            new QuizQuestion
            {
                id = "quiz_old_1",
                type = "TRUE_FALSE",
                category = "Akademik",
                questionText = "Menurut Peraturan Akademik, mahasiswa diperbolehkan memakai kaos oblong tak berkerah dan sandal jepit saat mengikuti UTS.",
                isTrueFalse = true,
                choices = new string[] { "Benar", "Salah" },
                correctAnswerIndex = 1, // "Salah" is index 1
                correctFeedback = "Benar. Saat kegiatan akademik resmi, mahasiswa harus berpakaian sopan dan sesuai ketentuan.",
                incorrectFeedback = "Kurang tepat. Pakaian saat kegiatan akademik resmi harus mengikuti aturan yang berlaku (berkerah dan bersepatu)."
            },
            new QuizQuestion
            {
                id = "quiz_old_2",
                type = "MULTIPLE_CHOICE",
                category = "Identitas Mahasiswa",
                questionText = "Apa nama masa persiapan untuk mahasiswa baru pada tahun pertama di IPB?",
                isTrueFalse = false,
                choices = new string[] { "PPKU", "TPB" },
                correctAnswerIndex = 0, // "PPKU" is index 0
                correctFeedback = "Benar. PPKU (Program Pendidikan Kompetensi Umum) adalah masa persiapan awal mahasiswa IPB.",
                incorrectFeedback = "Kurang tepat. Jawaban yang benar adalah PPKU (Program Pendidikan Kompetensi Umum)."
            },
            new QuizQuestion
            {
                id = "quiz_old_3",
                type = "MULTIPLE_CHOICE",
                category = "Akademik",
                questionText = "Berapa batas waktu maksimal masa studi untuk program Sarjana (S1) reguler di IPB?",
                isTrueFalse = false,
                choices = new string[] { "8 Semester", "14 Semester" },
                correctAnswerIndex = 1, // "14 Semester" is index 1
                correctFeedback = "Benar. Batas maksimal masa studi S1 reguler adalah 14 semester.",
                incorrectFeedback = "Kurang tepat. Jawaban yang benar adalah 14 semester (7 tahun)."
            },
            new QuizQuestion
            {
                id = "quiz_001",
                type = "TRUE_FALSE",
                category = "Kewajiban Umum",
                questionText = "Mahasiswa IPB wajib menjaga kehidupan akademik yang mengutamakan kebenaran dan kejujuran.",
                isTrueFalse = true,
                choices = new string[] { "Benar", "Salah" },
                correctAnswerIndex = 0,
                correctFeedback = "Benar! Mahasiswa wajib menjaga kejujuran dan kebenaran dalam kehidupan akademik.",
                incorrectFeedback = "Kurang tepat. Mahasiswa IPB wajib menjunjung kebenaran dan kejujuran akademik."
            },
            new QuizQuestion
            {
                id = "quiz_002",
                type = "MULTIPLE_CHOICE",
                category = "Identitas Mahasiswa",
                questionText = "Saat beraktivitas di lingkungan kampus, mahasiswa wajib membawa apa sebagai identitas resmi?",
                isTrueFalse = false,
                choices = new string[] { "Kartu ATM", "KTM", "Kartu perpustakaan", "Kartu parkir" },
                correctAnswerIndex = 1,
                correctFeedback = "Benar! KTM adalah identitas resmi mahasiswa IPB.",
                incorrectFeedback = "Kurang tepat. Identitas resmi yang wajib dibawa adalah KTM."
            },
            new QuizQuestion
            {
                id = "quiz_003",
                type = "MULTIPLE_CHOICE",
                category = "Lingkungan Kampus",
                questionText = "Prinsip pengelolaan sampah yang wajib dijalankan dalam kegiatan kemahasiswaan adalah...",
                isTrueFalse = false,
                choices = new string[] { "Membuang, membakar, menimbun", "Mengurangi, menggunakan kembali, mendaur ulang", "Mengumpulkan, menyimpan, membuang", "Membeli, memakai, membuang" },
                correctAnswerIndex = 1,
                correctFeedback = "Benar! Prinsipnya adalah pengurangan, penggunaan kembali, dan pendaurulangan sampah.",
                incorrectFeedback = "Kurang tepat. Prinsip yang benar adalah mengurangi, menggunakan kembali, dan mendaur ulang."
            },
            new QuizQuestion
            {
                id = "quiz_004",
                type = "TRUE_FALSE",
                category = "Akademik",
                questionText = "Menyontek dan bekerja sama saat ujian termasuk pelanggaran tata tertib akademik.",
                isTrueFalse = true,
                choices = new string[] { "Benar", "Salah" },
                correctAnswerIndex = 0,
                correctFeedback = "Benar! Menyontek dan bekerja sama tidak sah saat ujian termasuk pelanggaran akademik.",
                incorrectFeedback = "Kurang tepat. Menyontek dan bekerja sama saat ujian dilarang dalam tata tertib akademik."
            },
            new QuizQuestion
            {
                id = "quiz_005",
                type = "MULTIPLE_CHOICE",
                category = "Integritas Akademik",
                questionText = "Tindakan mengambil gagasan, data, atau tulisan orang lain tanpa menyebutkan sumber secara memadai disebut...",
                isTrueFalse = false,
                choices = new string[] { "Presensi", "Perjokian", "Plagiat", "Praktikum" },
                correctAnswerIndex = 2,
                correctFeedback = "Benar! Menggunakan karya atau gagasan orang lain tanpa sumber yang memadai termasuk plagiat.",
                incorrectFeedback = "Kurang tepat. Tindakan tersebut disebut plagiat."
            },
            new QuizQuestion
            {
                id = "quiz_006",
                type = "MULTIPLE_CHOICE",
                category = "Akademik",
                questionText = "Menggantikan atau digantikan orang lain dalam ujian, praktikum, atau kegiatan akademik disebut...",
                isTrueFalse = false,
                choices = new string[] { "Perjokian", "Presensi", "Konsultasi", "Kolaborasi" },
                correctAnswerIndex = 0,
                correctFeedback = "Benar! Menggantikan atau digantikan dalam kegiatan akademik termasuk perjokian.",
                incorrectFeedback = "Kurang tepat. Istilah yang benar adalah perjokian."
            },
            new QuizQuestion
            {
                id = "quiz_007",
                type = "TRUE_FALSE",
                category = "Busana dan Penampilan",
                questionText = "Mahasiswa boleh memakai sandal dalam kegiatan belajar mengajar biasa di lingkungan kampus.",
                isTrueFalse = true,
                choices = new string[] { "Benar", "Salah" },
                correctAnswerIndex = 1,
                correctFeedback = "Benar! Sandal tidak diperbolehkan dalam kegiatan belajar mengajar biasa, kecuali pada aktivitas tertentu seperti olahraga atau praktikum lapang.",
                incorrectFeedback = "Kurang tepat. Sandal tidak diperbolehkan dalam kegiatan belajar mengajar biasa."
            },
            new QuizQuestion
            {
                id = "quiz_008",
                type = "MULTIPLE_CHOICE",
                category = "Busana dan Penampilan",
                questionText = "Pengecualian penggunaan t-shirt tidak berkerah dan sandal dapat berlaku saat mahasiswa sedang...",
                isTrueFalse = false,
                choices = new string[] { "Mengikuti ujian tengah semester", "Mengikuti rapat resmi", "Melakukan olahraga atau praktikum lapang", "Mengurus administrasi akademik" },
                correctAnswerIndex = 2,
                correctFeedback = "Benar! Pengecualian dapat berlaku untuk aktivitas olahraga atau praktikum lapang.",
                incorrectFeedback = "Kurang tepat. Pengecualian berlaku untuk aktivitas olahraga atau praktikum lapang."
            },
            new QuizQuestion
            {
                id = "quiz_009",
                type = "A_B",
                category = "Ketertiban Kampus",
                questionText = "Kegiatan di lingkungan kampus pada pukul 23.00 tanpa izin tertulis termasuk tindakan yang...",
                isTrueFalse = true,
                choices = new string[] { "Diperbolehkan", "Dilarang" },
                correctAnswerIndex = 1,
                correctFeedback = "Benar! Kegiatan di atas pukul 22.00 sampai 06.00 WIB memerlukan pengecualian/izin tertentu.",
                incorrectFeedback = "Kurang tepat. Kegiatan di atas pukul 22.00 sampai 06.00 WIB tidak boleh dilakukan tanpa izin tertentu."
            },
            new QuizQuestion
            {
                id = "quiz_010",
                type = "MULTIPLE_CHOICE",
                category = "Lalu Lintas Kampus",
                questionText = "Batas kecepatan kendaraan yang disebut dalam aturan berkendara di lingkungan kampus adalah...",
                isTrueFalse = false,
                choices = new string[] { "Di atas 10 km/jam", "Di atas 20 km/jam", "Di atas 30 km/jam", "Di atas 60 km/jam" },
                correctAnswerIndex = 2,
                correctFeedback = "Benar! Aturan menyebut pelanggaran seperti kecepatan di atas 30 km/jam.",
                incorrectFeedback = "Kurang tepat. Batas yang disebut adalah kecepatan di atas 30 km/jam."
            },
            new QuizQuestion
            {
                id = "quiz_011",
                type = "TRUE_FALSE",
                category = "Fasilitas dan Lingkungan",
                questionText = "Membuang sampah tidak pada tempatnya dan mencoret fasilitas kampus termasuk tindakan yang dilarang.",
                isTrueFalse = true,
                choices = new string[] { "Benar", "Salah" },
                correctAnswerIndex = 0,
                correctFeedback = "Benar! Tindakan yang merusak kebersihan, keindahan, dan fasilitas kampus dilarang.",
                incorrectFeedback = "Kurang tepat. Membuang sampah sembarangan dan mencoret fasilitas kampus termasuk tindakan yang dilarang."
            },
            new QuizQuestion
            {
                id = "quiz_012",
                type = "MULTIPLE_CHOICE",
                category = "Fasilitas dan Lingkungan",
                questionText = "Fasilitas pendidikan IPB boleh digunakan untuk kegiatan selain perkuliahan apabila...",
                isTrueFalse = false,
                choices = new string[] { "Dilakukan malam hari", "Mendapat izin pimpinan terkait", "Pesertanya banyak", "Tidak ada dosen yang melihat" },
                correctAnswerIndex = 1,
                correctFeedback = "Benar! Penggunaan fasilitas pendidikan untuk kegiatan lain harus mendapat izin pimpinan terkait.",
                incorrectFeedback = "Kurang tepat. Fasilitas pendidikan harus digunakan sesuai izin pimpinan terkait."
            },
            new QuizQuestion
            {
                id = "quiz_013",
                type = "TRUE_FALSE",
                category = "Kampus Sehat",
                questionText = "Merokok elektrik, vape, dan sejenisnya dilarang di lingkungan kampus.",
                isTrueFalse = true,
                choices = new string[] { "Benar", "Salah" },
                correctAnswerIndex = 0,
                correctFeedback = "Benar! Rokok tradisional, rokok elektrik, vape, dan sejenisnya dilarang di lingkungan kampus.",
                incorrectFeedback = "Kurang tepat. Vape dan rokok elektrik juga termasuk yang dilarang di lingkungan kampus."
            },
            new QuizQuestion
            {
                id = "quiz_014",
                type = "MULTIPLE_CHOICE",
                category = "Keamanan Kampus",
                questionText = "Manakah tindakan berikut yang termasuk dilarang karena dapat membahayakan keselamatan?",
                isTrueFalse = false,
                choices = new string[] { "Membawa KTM", "Membawa senjata tajam", "Memakai sepatu", "Membuang sampah pada tempatnya" },
                correctAnswerIndex = 1,
                correctFeedback = "Benar! Membawa senjata tajam atau sejenisnya dilarang karena membahayakan keselamatan.",
                incorrectFeedback = "Kurang tepat. Tindakan yang dilarang adalah membawa senjata tajam."
            },
            new QuizQuestion
            {
                id = "quiz_015",
                type = "MULTIPLE_CHOICE",
                category = "Etika Digital",
                questionText = "Dalam penggunaan teknologi informasi, mahasiswa dilarang menyebarkan...",
                isTrueFalse = false,
                choices = new string[] { "Informasi akademik resmi", "Jadwal kuliah dari dosen", "Username dan password akun IPB", "Materi kuliah yang diberikan dosen" },
                correctAnswerIndex = 2,
                correctFeedback = "Benar! Username dan password akun IPB tidak boleh disebarkan karena berpotensi disalahgunakan.",
                incorrectFeedback = "Kurang tepat. Username dan password akun IPB tidak boleh disebarkan."
            },
            new QuizQuestion
            {
                id = "quiz_016",
                type = "TRUE_FALSE",
                category = "Toleransi dan Organisasi",
                questionText = "Mahasiswa boleh menunjukkan sikap tidak menerima perbedaan keyakinan, pendapat, budaya, atau cara hidup orang lain.",
                isTrueFalse = true,
                choices = new string[] { "Benar", "Salah" },
                correctAnswerIndex = 1,
                correctFeedback = "Benar! Mahasiswa harus menghargai perbedaan dan tidak boleh menunjukkan sikap intoleran.",
                incorrectFeedback = "Kurang tepat. Sikap tidak menerima perbedaan termasuk perilaku yang dilarang."
            },
            new QuizQuestion
            {
                id = "quiz_017",
                type = "MULTIPLE_CHOICE",
                category = "Klasifikasi Pelanggaran",
                questionText = "Jika pelanggaran ringan dilakukan lebih dari 3 kali, klasifikasinya dapat berubah menjadi...",
                isTrueFalse = false,
                choices = new string[] { "Tidak dihitung", "Pelanggaran sedang", "Pelanggaran berat langsung", "Peringatan informal saja" },
                correctAnswerIndex = 1,
                correctFeedback = "Benar! Pelanggaran ringan yang dilakukan lebih dari 3 kali dapat naik menjadi pelanggaran sedang.",
                incorrectFeedback = "Kurang tepat. Pelanggaran ringan yang berulang lebih dari 3 kali dapat menjadi pelanggaran sedang."
            },
            new QuizQuestion
            {
                id = "quiz_018",
                type = "MULTIPLE_CHOICE",
                category = "Sanksi",
                questionText = "Salah satu contoh sanksi administratif tingkat berat adalah...",
                isTrueFalse = false,
                choices = new string[] { "Teguran lisan", "Permohonan maaf tertulis", "Diberhentikan sebagai mahasiswa", "Peringatan dari teman sekelas" },
                correctAnswerIndex = 2,
                correctFeedback = "Benar! Salah satu sanksi tingkat berat adalah diberhentikan sebagai mahasiswa.",
                incorrectFeedback = "Kurang tepat. Contoh sanksi tingkat berat adalah diberhentikan sebagai mahasiswa."
            }
        };

        private List<QuizQuestion> selectedQuestionsThisGame = new List<QuizQuestion>();
        private Queue<QuizQuestion> questionQueue = new Queue<QuizQuestion>();

        private void OnEnable()
        {
            Debug.Log($"Quiz bank loaded: {questions.Count} questions.");
        }

        public void SelectQuestionsForThisGame()
        {
            Debug.Log("Added/merged new quiz questions: 18.");

            if (questions == null || questions.Count == 0)
            {
                selectedQuestionsThisGame = new List<QuizQuestion>();
                questionQueue = new Queue<QuizQuestion>();
                return;
            }

            List<QuizQuestion> shuffled = new List<QuizQuestion>(questions);
            for (int i = shuffled.Count - 1; i > 0; i--)
            {
                int r = Random.Range(0, i + 1);
                QuizQuestion temp = shuffled[i];
                shuffled[i] = shuffled[r];
                shuffled[r] = temp;
            }

            int count = Mathf.Min(6, shuffled.Count);
            selectedQuestionsThisGame = shuffled.GetRange(0, count);
            questionQueue = new Queue<QuizQuestion>(selectedQuestionsThisGame);

            Debug.Log($"Selected {selectedQuestionsThisGame.Count} quiz questions for this game.");
        }

        public QuizQuestion GetNextQuestion()
        {
            if (questionQueue == null || questionQueue.Count == 0)
            {
                SelectQuestionsForThisGame();
            }

            if (questionQueue.Count == 0) return null;
            return questionQueue.Dequeue();
        }

        public QuizQuestion GetRandomQuestion()
        {
            if (questions == null || questions.Count == 0) return null;
            int r = Random.Range(0, questions.Count);
            return questions[r];
        }
    }
}

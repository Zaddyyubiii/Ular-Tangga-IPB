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
                questionText = "Menurut Peraturan Akademik, mahasiswa diperbolehkan memakai kaos oblong tak berkerah dan sandal jepit saat mengikuti UTS.",
                isTrueFalse = true,
                choices = new string[] { "Benar", "Salah" },
                correctAnswerIndex = 1, // "Salah" is index 1
                correctFeedback = "Benar. Saat kegiatan akademik resmi, mahasiswa harus berpakaian sopan dan sesuai ketentuan.",
                incorrectFeedback = "Kurang tepat. Pakaian saat kegiatan akademik resmi harus mengikuti aturan yang berlaku (berkerah dan bersepatu)."
            },
            new QuizQuestion
            {
                questionText = "Apa nama masa persiapan untuk mahasiswa baru pada tahun pertama di IPB?",
                isTrueFalse = false,
                choices = new string[] { "PPKU", "TPB" },
                correctAnswerIndex = 0, // "PPKU" is index 0
                correctFeedback = "Benar. PPKU (Program Pendidikan Kompetensi Umum) adalah masa persiapan awal mahasiswa IPB.",
                incorrectFeedback = "Kurang tepat. Jawaban yang benar adalah PPKU (Program Pendidikan Kompetensi Umum)."
            },
            new QuizQuestion
            {
                questionText = "Berapa batas waktu maksimal masa studi untuk program Sarjana (S1) reguler di IPB?",
                isTrueFalse = false,
                choices = new string[] { "8 Semester", "14 Semester" },
                correctAnswerIndex = 1, // "14 Semester" is index 1
                correctFeedback = "Benar. Batas maksimal masa studi S1 reguler adalah 14 semester.",
                incorrectFeedback = "Kurang tepat. Jawaban yang benar adalah 14 semester (7 tahun)."
            }
        };

        public QuizQuestion GetRandomQuestion()
        {
            if (questions == null || questions.Count == 0) return null;
            int r = Random.Range(0, questions.Count);
            return questions[r];
        }
    }
}

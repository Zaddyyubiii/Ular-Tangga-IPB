using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    [CreateAssetMenu(fileName = "MessageBank", menuName = "UlarTangga/MessageBank")]
    public class MessageBank : ScriptableObject
    {
        [TextArea(3, 10)]
        public string prologueText = "Di awal perjalanan kampus, kamu adalah mahasiswa yang masih sering mengabaikan tata tertib. Namun perjalanan di IPB University akan membentukmu menjadi pribadi yang lebih disiplin, bertanggung jawab, dan siap menjadi teladan. Lewati tantangan, jawab kuis, hindari pelanggaran, dan capai petak 100 untuk menjadi Duta Tata Tertib IPB University.";

        [Header("Normal Positive Messages (6 levels)")]
        public List<string> positiveMessages = new List<string>()
        {
            "Kamu memungut sampah di depan GWW dan membuangnya ke tempat sampah. Langkah kecil menuju kampus bersih!",
            "Kamu datang tepat waktu ke kelas pagi meskipun hujan. Disiplinmu mulai terbentuk!",
            "Kamu membantu teman memahami materi praktikum. Sikap kolaboratifmu patut diapresiasi!",
            "Kamu mengembalikan barang yang tertinggal di kantin. Kejujuran adalah kunci mahasiswa teladan!",
            "Kamu mengikuti kegiatan organisasi dengan bertanggung jawab. Soft skill-mu makin berkembang!",
            "Kamu menyelesaikan tugas tepat waktu dan menjaga etika akademik. Kamu semakin dekat menjadi mahasiswa teladan!"
        };

        [Header("Snake Violation Messages (3 severity levels)")]
        public List<string> snakeMessages = new List<string>()
        {
            "Pelanggaran Ringan! Anda parkir sembarangan. Turun 1 baris.",
            "Pelanggaran Sedang! Anda merusak fasilitas IPB. Turun 2 baris.",
            "Pelanggaran Berat! Anda membawa senjata tajam di lingkungan kampus. Turun 4 baris."
        };

        [Header("Skull Severe Violation Messages (3 levels)")]
        public List<string> skullMessages = new List<string>()
        {
            "Pelanggaran Berat! Anda melakukan pelanggaran serius terhadap tata tertib kampus. Kena skorsing & turun 4 baris!",
            "Pelanggaran Berat! Anda mengedarkan barang terlarang di lingkungan kampus. Kena skorsing & turun 4 baris!",
            "Pelanggaran Berat! Anda mengabaikan aturan keselamatan dan ketertiban kampus. Kena skorsing & turun 4 baris!"
        };

        [Header("Ladder Reward Messages (5 levels)")]
        public List<string> ladderMessages = new List<string>()
        {
            "Hebat! Kamu meraih Juara 1 Lomba Debat antar Departemen di tingkat Fakultas. Naik 1 baris.",
            "Proposal proyek sosialmu lolos pendanaan BEM Fakultas. Naik 1 baris.",
            "Karya tulis ilmiahmu memenangkan kompetisi tingkat nasional. Naik 2 baris.",
            "Kamu diterima magang karena konsisten membangun portofolio. Naik 2 baris.",
            "Kamu menjadi mahasiswa inspiratif dan dipercaya menjadi Duta IPB. Naik 3 baris."
        };

        public string GetPositiveMessage(int index)
        {
            if (positiveMessages == null || positiveMessages.Count == 0) return "Kamu belajar dengan rajin.";
            int i = Mathf.Clamp(index, 0, positiveMessages.Count - 1);
            return positiveMessages[i];
        }

        public string GetSnakeMessage(int severity)
        {
            if (snakeMessages == null || snakeMessages.Count == 0) return "Kamu melakukan pelanggaran.";
            int i = Mathf.Clamp(severity, 0, snakeMessages.Count - 1);
            return snakeMessages[i];
        }

        public string GetSkullMessage(int index)
        {
            if (skullMessages == null || skullMessages.Count == 0) return "Pelanggaran berat terdeteksi.";
            int i = Mathf.Clamp(index, 0, skullMessages.Count - 1);
            return skullMessages[i];
        }

        public string GetLadderMessage(int level)
        {
            if (ladderMessages == null || ladderMessages.Count == 0) return "Pencapaian akademik yang bagus!";
            int i = Mathf.Clamp(level, 0, ladderMessages.Count - 1);
            return ladderMessages[i];
        }
    }
}

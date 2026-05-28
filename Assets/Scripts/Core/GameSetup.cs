using System;

namespace UI
{
    public static class GameSetup
    {
        public static int HumanPlayerCount = 4;
        public static string[] PlayerNames = new string[]
        {
            "Mahasiswa 1",
            "Mahasiswa 2",
            "Mahasiswa 3",
            "Mahasiswa 4"
        };

        public static void Reset()
        {
            HumanPlayerCount = 4;
            PlayerNames = new string[]
            {
                "Mahasiswa 1",
                "Mahasiswa 2",
                "Mahasiswa 3",
                "Mahasiswa 4"
            };
        }
    }
}

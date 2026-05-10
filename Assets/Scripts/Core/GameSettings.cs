using UnityEngine;

namespace Assets.Scripts.Core
{
    public static class GameSettings
    {
        private const string KEY_VIBRATION = "Settings_Vibration";
        private const string KEY_SIZE = "Settings_Size";
        private const string KEY_MINES = "Settings_Mines";

        public static bool VibrationEnabled { get; set; } = true;
        public static int FieldSize { get; set; } = 5;
        public static int MineCount { get; set; } = 10;

        public static void Load()
        {
            VibrationEnabled = PlayerPrefs.GetInt(KEY_VIBRATION, 1) == 1;
            FieldSize = PlayerPrefs.GetInt(KEY_SIZE, 5);
            MineCount = PlayerPrefs.GetInt(KEY_MINES, 10);
        }

        public static void Save()
        {
            PlayerPrefs.SetInt(KEY_VIBRATION, VibrationEnabled ? 1 : 0);
            PlayerPrefs.SetInt(KEY_SIZE, FieldSize);
            PlayerPrefs.SetInt(KEY_MINES, MineCount);
            PlayerPrefs.Save();
        }
    }
}


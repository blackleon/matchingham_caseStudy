using _Project.Scripts.Runtime.Core.Events;
using UnityEngine;

namespace _Project.Scripts.Runtime.Data.Class
{
    public class PlayerData
    {
        public static int Level
        {
            get => PlayerPrefs.GetInt("Level", 0);
            set => PlayerPrefs.SetInt("Level", value);
        }

        private static int money;

        public static void SetMoney(int _money, Vector3 pos)
        {
            var change = _money - money;
            money = _money;
            PlayerPrefs.SetInt("Money", money);
            CoreEvents.MoneyChanged?.Invoke(change, pos);
        }

        public static int GetMoney()
        {
            return money;
        }

        public static bool Audio
        {
            get => PlayerPrefs.GetInt("Audio", 1) == 1;
            set => PlayerPrefs.SetInt("Audio", value ? 1 : 0);
        }

        public static bool Haptic
        {
            get => PlayerPrefs.GetInt("Haptic", 1) == 1;
            set => PlayerPrefs.SetInt("Haptic", value ? 1 : 0);
        }

        public static void Init()
        {
            money = PlayerPrefs.GetInt("Money", 0);
        }
    }
}
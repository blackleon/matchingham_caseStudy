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

        public static int Money
        {
            get => PlayerPrefs.GetInt("Money", 0);
            set
            {
                var oldMoney = Money;
                PlayerPrefs.SetInt("Money", value);

                CoreEvents.Moneychanged?.Invoke(Money - oldMoney);
            }
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
        }
    }
}
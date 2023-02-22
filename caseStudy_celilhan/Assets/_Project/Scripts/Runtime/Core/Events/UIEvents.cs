using _Project.Scripts.Runtime.Enums;
using UnityEngine.Events;

namespace _Project.Scripts.Runtime.Core.Events
{
    public class UIEvents
    {
        public static UnityAction<UIKey, bool> SetUI;
        public static UnityAction<int> UpdateTimer;
    }
}
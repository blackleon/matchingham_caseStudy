using _Project.Scripts.Runtime.Enums;
using UnityEngine.Events;

namespace _Project.Scripts.Runtime.Core.Events
{
    public class CoreEvents
    {
        public static UnityAction LoadScene;
        
        public static UnityAction<int> Moneychanged;

        public static UnityAction<MatchableKey, int> MatchablePlaced;
        public static UnityAction<MatchableKey, int, int> MatchableMoved;
        public static UnityAction<MatchableKey, int> MatchableRemoved;
    }
}
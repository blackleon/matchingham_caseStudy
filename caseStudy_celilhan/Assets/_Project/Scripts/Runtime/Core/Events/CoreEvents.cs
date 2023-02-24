using _Project.Scripts.Runtime.Core.Controller;
using UnityEngine;
using UnityEngine.Events;

namespace _Project.Scripts.Runtime.Core.Events
{
    public class CoreEvents
    {
        public static UnityAction LoadScene;

        public static UnityAction<int> Moneychanged;
        
        public static UnityAction<Collider> MatchClickInput;
        public static UnityAction<Collider> MatchableSelected;

        public static UnityAction<MatchableController, int> MatchablePlaced;
        public static UnityAction<int, int, bool> MatchableMoved;
        public static UnityAction<int, int> MatchableRemoved;
    }
}
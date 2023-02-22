using _Project.Scripts.Runtime.Enums;
using UnityEngine;

namespace _Project.Scripts.Runtime.Core.Controller
{
    public class MatchableController : MonoBehaviour
    {
        public MatchableKey Key;
        [SerializeField] private Rigidbody rig;
    }
}
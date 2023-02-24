using _Project.Scripts.Runtime.Core.Events;
using _Project.Scripts.Runtime.Data.Class;
using _Project.Scripts.Runtime.Utils.Mono;
using UnityEngine;

namespace _Project.Scripts.Runtime.Core.Controller
{
    public class InputController : MonoBehaviour
    {
        [SerializeField] private LayerMask matchableMask;
        private bool active;

        private void OnEnable()
        {
            Updater.OnUpdate += OnUpdate;
        }

        private void OnDisable()
        {
            Updater.OnUpdate -= OnUpdate;
        }

        private void OnUpdate()
        {
            if (!GameData.InputEnabled) return;

            if (Input.GetMouseButtonDown(0))
            {
                active = true;
            }

            if (!active) return;
            if (Input.GetMouseButton(0))
            {
                var ray = GameData.Cam.ScreenPointToRay(Input.mousePosition + (Vector3.forward));
                bool hitem = false;
                if (Physics.Raycast(ray, out var hit, float.MaxValue, matchableMask))
                {
                    hitem = true;
                    CoreEvents.MatchClickInput?.Invoke(hit.collider);
                }
                else
                {
                    CoreEvents.MatchClickInput?.Invoke(null);
                }

                Debug.DrawRay(ray.origin, ray.direction * 5f, hitem ? Color.red : Color.green);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                var ray = GameData.Cam.ScreenPointToRay(Input.mousePosition + (Vector3.forward));
                bool hitem = false;
                if (Physics.Raycast(ray, out var hit, float.MaxValue, matchableMask))
                {
                    hitem = true;
                    CoreEvents.MatchableSelected?.Invoke(hit.collider);
                }
                else
                {
                    CoreEvents.MatchClickInput?.Invoke(null);
                }

                Debug.DrawRay(ray.origin, ray.direction * 5f, hitem ? Color.white : Color.blue);

                active = false;
            }
        }
    }
}
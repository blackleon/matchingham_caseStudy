using _Project.Scripts.Runtime.Core.Class;
using _Project.Scripts.Runtime.Core.Events;
using _Project.Scripts.Runtime.Data.Class;
using _Project.Scripts.Runtime.Enums;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace _Project.Scripts.Runtime.Core.Controller
{
    public class MatchableController : MonoBehaviour
    {
        public MatchableKey Key;
        [SerializeField] private Rigidbody rig;
        [SerializeField] private Transform visual;
        [SerializeField] private Collider col;

        private int id;

        private void Awake()
        {
            id = gameObject.GetHashCode();
            Debug.Log("id: " + id);
        }

        private void OnEnable()
        {
            CoreEvents.MatchClickInput += OnMatchClickInput;
            CoreEvents.MatchableSelected += OnMatchableSelected;
        }

        private void OnDisable()
        {
            CoreEvents.MatchClickInput -= OnMatchClickInput;
            CoreEvents.MatchableSelected -= OnMatchableSelected;
        }

        private void OnMatchClickInput(Collider _col)
        {
            if (_col != col)
            {
                if (!DOTween.IsTweening("spinMatchable" + id)) return;

                DOTween.Kill("moveMatchable" + id);
                DOTween.Kill("spinMatchable" + id);
                visual.DOLocalMove(Vector3.zero, 0.1f).SetId("moveMatchableReset" + id);
                visual.DORotate(Vector3.zero, 0.1f, RotateMode.FastBeyond360).SetId("spinMatchableReset" + id);

                return;
            }

            if (DOTween.IsTweening("spinMatchable" + id)) return;

            DOTween.Kill("moveMatchableReset" + id);
            DOTween.Kill("spinMatchableReset" + id);
            var pos = transform.position - GameData.Cam.transform.forward * 2.5f;
            visual.DOMove(pos, 0.25f).SetId("moveMatchable" + id);
            visual.DORotate(Vector3.up * (Random.Range(0, 2) == 0 ? -360f : 360f), 5f, RotateMode.FastBeyond360)
                .SetRelative().SetEase(Ease.Linear).SetLoops(-1, LoopType.Incremental).SetId("spinMatchable" + id);
        }

        private async void OnMatchableSelected(Collider _col)
        {
            if (_col != col)
            {
                return;
            }

            rig.isKinematic = true;
            col.enabled = false;

            DOTween.Kill("moveMatchable" + id);
            DOTween.Kill("spinMatchable" + id);

            visual.DOLocalMove(Vector3.zero, 0.1f).SetId("moveMatchableReset" + id);
            visual.DORotate(Vector3.zero, 0.1f, RotateMode.FastBeyond360).SetId("spinMatchableReset" + id);

            await UniTask.Delay(System.TimeSpan.FromSeconds(0.1f));

            CoreEvents.MoveMatchableToSlot?.Invoke(transform);
            GameLogic.AddMatchableToSlot(this);
        }
    }
}
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
        [SerializeField] private Collider col;

        [SerializeField] private Transform visual;
        public int id;
        public bool matched;

        private void Awake()
        {
            id = gameObject.GetHashCode();
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

                rig.isKinematic = false;
                DOTween.Kill("moveMatchable" + id);
                DOTween.Kill("spinMatchable" + id);
                visual.DOLocalMove(Vector3.zero, 0.1f).SetId("moveMatchableReset" + id);
                visual.DORotate(Vector3.zero, 0.1f, RotateMode.FastBeyond360).SetId("spinMatchableReset" + id);

                return;
            }

            if (DOTween.IsTweening("spinMatchable" + id)) return;

            rig.isKinematic = true;
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

            visual.DOScale(0.5f, 0.1f).SetId("moveMatchableReset" + id);
            visual.DOLocalMove(Vector3.zero, 0.1f).SetId("moveMatchableReset" + id);
            visual.DORotate(Vector3.zero, 0.1f, RotateMode.FastBeyond360).SetId("spinMatchableReset" + id);

            await UniTask.Delay(System.TimeSpan.FromSeconds(0.1f));

            GameLogic.AddMatchableToSlot(this);
        }

        public void ReturnToPool()
        {
            rig.isKinematic = true;
            col.enabled = false;
            
            DOTween.Kill("moveMatchableReset" + id);
            DOTween.Kill("spinMatchableReset" + id);
            DOTween.Kill("moveMatchable" + id);
            DOTween.Kill("spinMatchable" + id);
            DOTween.Kill("moveSlot" + id);
            DOTween.Kill("matched" + id);
        }
        
        public void ResetMatchable()
        {
            matched = false;
            
            rig.isKinematic = false;
            col.enabled = true;

            visual.transform.localPosition = Vector3.zero;
            visual.transform.localEulerAngles = Vector3.zero;
            visual.transform.localScale = Vector3.one;
            transform.localScale = Vector3.one;
        }
    }
}
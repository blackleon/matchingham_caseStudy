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
        public Rigidbody rig;
        public Collider col;

        public Transform visual;
        [HideInInspector] public int id;
        [HideInInspector] public bool matched;
        [HideInInspector] public bool placed;

        [HideInInspector] public Transform[] gameObjects;

        private void Awake()
        {
            id = gameObject.GetHashCode();
            gameObjects = gameObject.GetComponentsInChildren<Transform>();
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
            if(placed) return;
            if (_col != col)
            {
                if (!DOTween.IsTweening("spinMatchable" + id)) return;

                DOTween.Kill("moveMatchable" + id);
                DOTween.Kill("spinMatchable" + id);
                visual.DOLocalMove(Vector3.zero, 0.1f).SetId("moveMatchableReset" + id);
                visual.DOLocalRotate(Vector3.zero, 0.1f, RotateMode.FastBeyond360).SetId("spinMatchableReset" + id)
                    .OnComplete(() => rig.isKinematic = false);
                visual.GetChild(0).DOLocalRotate(Vector3.zero, 0.1f, RotateMode.FastBeyond360).SetId("spinMatchableReset" + id);

                return;
            }

            if (DOTween.IsTweening("spinMatchable" + id)) return;

            rig.isKinematic = true;
            DOTween.Kill("moveMatchableReset" + id);
            DOTween.Kill("spinMatchableReset" + id);
            var pos = transform.position + (GameData.Cam.transform.position - transform.position).normalized * 2.5f;
            visual.DOMove(pos, 0.25f).SetId("moveMatchable" + id);
            visual.DORotate(
                Quaternion.LookRotation(GameData.Cam.transform.up + GameData.Cam.transform.forward,
                    GameData.Cam.transform.up - GameData.Cam.transform.forward).eulerAngles, 0.25f, RotateMode.FastBeyond360).SetId("spinMatchable" + id).OnComplete(
                () =>
                {
                    visual.GetChild(0).DOLocalRotate(Vector3.up * (Random.Range(0, 2) == 0 ? -360f : 360f), 5f, RotateMode.FastBeyond360)
                        .SetRelative().SetEase(Ease.Linear).SetLoops(-1, LoopType.Incremental).SetId("spinMatchable" + id);
                });
        }

        private async void OnMatchableSelected(Collider _col)
        {
            if(placed) return;
            if (_col != col)
            {
                rig.WakeUp();
                DOTween.Kill("moveMatchable" + id);
                DOTween.Kill("spinMatchable" + id);
                return;
            }

            rig.isKinematic = true;
            col.enabled = false;
            DOTween.Kill("moveMatchable" + id);
            DOTween.Kill("spinMatchable" + id);

            visual.DOScale(0.5f, 0.1f).SetId("moveMatchableReset" + id);
            visual.DOLocalMove(Vector3.zero, 0.1f).SetId("moveMatchableReset" + id);
            visual.GetChild(0).DOLocalRotate(Vector3.zero, 0.1f, RotateMode.FastBeyond360).SetId("spinMatchableReset" + id);

            placed = true;

            GameLogic.AddMatchableToSlot(this);
        }

        public void ReturnToPool()
        {
            DOTween.Kill("moveMatchableReset" + id);
            DOTween.Kill("spinMatchableReset" + id);
            DOTween.Kill("moveMatchable" + id);
            DOTween.Kill("spinMatchable" + id);
            DOTween.Kill("moveSlot" + id);
            DOTween.Kill("matched" + id);
        }

        public void ResetMatchable()
        {
            ReturnToPool();

            matched = false;
            placed = false;

            rig.isKinematic = false;
            col.enabled = true;

            foreach (var child in gameObjects)
                child.gameObject.layer = LayerMask.NameToLayer("Matchable");

            visual.localPosition = Vector3.zero;
            visual.localEulerAngles = Vector3.zero;
            visual.GetChild(0).localEulerAngles = Vector3.zero;
            visual.localScale = Vector3.one;
            transform.localScale = Vector3.one;
        }
    }
}
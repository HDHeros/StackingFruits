using DG.Tweening;
using GameStructConfigs;
using UnityEngine;

namespace Menu
{
    public class SectionView : MonoBehaviour
    {
        [SerializeField] private Vector3 _boundsSize = Vector3.one;
        [SerializeField] private Transform _model;
        [SerializeField] private Vector3 _onSelectOffset;
        [SerializeField] private Vector3 _onPickedOffset;
        public Vector3 Bounds => _boundsSize;
        public Transform Transform => transform;
        public Vector3 DefaultLocalPosition => _defaultLocalPos;

        private SectionConfig _config;
        private Vector3 _defaultLocalPos;


        public void Initialize(SectionConfig config, Vector3 localPos)
        {
            _config = config;
            _defaultLocalPos = localPos;
            Transform.localPosition = localPos;
        }

        public void Select(float duration)
        {
            _model.DOKill();
            _model.DOLocalMove(_onSelectOffset, duration).SetEase(Ease.OutQuint);
        }

        public void Deselect(float duration)
        {
            _model.DOKill();
            _model.DOLocalMove(Vector3.zero, duration).SetEase(Ease.OutQuint);
        }

        public void OnPicked(float duration)
        {
            _model.DOKill();
            _model.DOLocalMove(_onPickedOffset, duration).SetEase(Ease.OutBounce);
        }

        public void OnUnpicked(float duration)
        {
            _model.DOKill();
            Tween tween = _model.DOLocalMove(Vector3.zero, duration).SetEase(Ease.OutQuint);
            tween.OnKill(() =>
            {
                if (tween.IsActive() && tween.IsPlaying())
                    Transform.localPosition = _defaultLocalPos;
            });
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.position, _boundsSize);
        }
    }
}
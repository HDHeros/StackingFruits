using System;
using DG.Tweening;
using GameStructConfigs;
using HDH.GoPool;
using UnityEngine;

namespace Menu
{
    public class SectionView : MonoBehaviour
    {
        public event Action<LevelPreview> OnLevelPreviewClick;
        [SerializeField] private Bounds _localBounds;
        [SerializeField] private Transform _model;
        [SerializeField] private Vector3 _onSelectOffset;
        [SerializeField] private Vector3 _onPickedOffset;
        [SerializeField] private Bounds[] _availablePreviewBounds;
        private LevelPreview[] _previews;
        public Vector3 Bounds => _localBounds.size;
        public Transform Transform => transform;
        public Vector3 DefaultLocalPosition => _defaultLocalPos;

        private SectionConfig _config;
        private Vector3 _defaultLocalPos;


        public void Initialize(SectionConfig config, Vector3 localPos, IGoPool pool)
        {
            _config = config;
            _defaultLocalPos = localPos;
            Transform.localPosition = localPos;
            if (config.Levels.Length > _availablePreviewBounds.Length) 
                throw new Exception();
            _previews = new LevelPreview[config.Levels.Length];
            for (var i = 0; i < config.Levels.Length; i++)
            {
                _previews[i] = pool.Get(config.Levels[i].LevelPreview, _model);
                _previews[i].gameObject.SetActive(true);
                _previews[i].Initialize(config.Levels[i], _availablePreviewBounds[i]);
                _previews[i].OnClick += OnPreviewClick;
            }
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

        private void OnPreviewClick(LevelPreview preview) => 
            OnLevelPreviewClick?.Invoke(preview);

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.position + _localBounds.center, _localBounds.size);
            foreach (Bounds bound in _availablePreviewBounds)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireCube(transform.position + bound.center, bound.size);
            }
        }
    }
}
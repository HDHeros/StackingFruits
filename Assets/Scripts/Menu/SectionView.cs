using System;
using DG.Tweening;
using Gameplay.LevelsLogic;
using GameStructConfigs;
using HDH.GoPool;
using Infrastructure.SoundsLogic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Menu
{
    public class SectionView : MonoBehaviour, IPointerClickHandler
    {
        public event Action<LevelPreview> OnLevelPreviewClick;
        public event Action<SectionView> OnSectionClick;
        [SerializeField] private Transform _model;
        [SerializeField] private Vector3 _onSelectOffset;
        [SerializeField] private Vector3 _onPickedOffset;
        [SerializeField] private Bounds[] _availablePreviewBounds;
        [SerializeField] private BoxCollider _collider;
        public Vector3 Bounds => _collider.bounds.size;
        public Transform Transform => transform;
        public Vector3 DefaultLocalPosition => _defaultLocalPos;
        public SectionId Id { get; private set; }
        public int Index { get; private set; }
        private LevelPreview[] _previews;
        private Vector3 _defaultLocalPos;
        private SoundsService _sounds;
        private bool _isPicked;


        public void Initialize(LevelsService.SectionModel sectionModel, Vector3 localPos, IGoPool pool, int index,
            SoundsService sounds)
        {
            Id = sectionModel.Id;
            _defaultLocalPos = localPos;
            Transform.localPosition = localPos;
            Index = index;
            _sounds = sounds;
            if (sectionModel.Levels.Length > _availablePreviewBounds.Length) 
                throw new Exception();
            _previews = new LevelPreview[sectionModel.Levels.Length];
            for (var i = 0; i < sectionModel.Levels.Length; i++)
            {
                int inViewIndex = sectionModel.Levels[i].Config.InSectionViewIndex;
                _previews[i] = pool.Get(sectionModel.Levels[i].Config.LevelPreview, _model);
                _previews[i].gameObject.SetActive(true);
                _previews[i].Initialize(sectionModel.Levels[i], _availablePreviewBounds[inViewIndex], _sounds);
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
            if (_isPicked) return;
            _isPicked = true;
            _collider.enabled = false;
            _model.DOKill();
            _model.DOLocalMove(_onPickedOffset, duration).SetEase(Ease.OutBounce);
            _sounds.RaiseEvent(EventId.SectionPick);
            foreach (LevelPreview levelPreview in _previews) 
                levelPreview.EnableSelection();
        }

        public void OnUnpicked(float duration)
        {
            _isPicked = false;
            _collider.enabled = true;
            _model.DOKill();
            Tween tween = _model.DOLocalMove(Vector3.zero, duration).SetEase(Ease.OutQuint);
            tween.OnKill(() =>
            {
                if (tween.IsActive() && tween.IsPlaying())
                    Transform.localPosition = _defaultLocalPos;
            });
            foreach (LevelPreview levelPreview in _previews)
                levelPreview.DisableSelection();
        }

        public void OnPointerClick(PointerEventData eventData) => 
            OnSectionClick?.Invoke(this);

        private void OnPreviewClick(LevelPreview preview) => 
            OnLevelPreviewClick?.Invoke(preview);

        private void OnDrawGizmosSelected()
        {
            foreach (Bounds bound in _availablePreviewBounds)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireCube(transform.position + bound.center, bound.size);
            }
        }
    }
}
using System;
using DG.Tweening;
using Gameplay.LevelsLogic;
using HDH.GoPool;
using HDH.UnityExt.Extensions;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Menu
{
    public class SectionPicker : MonoBehaviour
    {
        public event Action<SectionView> OnSectionPicked;
        [SerializeField] private Transform _leftViewsParent;
        [SerializeField] private Transform _rightViewsParent;
        [SerializeField] private Vector2 _itemsSpacingRange;
        [SerializeField] private Vector2 _itemsRotationRange;
        [SerializeField] private Vector3 _hiddenPosition;
        private SectionView[] _views;
        private SectionView _selected;
        private Transform _transform;
        private int _selectedIndex;
        private Vector3 _selectedLocalPosCached;
        private bool _controlBlocked;
        private bool _isInteractionLocked;

        public void Initialize(LevelsService levels, IGoPool pool)
        {
            _views = new SectionView[levels.SectionsCount];
            _transform = transform;
            float width = 0;
            for (var i = 0; i < levels.SectionsCount; i++)
            {
                LevelsService.SectionModel section = levels.GetSectionByIndex(i);
                _views[i] = pool.Get(section.Config.ViewPrefab, _rightViewsParent);
                _views[i].OnSectionClick += OnAnySectionClick;
                SectionView view = _views[i];
                view.gameObject.SetActive(true);
                view.Initialize(section, Vector3.zero.WithX(width + view.Bounds.x * 0.5f), pool, i);
                view.Transform.rotation *= Quaternion.Euler(0, 0, Random.Range(_itemsRotationRange.x, _itemsRotationRange.y));
                width += view.Bounds.x + Random.Range(_itemsSpacingRange.x, _itemsSpacingRange.y);
            }
            
            SelectByIndex(0, false);
            Hide(false);
        }

        private void OnAnySectionClick(SectionView view)
        {
            if (view == _selected)
            {
                if (PickSelected(out var pickedSection) == false) return;
                OnSectionPicked?.Invoke(view);
                return;
            }
            SelectByIndex(view.Index, true);
        }

        public void SelectNext() => 
            SelectByIndex(_selectedIndex + 1, true);
        
        public void SelectPrev() =>
            SelectByIndex(_selectedIndex - 1, true);

        public void Show(bool animated)
        {
            if (CheckInteraction() == false) return;
            _transform.DOKill();
            if (animated)
            {
                _transform.DOMove(Vector3.zero, 0.5f).SetEase(Ease.OutBack).OnComplete(UnlockInteraction);
            }
            else
            {
                _transform.position = Vector3.zero;
                UnlockInteraction();
            }
        }

        public void Hide(bool animated)
        {
            if (CheckInteraction() == false) return;
            _transform.DOKill();
            if (animated)
                _transform.DOMove(_hiddenPosition + _selected.Transform.localPosition, 0.5f).SetEase(Ease.InBack).OnComplete(UnlockInteraction);
            else
            {
                _transform.position = _hiddenPosition + _selected.Transform.localPosition;
                UnlockInteraction();
            }
        }

        public bool PickSelected(out SectionView pickedSection)
        {
            pickedSection = null;
            if (CheckInteraction() == false) return false;
            _selectedLocalPosCached = _selected.DefaultLocalPosition;
            _selected.Transform.SetParent(_transform);
            _selected.OnPicked(0.25f);
            _leftViewsParent.DOKill();
            _rightViewsParent.DOKill();
            _leftViewsParent.DOMove(-_hiddenPosition - _selectedLocalPosCached, 0.5f);
            _rightViewsParent.DOMove(_hiddenPosition, 0.5f).OnComplete(UnlockInteraction);
            pickedSection = _selected;
            return true;
        }

        public bool UnpickSelected()
        {
            if (CheckInteraction() == false) return false;
            _selected.OnUnpicked(0.25f);
            _selected.Select(0.25f);
            _leftViewsParent.DOKill();
            _rightViewsParent.DOKill();
            _leftViewsParent.DOMove(-_selectedLocalPosCached, 0.5f);
            _rightViewsParent.DOMove(-_selectedLocalPosCached, 0.5f)
                .OnComplete(() =>
                {
                    _selected.Transform.SetParent(_rightViewsParent);
                    UnlockInteraction();
                });
            return true;
        }

        private bool CheckInteraction()
        {
            if (_isInteractionLocked) return false;
            _isInteractionLocked = true;
            return true;
        }

        private void UnlockInteraction() => 
            _isInteractionLocked = false;
        
        private void SelectByIndex(int index, bool animated)
        {
            if (index.InRangeInA(0, _views.Length) == false) return;
            if (_isInteractionLocked) return;
            
            if (_selected.IsNotNull())
                _selected.Deselect(0.5f);
            
            ChangeParents(index);
            
            _selectedIndex = index;
            _selected = _views[_selectedIndex];
            _leftViewsParent.DOKill();
            
            Vector3 scrollPosition = -_selected.Transform.localPosition;
            if (animated)
            {
                _leftViewsParent.DOMove(scrollPosition, 0.5f).SetEase(Ease.OutBack);
                _rightViewsParent.DOMove(scrollPosition, 0.5f).SetEase(Ease.OutBack).OnComplete(UnlockInteraction);
            }
            else
            {
                _leftViewsParent.position = scrollPosition;
                _rightViewsParent.position = scrollPosition;
                UnlockInteraction();
            }
            _selected.Select(0.5f);
        }

        private void ChangeParents(int index)
        {
            if (index > _selectedIndex)
            {
                for (int i = _selectedIndex; i < index; i++)
                {
                    _views[i].Transform.SetParent(_leftViewsParent);
                }
            }
            else
            {
                for (int i = _selectedIndex; i >= index; i--)
                {
                    _views[i].Transform.SetParent(_rightViewsParent);
                }
            }
        }
    }
}
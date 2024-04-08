using DG.Tweening;
using GameStructConfigs;
using HDH.GoPool;
using HDH.UnityExt.Extensions;
using UnityEngine;

namespace Menu
{
    public class SectionPicker : MonoBehaviour
    {
        [SerializeField] private Transform _leftViewsParent;
        [SerializeField] private Transform _rightViewsParent;
        [SerializeField] private Vector2 _itemsSpacingRange;
        [SerializeField] private Vector2 _itemsRotationRange;
        [SerializeField] private Vector3 _hiddenPosition;
        private SectionView[] _views;
        private SectionView _selected;
        private Transform _transform;
        private int _selectedIndex;

        public void Initialize(SectionConfig[] configs, IGoPool pool)
        {
            _views = new SectionView[configs.Length];
            _transform = transform;
            float width = 0;
            for (var i = 0; i < configs.Length; i++)
            {
                _views[i] = pool.Get(configs[i].ViewPrefab, _rightViewsParent);
                SectionView view = _views[i];
                view.gameObject.SetActive(true);
                view.Initialize(configs[i]);
                view.Transform.position = Vector3.zero.WithX(width + view.Bounds.x * 0.5f);
                view.Transform.rotation *= Quaternion.Euler(0, 0, Random.Range(_itemsRotationRange.x, _itemsRotationRange.y));
                width += view.Bounds.x + Random.Range(_itemsSpacingRange.x, _itemsSpacingRange.y);
            }
            
            SelectByIndex(0, false);
            Hide(false);
        }

        public void SelectNext() => 
            SelectByIndex(_selectedIndex + 1, true);
        
        public void SelectPrev() =>
            SelectByIndex(_selectedIndex - 1, true);

        public void Show(bool animated)
        {
            _transform.DOKill();
            if (animated)
                _transform.DOMove(Vector3.zero, 0.5f).SetEase(Ease.OutBack);
            else
                _transform.position = Vector3.zero;
        }

        public void Hide(bool animated)
        {
            _transform.DOKill();
            if (animated)
                _transform.DOMove(_hiddenPosition + _selected.Transform.localPosition, 0.5f).SetEase(Ease.InBack);
            else
                _transform.position = _hiddenPosition + _selected.Transform.localPosition;
        }

        private void SelectByIndex(int index, bool animated)
        {
            if (index.InRangeInA(0, _views.Length) == false) return;
            
            if (_selected.IsNotNull())
                _selected.Deselect(0.5f);
            
            ChangeParents(index);
            
            _selectedIndex = index;
            _selected = _views[_selectedIndex];
            _leftViewsParent.DOKill();
            if (animated)
            {
                _leftViewsParent.DOMove(-_selected.Transform.localPosition, 0.5f).SetEase(Ease.OutBack);
                _rightViewsParent.DOMove(-_selected.Transform.localPosition, 0.5f).SetEase(Ease.OutBack);
            }
            else
            {
                _leftViewsParent.position = -_selected.Transform.localPosition;
                _rightViewsParent.position = -_selected.Transform.localPosition;
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
                for (int i = _selectedIndex; i >= index; i--)
                {
                    _views[i].Transform.SetParent(_rightViewsParent);
                }
                //
                // for (int i = _selectedIndex; i != index; i += _selectedIndex < index ? 1 : -1)
                // {
                //     if (index > _selectedIndex)
                //         _views[i].Transform.SetParent(_leftViewsParent);
                //     else 
                //         _views[i].Transform.SetParent(_rightViewsParent);
                // }
        }
    }
}
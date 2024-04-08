using System;
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
        [SerializeField] private float _onPickedOffset;
        public Vector3 Bounds => _boundsSize;
        public Transform Transform => transform;
        private SectionConfig _config;


        public void Initialize(SectionConfig config)
        {
            _config = config;
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

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.position, _boundsSize);
        }
    }
}
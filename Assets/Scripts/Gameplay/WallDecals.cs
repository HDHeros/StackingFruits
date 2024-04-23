using System.Collections.Generic;
using HDH.GoPool;
using HDH.UnityExt.Extensions;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;

namespace Gameplay
{
    public class WallDecals : MonoBehaviour
    {
        private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");
        [SerializeField] private DecalProjector _projectorPrefab;
        [SerializeField] private Material[] _materialPool;
        private readonly List<DecalProjector> _spawnedProjectors = new(10);
        private Camera _camera;
        private IGoPool _pool;

        public void Initialize(Camera cam, IGoPool pool)
        {
            _camera = cam;
            _pool = pool;
        }
        
        public void SpawnDecal(Vector3 at, Color color)
        {
            DecalProjector projector = _pool.Get(_projectorPrefab);
            Material decalMaterial = _materialPool[Mathf.Clamp(_spawnedProjectors.Count, 0, _materialPool.Length - 1)];
            _spawnedProjectors.Add(projector);
            projector.gameObject.SetActive(true);
            projector.transform.position = at;
            Vector3 rotation = Quaternion.LookRotation(at - _camera.transform.position).eulerAngles;
            projector.transform.rotation = Quaternion.Euler(rotation.WithZ(Random.Range(0f, 360f)));
            float decalSize = Random.Range(2, 4);
            projector.size = new Vector3(decalSize, decalSize, projector.size.z);
            
            decalMaterial.SetColor(BaseColor, color);
            projector.material = decalMaterial;
            projector.fadeFactor = Random.Range(0.4f, 1f);
        }

        public void Clear()
        {
            foreach (DecalProjector projector in _spawnedProjectors) 
                _pool.Return(projector, _projectorPrefab);
            _spawnedProjectors.Clear();
        }
    }
}
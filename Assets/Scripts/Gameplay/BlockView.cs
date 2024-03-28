using DG.Tweening;
using UnityEngine;

namespace Gameplay
{
    public class BlockView : MonoBehaviour
    {
        [SerializeField] private MeshRenderer _renderer;
        
        public void Setup(int block)
        {
            Color color = Color.white;
            switch (block)
            {
                case 1: color = Color.green; break;
                case 2: color = Color.yellow; break;
                case 3: color = Color.red; break;
            }

            _renderer.material.color = color;
        }

        public void MoveTo(Vector2Int to)
        {
            transform.DOMove(new Vector3(to.x, to.y), 0.2f);
        }
    }
}
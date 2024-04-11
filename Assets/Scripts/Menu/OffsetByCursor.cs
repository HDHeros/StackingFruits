using UnityEngine;

namespace Menu
{
    public class OffsetByCursor : MonoBehaviour
    {
        [SerializeField] private Vector2 _horizontalOffsetRange;
        [SerializeField] private Vector2 _verticalOffsetRage;
        private Transform _transform;
        private Vector3 _initialLocalPos;

        private void Awake()
        {
            _transform = transform;
            _initialLocalPos = _transform.localPosition;
        }

        private void Update()
        {
            Vector2 cursorNormPos = new Vector2(Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height);
            _transform.localPosition = _initialLocalPos + new Vector3(
                Mathf.Lerp(_horizontalOffsetRange.x, _horizontalOffsetRange.y, cursorNormPos.x),
                Mathf.Lerp(_verticalOffsetRage.x, _verticalOffsetRage.y, cursorNormPos.y));
        }
    }
}
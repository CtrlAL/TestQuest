using UnityEngine;

namespace Views
{
    public class UIFlyer : MonoBehaviour
    {
        [SerializeField] private RectTransform _from;        // откуда летит (например, кнопка)
        [SerializeField] private RectTransform _to;          // куда летит (иконка Coin)
        [SerializeField] private GameObject _flyItemPrefab;  // префаб, который летит (например, +1)
        [SerializeField] private Canvas _canvas;

        private RectTransform _canvasRect;

        private void Awake()
        {
            _canvasRect = _canvas.GetComponent<RectTransform>();
        }

        public void Fly()
        {
            Vector2 fromPosition = GetCenterInCanvas(_from);
            Vector2 toPosition = GetCenterInCanvas(_to);

            GameObject item = Instantiate(_flyItemPrefab, _canvas.transform);
            RectTransform itemRect = item.GetComponent<RectTransform>();

            itemRect.anchoredPosition = fromPosition;

            // Анимация от A к B
            StartCoroutine(FlyRoutine(itemRect, fromPosition, toPosition));
        }

        private Vector2 GetCenterInCanvas(RectTransform rectTransform)
        {
            Vector2 screenPoint = rectTransform.TransformPoint(rectTransform.rect.center);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _canvasRect,
                screenPoint,
                null,
                out Vector2 localPoint
            );
            return localPoint;
        }

        private System.Collections.IEnumerator FlyRoutine(RectTransform item, Vector2 from, Vector2 to, float duration = 1f)
        {
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                t = Mathf.SmoothStep(0, 1, t); // плавное ускорение/замедление

                item.anchoredPosition = Vector2.Lerp(from, to, t);

                // Опционально: уменьшаем размер и альфу
                float scale = 1f - t;
                item.localScale = Vector3.one * scale;

                if (item.TryGetComponent<CanvasRenderer>(out var renderer))
                {
                    renderer.SetAlpha(1f - t);
                }

                yield return null;
            }

            Destroy(item.gameObject);
        }
    }
}

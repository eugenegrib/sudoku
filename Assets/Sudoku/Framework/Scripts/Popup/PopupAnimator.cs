using System.Collections;
using dotmob;
using UnityEngine;
using UnityEngine.UI;

namespace Sudoku.Framework.Scripts.Popup
{
    public class PopupAnimator : MonoBehaviour
    {
        public enum AnimType
        {
            Fade,
            Zoom
        }

        [Header("Anim Settings")]
        [SerializeField] private float animDuration;
        [SerializeField] private AnimType animType;
        [SerializeField] private AnimationCurve animCurve;

        private CanvasGroup canvasGroup;
        private RectTransform animContainer;

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            animContainer = GetComponent<RectTransform>();
        }

        public void ShowAnim(System.Action onComplete)
        {
            switch (animType)
            {
                case AnimType.Fade:
                    DoFadeAnim(onComplete);
                    break;
                case AnimType.Zoom:
                    DoZoomAnim(onComplete);
                    break;
            }
        }

        public void HideAnim(System.Action onComplete)
        {
            // Анимация скрытия
            UIAnimation anim = UIAnimation.Alpha(canvasGroup, 1f, 0f, animDuration);
            anim.style = UIAnimation.Style.EaseOut;
            anim.OnAnimationFinished += (GameObject target) =>
            {
                onComplete?.Invoke();
            };
            anim.Play();

            // Анимация для дочерних объектов
            AnimateChildrenAlpha(1f, 0f);
        }

        private void DoFadeAnim(System.Action onComplete)
        {
            // Анимация появления
            UIAnimation anim = UIAnimation.Alpha(canvasGroup, 0f, 1f, animDuration);
            anim.startOnFirstFrame = true;
            anim.OnAnimationFinished += (GameObject obj) =>
            {
                onComplete?.Invoke();
            };
            anim.Play();

            // Анимация для дочерних объектов
            AnimateChildrenAlpha(0f, 1f);
        }

        private void DoZoomAnim(System.Action onComplete)
        {
            // Анимация появления с масштабированием
            UIAnimation anim = UIAnimation.Alpha(canvasGroup, 0f, 1f, animDuration);
            anim.style = UIAnimation.Style.EaseOut;
            anim.startOnFirstFrame = true;
            anim.Play();

            UIAnimation scaleXAnim = UIAnimation.ScaleX(animContainer, 0f, 1f, animDuration);
            scaleXAnim.style = UIAnimation.Style.Custom;
            scaleXAnim.animationCurve = animCurve;
            scaleXAnim.startOnFirstFrame = true;
            scaleXAnim.Play();

            UIAnimation scaleYAnim = UIAnimation.ScaleY(animContainer, 0f, 1f, animDuration);
            scaleYAnim.style = UIAnimation.Style.Custom;
            scaleYAnim.animationCurve = animCurve;
            scaleYAnim.startOnFirstFrame = true;
            scaleYAnim.OnAnimationFinished += (GameObject obj) =>
            {
                onComplete?.Invoke();
            };
            scaleYAnim.Play();

            // Анимация для дочерних объектов
            AnimateChildrenAlpha(0f, 1f);
        }
        private void AnimateChildrenAlpha(float from, float to)
        {
            foreach (Transform child in animContainer)
            {
                // Анимация CanvasGroup
                CanvasGroup childCanvasGroup = child.GetComponent<CanvasGroup>();
                if (childCanvasGroup != null)
                {
                    UIAnimation childAnim = UIAnimation.Alpha(childCanvasGroup, from, to, animDuration);
                    childAnim.startOnFirstFrame = true;
                    childAnim.Play();
                }

                // Анимация общего альфа-канала
                Image childImage = child.GetComponent<Image>();
                if (childImage != null)
                {
                    // Создаем отдельный экземпляр материала для каждого объекта
                    Material newMaterial = new Material(childImage.material);
                    childImage.material = newMaterial;

                    // Запускаем корутину для анимации альфа-канала
                    StartCoroutine(AnimateAlpha(childImage, newMaterial, from, to));
                }
            }
        }

        private IEnumerator AnimateAlpha(Image image, Material material, float from, float to)
        {
            Debug.Log($"Animating alpha from {from} to {to}"); // Для отладки
            float elapsedTime = 0f;

            while (elapsedTime < animDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / animDuration);

                // Линейная интерполяция альфа-значения
                float currentAlpha = Mathf.Lerp(from, to, t);
                material.SetFloat("_Alpha", currentAlpha);

                // Принудительно обновляем изображение
                image.SetMaterialDirty();

                yield return null; // Ждем до следующего кадра
            }

            // Убедитесь, что альфа-значение установлено в целевое
            material.SetFloat("_Alpha", to);
            image.SetMaterialDirty();
        }

        

    }
}

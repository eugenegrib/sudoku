using UnityEngine;
using UnityEngine.UI;

namespace Sudoku.Scripts.Theme
{
    public class ThemeGraphicBehaviour : ThemeBehaviour
    {
        #region Member Variables

        private Image imageComponent;            // Компонент Image для замены изображения
        private RawImage rawImageComponent;      // Компонент RawImage для замены изображения
        private Text textComponent;               // Компонент Text для изменения цвета текста

        #endregion

        #region Unity Methods

        private void Awake()
        {
            imageComponent = gameObject.GetComponent<Image>();
            rawImageComponent = gameObject.GetComponent<RawImage>();
            textComponent = gameObject.GetComponent<Text>();

            if (imageComponent == null && rawImageComponent == null && textComponent == null)
            {
                Debug.LogError("[ThemeGraphicBehaviour] No Image, RawImage, or Text component found on this GameObject, gameObject.name: " + gameObject.name);
            }
        }

        #endregion

        #region Protected Methods

        protected override void SetColor(Color color)
        {
            // Устанавливаем цвет для Image
            if (imageComponent != null)
            {
                imageComponent.color = color;
            }

            // Устанавливаем цвет для RawImage
            if (rawImageComponent != null)
            {
                rawImageComponent.color = color;
            }

            // Устанавливаем цвет для Text
            if (textComponent != null)
            {
                textComponent.color = color;
            }
        }

        protected override void SetImage(Sprite sprite)
        {
            // Устанавливаем спрайт для компонента Image
            if (imageComponent != null)
            {
                imageComponent.sprite = sprite;
            }

            // Устанавливаем текстуру для компонента RawImage
            if (rawImageComponent != null)
            {
                rawImageComponent.texture = sprite.texture;
            }
        }

        #endregion
    }
}

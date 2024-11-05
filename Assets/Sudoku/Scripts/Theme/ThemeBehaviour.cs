using UnityEngine;

namespace Sudoku.Scripts.Theme
{
    public abstract class ThemeBehaviour : MonoBehaviour, IThemeBehaviour
    {
        #region Inspector Variables

        [SerializeField] private string colorId = ""; // id для цвета
        [SerializeField] private string spriteId = ""; // id для спрайта

        #endregion

        #region Abstract Methods

        protected abstract void SetColor(Color color); // Метод для смены цвета
        protected abstract void SetImage(Sprite sprite); // Метод для смены изображения

        #endregion

        #region Unity Methods

        private void Start()
        {
            if (ThemeManager.Exists() && ThemeManager.Instance.Enabled)
            {
                ThemeManager.Instance.Register(this);
                UpdateGraphics();
            }
        }

        private void UpdateGraphics()
        {
            // Устанавливаем цвет
            if (ThemeManager.Instance.GetItemColor(colorId, out Color color))
            {
                SetColor(color);
            }
            else
            {
                Debug.LogErrorFormat("[ThemeBehaviour] Could not find theme color id \"{0}\", gameObject: {1}", colorId, gameObject.name);
            }

            // Устанавливаем изображение
            if (ThemeManager.Instance.GetItemSprite(spriteId, out Sprite sprite))
            {
                SetImage(sprite);
            }
            else
            {
                Debug.LogErrorFormat("[ThemeBehaviour] Could not find theme sprite id \"{0}\", gameObject: {1}", spriteId, gameObject.name);
                // Здесь вы можете добавить любое другое поведение, например, установить стандартный спрайт
            }
        }

        #endregion

        #region Public Methods

        public void NotifyThemeChanged()
        {
            UpdateGraphics();
        }

        #endregion
    }
}
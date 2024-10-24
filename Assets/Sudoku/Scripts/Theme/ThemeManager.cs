using System.Collections.Generic;
using dotmob;
using UnityEngine;

namespace Sudoku.Scripts.Theme
{
    public class ThemeManager : SingletonComponent<ThemeManager>, ISaveable
    {
        #region Classes

        [System.Serializable]
        public class Theme
        {
            public string name = "";
            public bool defaultTheme = false;
            public List<Color> themeColors = null;
        }

        #endregion

        #region Inspector Variables
        [SerializeField] private List<string> itemIds = new List<string> { "numberButtonColor", /* другие идентификаторы */ };

        [SerializeField] private bool themesEnabled = false;
        [SerializeField] private List<Theme> themes = null;

        #endregion

        #region Member Variables

        private List<IThemeBehaviour> themeBehaviours;

        #endregion

        #region Properties

        public string SaveId { get { return "theme_manager"; } }

        public bool Enabled { get { return themesEnabled; } }
        public List<Theme> Themes { get { return themes; } }
        public int ActiveThemeIndex { get; private set; }
        public Theme ActiveTheme { get { return themes[ActiveThemeIndex]; } }

        public System.Action OnThemeChanged { get; set; }

        #endregion

        #region Unity Methods

        protected override void Awake()
        {
            base.Awake();

            SaveManager.Instance.Register(this);

            themeBehaviours = new List<IThemeBehaviour>();

            if (!LoadSave())
            {
                // Find the default theme and set it as the active theme
                for (int i = 0; i < themes.Count; i++)
                {
                    if (themes[i].defaultTheme)
                    {
                        ActiveThemeIndex = i;
                        break;
                    }
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Registers a theme behaviour to receive notifications when the theme changes
        /// </summary>
        public void Register(IThemeBehaviour themeBehaviour)
        {
            if (!themeBehaviours.Contains(themeBehaviour))
            {
                themeBehaviours.Add(themeBehaviour);
                // Установим сразу текущую тему для элемента при его добавлении
                themeBehaviour.NotifyThemeChanged();
            }
        }

        /// <summary>
        /// Unregister a theme behaviour
        /// </summary>
        public void Unregister(IThemeBehaviour themeBehaviour)
        {
            if (themeBehaviours.Contains(themeBehaviour))
            {
                themeBehaviours.Remove(themeBehaviour);
            }
        }

        /// <summary>
        /// Sets the given theme as the active theme
        /// </summary>
        public void SetActiveTheme(Theme theme)
        {
            SetActiveTheme(themes.IndexOf(theme));
        }

        /// <summary>
        /// Sets the active theme to the given index
        /// </summary>
        public void SetActiveTheme(int themeIndex)
        {
            if (ActiveThemeIndex == themeIndex)
            {
                return;
            }

            ActiveThemeIndex = themeIndex;

            // Обновляем все элементы, когда тема меняется
            for (int i = 0; i < themeBehaviours.Count; i++)
            {
                themeBehaviours[i].NotifyThemeChanged();
            }

            OnThemeChanged?.Invoke();
        }
        
        /// <summary>
        /// Toggles between the two available themes
        /// </summary>
        public void ToggleTheme()
        {
            // Предполагается, что у вас всего две темы: 0 и 1
            int newThemeIndex = (ActiveThemeIndex == 0) ? 1 : 0;
    
            SetActiveTheme(newThemeIndex);
        }

        

        /// <summary>
        /// Gets the theme item with the given id in the active theme
        /// </summary>
        public bool GetItemColor(string itemId, out Color color)
        {
            for (int i = 0; i < itemIds.Count; i++)
            {
                if (itemId == itemIds[i])
                {
                    color = ActiveTheme.themeColors[i];
                    return true;
                }
            }

            color = Color.white;
            return false;
        }

        #endregion

        #region Save Methods

        public Dictionary<string, object> Save()
        {
            Dictionary<string, object> json = new Dictionary<string, object>();

            json["active_theme_index"] = ActiveThemeIndex;

            return json;
        }

        public bool LoadSave()
        {
            JSONNode json = SaveManager.Instance.LoadSave(this);

            if (json == null)
            {
                return false;
            }

            ActiveThemeIndex = json["active_theme_index"].AsInt;

            return true;
        }

        #endregion
    }
}

using System.Collections.Generic;
using dotmob;
using Sudoku.Framework.Scripts.Save;
using UnityEngine;

namespace Sudoku.Scripts.Theme
{
    public class ThemeManager : SingletonComponent<ThemeManager>, ISaveable
    {
        #region Classes

        [System.Serializable]
        public class Theme
        {
            public string name;
            public bool defaultTheme;
            public List<Sprite> themeSprite; 
            public List<Color> themeColors;
        }

        #endregion

        #region Inspector Variables
        
        [SerializeField] private List<string> itemIds = new List<string>();
        [SerializeField] private List<string> spriteIds = new List<string>();
        [SerializeField] private List<Theme> themes = new List<Theme>();
        [SerializeField] private bool themesEnabled = false;

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
                // Найти и установить тему по умолчанию
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

        public void Register(IThemeBehaviour themeBehaviour)
        {
            if (!themeBehaviours.Contains(themeBehaviour))
            {
                themeBehaviours.Add(themeBehaviour);
                themeBehaviour.NotifyThemeChanged();
            }
        }

        public void Unregister(IThemeBehaviour themeBehaviour)
        {
            if (themeBehaviours.Contains(themeBehaviour))
            {
                themeBehaviours.Remove(themeBehaviour);
            }
        }

        public void SetActiveTheme(Theme theme)
        {
            SetActiveTheme(themes.IndexOf(theme));
        }

        public void SetActiveTheme(int themeIndex)
        {
            if (ActiveThemeIndex == themeIndex) return;

            ActiveThemeIndex = themeIndex;

            foreach (var behaviour in themeBehaviours)
            {
                behaviour.NotifyThemeChanged();
            }

            OnThemeChanged?.Invoke();
        }

        public void ToggleTheme()
        {
            int newThemeIndex = (ActiveThemeIndex == 0) ? 1 : 0;
            SetActiveTheme(newThemeIndex);
        }

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
        public Sprite defaultSprite; // Убедитесь, что этот спрайт задан в инспекторе или инициализирован

        // Новый метод для получения спрайта по идентификатору
        public bool GetItemSprite(string itemId, out Sprite sprite)
        {

            for (int i = 0; i < spriteIds.Count; i++)
            {
                if (itemId == spriteIds[i])
                {
                    sprite = ActiveTheme.themeSprite[i];
                    return true;
                }
            }

            sprite = defaultSprite;
            return false;
        }


        #endregion

        #region Save Methods

        public Dictionary<string, object> Save()
        {
            Dictionary<string, object> json = new Dictionary<string, object>
            {
                ["active_theme_index"] = ActiveThemeIndex
            };

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

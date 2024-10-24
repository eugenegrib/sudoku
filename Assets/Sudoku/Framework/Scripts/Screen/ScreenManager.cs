using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Gley.MobileAds;
using Gley.MobileAds.Scripts.ToUse;
using Sudoku.Framework.Scripts.Popup;
using Sudoku.Framework.Scripts.Sound;
using Sudoku.Scripts.Game;

namespace dotmob
{
    public class ScreenManager : SingletonComponent<ScreenManager>
    {
        #region Inspector Variables

        [Tooltip("SplashScreen приложения, который отображается при запуске.")]
        [SerializeField] private string splashScreenId = "splash"; // ID splash screen

        [Tooltip("Экран меню выбора игры, который отображается после splash screen.")]
        [SerializeField] private string gameMenuScreenId = "gameMenu"; // ID экрана меню выбора игры

        [Tooltip("Экран выбора игры, который отображается после меню выбора.")]
        [SerializeField] private string gameSelectScreenId = "gameSelect"; // ID экрана выбора игры

        [Tooltip("Главный экран приложения, то есть первый экран, который отображается после экрана выбора.")]
        [SerializeField] private string homeScreenId = "main"; // ID главного экрана

        [Tooltip("Список компонентов экранов, которые используются в игре.")]
        [SerializeField] private List<Screen> screens = null; // Список экранов

        #endregion

        #region Member Variables

        private List<string> backStack; // Стек для сохранения истории экранов (назад)
        private Screen currentScreen; // Экран, который сейчас отображается
        [SerializeField] private GameObject topbar; // Верхняя панель

        #endregion

        #region Properties

        public string HomeScreenId => homeScreenId; // Возвращает ID главного экрана
        public string CurrentScreenId => currentScreen == null ? "" : currentScreen.Id; // Возвращает ID текущего экрана

        #endregion

        #region Events

        public System.Action<string, string> OnSwitchingScreens; // Событие переключения экранов
        public System.Action<string> OnShowingScreen; // Событие показа экрана

        #endregion

        #region Unity Methods

        private void Start()
        {
            // Инициализируем стек экранов
            backStack = new List<string>();

            // Инициализируем и скрываем все экраны при запуске
            foreach (var screen in screens)
            {
                if (screen.gameObject.GetComponent<CanvasGroup>() == null)
                {
                    screen.gameObject.AddComponent<CanvasGroup>();
                }
                screen.Initialize();
                screen.gameObject.SetActive(true);
                screen.Hide(false, true);
            }

            // Показать SplashScreen при старте
            Show(splashScreenId, false, true);

            // Переход после сплэшки на экран меню
            StartCoroutine(ShowAdAndSwitchToGameMenu());
        }

        private void Update()
        {
            // Обработка нажатия кнопки "назад" на устройстве Android
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (!PopupManager.Instance.CloseActivePopup())
                {
                    if (CurrentScreenId == HomeScreenId)
                    {
                        Application.Quit();
                    }
                    else
                    {
                        Back();
                    }
                }
            }
        }

        #endregion

        #region Public Methods

        public void Show(string screenId)
        {
            if (CurrentScreenId == screenId) return;
            Show(screenId, false, false);
        }

        public void Back()
        {
            if (backStack.Count <= 0)
            {
                Debug.LogWarning("[ScreenController] В стеке назад нет экранов для возврата.");
                return;
            }

            string screenId = backStack[^1]; // Получаем ID последнего экрана
            backStack.RemoveAt(backStack.Count - 1); // Удаляем экран из стека
            Show(screenId, true, false); // Показать экран
        }

        public void Home()
        {
            if (CurrentScreenId == homeScreenId) return;
            Show(homeScreenId, true, false);
            ClearBackStack();
        }

        public void OnGameSelectContinue()
        {
         Show("main");
            // Переход на экран выбора игры после меню
            //Show(gameSelectScreenId, false, false);

            topbar.SetActive(true);
            
            GameManager.Instance.ContinueActiveGame();
            
        }

        public void OnGameSelectNewGame()
        {
            //topbar.SetActive(true);

            Show("gameSelect");

            //Show(gameSelectScreenId, false, false);

        }

        #endregion

        #region Private Methods

        private void Show(string screenId, bool back, bool immediate)
        {
            Screen screen = GetScreenById(screenId);
            if (screen == null)
            {
                Debug.LogError("[ScreenController] Не удалось найти экран с ID: " + screenId);
                return;
            }

            if (currentScreen != null)
            {
                currentScreen.Hide(back, immediate);
                if (!back) backStack.Add(currentScreen.Id);
                OnSwitchingScreens?.Invoke(currentScreen.Id, screenId);
            }

            screen.Show(back, immediate);
            currentScreen = screen;
            OnShowingScreen?.Invoke(screenId);
        }

        private void ClearBackStack()
        {
            backStack.Clear();
        }

        private Screen GetScreenById(string id)
        {
            foreach (var screen in screens)
            {
                if (id == screen.Id)
                {
                    return screen;
                }
            }

            Debug.LogError("[ScreenTransitionController] Экран с ID " + id + " не существует.");
            return null;
        }

        private IEnumerator ShowAdAndSwitchToGameMenu()
        {
            float adTimeout = 5.0f;  // Максимальное время ожидания показа рекламы (5 секунд)
            float adDelay = 2.0f;    // Минимальная задержка перед показом рекламы (2 секунды)
    
            float splashScreenDisplayTime = 2.0f; // Минимальное время показа сплэш-экрана (2 секунды)

            // Показать сплэш-экран
            yield return new WaitForSeconds(splashScreenDisplayTime); // Ждем, чтобы сплэш-экран отображался минимум 2 секунды

            // Переход к ожиданию рекламы
            yield return new WaitForSeconds(adDelay); // Дополнительная задержка перед показом рекламы

            bool adCompleted = false;

            // Попытка показать межстраничную рекламу
            if (API.IsInterstitialAvailable())  // Проверяем, доступна ли реклама
            {
                API.ShowInterstitial(() => adCompleted = true, () => adCompleted = true); // Коллбэк на завершение рекламы
            }

            // Ждем либо завершения показа рекламы, либо таймаута
            float elapsedTime = 0f;
            while (!adCompleted && elapsedTime < adTimeout)
            {
                yield return null;
                elapsedTime += Time.deltaTime;  // Считаем время
            }

            // Если реклама не была показана, переходим на экран выбора игры
            if (!adCompleted)
            {
                Debug.Log("Реклама не была показана, переходим на следующий экран.");
            }

            // Переход на экран меню выбора игры
            Show(gameMenuScreenId, false, false);
        }

        public void openMenuScreen()
        {
            Show(gameMenuScreenId, false, false);
        }
        
        public  void OpenHomeScreen()
        {
            Show(homeScreenId, false, false);

        }
        #endregion
    }
}

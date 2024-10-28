using System;
using System.Collections;
using System.Collections.Generic;
using dotmob;
using Sudoku.Framework.Scripts.Popup;
using Sudoku.Scripts.Ads;
using Sudoku.Scripts.Game;
using UnityEngine;

namespace Sudoku.Framework.Scripts.Screen
{
    public class ScreenManager : SingletonComponent<ScreenManager>
    {
        #region Inspector Variables

        [Tooltip("SplashScreen приложения, который отображается при запуске.")] [SerializeField]
        private string splashScreenId = "splash"; // ID splash screen

        [Tooltip("Экран меню выбора игры, который отображается после splash screen.")] [SerializeField]
        private string gameMenuScreenId = "gameMenu"; // ID экрана меню выбора игры

        [Tooltip("Экран выбора игры, который отображается после меню выбора.")] [SerializeField]
        private string gameSelectScreenId = "gameSelect"; // ID экрана выбора игры

        [Tooltip("Главный экран приложения, то есть первый экран, который отображается после экрана выбора.")]
        [SerializeField]
        private string homeScreenId = "main"; // ID главного экрана

        [Tooltip("Список компонентов экранов, которые используются в игре.")] [SerializeField]
        private List<Screen> screens = null; // Список экранов

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

        private void Awake()
        {
        }

        [Obsolete("Obsolete")]
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

            appOpenAdController = FindObjectOfType<AppOpenAdController>();
            appOpenAdController.LoadAppOpenAd();
            
            // Найдите компонент InterstitialAdController в сцене
            interstitialAdController = FindObjectOfType<InterstitialAdController>();

            // Загрузите межстраничную рекламу
            interstitialAdController.LoadAd();
            // Найдите компонент InterstitialAdController в сцене


            rewardedInterstitialAdController = FindObjectOfType<RewardedInterstitialAdController>();

            // Загрузите межстраничную рекламу
            rewardedInterstitialAdController.LoadAd();

            bannerController = FindObjectOfType<AdaptiveBannerSample>(); // Находим или создаем контроллер баннера
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
            topbar.SetActive(true);
            GameManager.Instance.ContinueActiveGame();
        }

        public void OnGameSelectNewGame()
        {
            Show("gameSelect");
            topbar.gameObject.SetActive(false);
        }

        #endregion

        #region Private Methods

        private AdaptiveBannerSample bannerController;
        private InterstitialAdController interstitialAdController;
        private RewardedInterstitialAdController rewardedInterstitialAdController;
        private AppOpenAdController appOpenAdController;

        private void Show(string screenId, bool back, bool immediate)
        {
            Screen screen = GetScreenById(screenId);
            if (screen == null) return;

            if (currentScreen != null)
            {
                currentScreen.Hide(back, immediate);
            }

            screen.Show(back, immediate);
            currentScreen = screen;

            if (bannerController != null)
            {
                if (screen.showBanner)
                {
                    Debug.Log("Показываем баннер для экрана: " + screen.Id);
                    bannerController.ShowBanner();
                }
                else
                {
                    Debug.Log("Скрываем баннер для экрана: " + screen.Id);
                    bannerController.HideBanner();
                }
            }
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
            float splashScreenDisplayTime = 2.0f; // Минимальное время показа сплэш-экрана (2 секунды)
            float adTimeout = 5.0f; // Таймаут для показа рекламы (5 секунд)

            // Показать сплэш-экран
            yield return
                new WaitForSeconds(splashScreenDisplayTime); // Ждем, чтобы сплэш-экран отображался минимум 2 секунды

            bool adShown = false;

            // Показ межстраничной рекламы
            if (interstitialAdController != null)
            {
                interstitialAdController.OnAdClosed += () =>
                {
                    adShown = true; // Установить флаг, если реклама была показана
                    Show(gameMenuScreenId, false, false); // Показать экран меню
                };

                interstitialAdController.ShowAd();

                // Ждем 5 секунд или пока реклама не будет показана
                float elapsedTime = 0f;
                while (!adShown && elapsedTime < adTimeout)
                {
                    yield return null; // Ждем один кадр
                    elapsedTime += Time.deltaTime; // Увеличиваем время
                }

                // Если реклама не была показана, переходим на экран меню
                if (!adShown)
                {
                    Debug.Log("Реклама не была показана, переходим на следующий экран.");
                    Show(gameMenuScreenId, false, false);
                }
            }
            else
            {
                // Переход на экран меню выбора игры, если реклама недоступна
                Show(gameMenuScreenId, false, false);
            }
        }

        public void OpenMenuScreen()
        {
            Show(gameMenuScreenId, false, false);
        }

        public void OpenHomeScreen()
        {
            Show(homeScreenId, false, false);
        }

        #endregion
    }
}
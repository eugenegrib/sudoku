using System;
using System.Collections;
using GoogleMobileAds.Api;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Sudoku.Scripts
{
    public class GoogleMobileAds : MonoBehaviour
    {
        private InterstitialAd _interstitialAd;
        private bool _adLoaded = false;
        [SerializeField] public string nextSceneName; // Имя следующей сцены для перехода
        private AsyncOperation _sceneLoadingOperation;

        public void Start()
        {
            // Предварительно загружаем следующую сцену в фоне
            _sceneLoadingOperation = SceneManager.LoadSceneAsync(nextSceneName);
            _sceneLoadingOperation.allowSceneActivation = false; // Не активируем её сразу

            // Initialize the Google Mobile Ads SDK.
            MobileAds.Initialize((InitializationStatus initStatus) =>
            {
                // This callback is called once the MobileAds SDK is initialized.
                LoadInterstitialAd();
            });

            // Запуск корутины, которая будет управлять показом рекламы
            StartCoroutine(ShowAdWithDelay());
        }

        private string _adUnitId;

#if UNITY_ANDROID
        private string adUnitId = "ca-app-pub-3940256099942544/1033173712";
#elif UNITY_IPHONE
        private string adUnitId = "ca-app-pub-3940256099942544/4411468910";
#else
        private string adUnitId = "unused";
#endif

        /// <summary>
        /// Loads the interstitial ad.
        /// </summary>
        public void LoadInterstitialAd()
        {
            // Clean up the old ad before loading a new one.
            if (_interstitialAd != null)
            {
                _interstitialAd.Destroy();
                _interstitialAd = null;
            }

            Debug.Log("Loading the interstitial ad.");

            // Create the ad request used to load the ad.
            var adRequest = new AdRequest.Builder().Build();

            // Send the request to load the ad.
            InterstitialAd.Load(adUnitId, adRequest,
                (InterstitialAd ad, LoadAdError error) =>
                {
                    if (error != null || ad == null)
                    {
                        Debug.LogError("Interstitial ad failed to load: " + error);
                        return;
                    }

                    Debug.Log("Interstitial ad loaded.");
                    _interstitialAd = ad;
                    _adLoaded = true; // Реклама загружена

                    // Важно: сразу после успешной загрузки регистрируем события
                    RegisterEventHandlers(ad);
                });
        }

        /// <summary>
        /// Coroutine for showing the interstitial ad with a delay.
        /// </summary>
        private IEnumerator ShowAdWithDelay()
        {
            // Ждем 2 секунды перед показом рекламы
            yield return new WaitForSeconds(2f);

            // Если реклама загрузилась, показываем её
            if (_adLoaded && _interstitialAd != null && _interstitialAd.CanShowAd())
            {
                Debug.Log("Showing interstitial ad.");
                _interstitialAd.Show();
            }
            else
            {
                Debug.Log("Ad not loaded yet. Waiting for 5 seconds.");
                // Ждем еще до 5 секунд, если реклама не была загружена
                float remainingTime = 5f;
                while (!_adLoaded && remainingTime > 0)
                {
                    yield return null; // Ждем следующий кадр
                    remainingTime -= Time.deltaTime;
                }

                // Если реклама загрузилась за время ожидания
                if (_adLoaded && _interstitialAd != null && _interstitialAd.CanShowAd())
                {
                    Debug.Log("Ad loaded in time. Showing interstitial ad.");
                    _interstitialAd.Show();
                }
                else
                {
                    Debug.Log("Ad failed to load in time. Moving to next scene.");
                    // Переходим на следующую сцену, если реклама не была загружена за 5 секунд
                    _sceneLoadingOperation.allowSceneActivation = true;
                }
            }
        }

        private void RegisterEventHandlers(InterstitialAd interstitialAd)
        {
            interstitialAd.OnAdPaid += (AdValue adValue) =>
            {
                Debug.Log(String.Format("Interstitial ad paid {0} {1}.", adValue.Value, adValue.CurrencyCode));
            };
            interstitialAd.OnAdImpressionRecorded += () =>
            {
                Debug.Log("Interstitial ad recorded an impression.");
            };
            interstitialAd.OnAdClicked += () =>
            {
                Debug.Log("Interstitial ad was clicked.");
            };
            interstitialAd.OnAdFullScreenContentOpened += () =>
            {
                Debug.Log("Interstitial ad full screen content opened.");
            };
            interstitialAd.OnAdFullScreenContentClosed += () =>
            {
                Debug.Log("Interstitial ad full screen content closed.");
                // Активируем заранее загруженную сцену после закрытия рекламы
                _sceneLoadingOperation.allowSceneActivation = true;
            };
            interstitialAd.OnAdFullScreenContentFailed += (AdError error) =>
            {
                Debug.LogError("Interstitial ad failed to open full screen content with error: " + error);
                // Активируем сцену, если реклама не смогла открыться
                _sceneLoadingOperation.allowSceneActivation = true;
            };
        }
    }
}

using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using UnityEngine;

namespace Sudoku.Scripts.Ads
{
    public class AppOpenAdController : MonoBehaviour
    {
        
#if UNITY_ANDROID
        private const string ADUnitId = "ca-app-pub-3940256099942544/9257395921";
#elif UNITY_IPHONE
   string _adUnitId = "ca-app-pub-3940256099942544/5575463023";
#else
  private string _adUnitId = "unused";
#endif

        private AppOpenAd appOpenAd;
        
        public void LoadAppOpenAd()
        {
            Debug.Log("AppOpen LoadAppOpenAd");

            // Clean up the old ad before loading a new one.
            if (appOpenAd != null)
            {
                appOpenAd.Destroy();
                appOpenAd = null;
            }

            // Create our request used to load the ad.
            var adRequest = new AdRequest();

            // Send the request to load the ad.
            AppOpenAd.Load(ADUnitId, adRequest, (AppOpenAd ad, LoadAdError error) =>
            {
                // If error is not null, the load request failed.
                if (error != null)
                {
                    Debug.LogError($"AppOpen failed to load an ad with error: {error.GetMessage()}");
                    return;
                }

                // Ensure ad is not null
                if (ad == null)
                {
                    Debug.LogError("AppOpen failed to load an ad: received null ad instance.");
                    return;
                }

                Debug.Log($"AppOpen ad loaded successfully with response: {ad.GetResponseInfo()}");

                appOpenAd = ad;
                RegisterEventHandlers(ad);
            });
        }

        private  void Awake()
        {
            Debug.Log("AppOpen Awake");

            // Use the AppStateEventNotifier to listen to application open/close events.
            // This is used to launch the loaded ad when we open the app.
            AppStateEventNotifier.AppStateChanged += OnAppStateChanged;
        }

        private void OnDestroy()
        {
            Debug.Log("AppOpen OnDestroy");

            // Always unlisten to events when complete.
            AppStateEventNotifier.AppStateChanged -= OnAppStateChanged;
        }
    
        private void OnAppStateChanged(AppState state)
        {
            Debug.Log("AppOpen OnAppStateChanged");

            // if the app is Foregrounded and the ad is available, show it.
            if (state != AppState.Foreground) return;
            if (IsAdAvailable)
            {
                ShowAppOpenAd();
            }
        }

        private bool IsAdAvailable
        {
            get
            {      
                Debug.Log("AppOpen IsAdAvailable");
                return appOpenAd != null;
            }
        }

        public void Start()
        {

            Debug.Log("AppOpen Start");
            // Initialize the Google Mobile Ads SDK.
            MobileAds.Initialize((InitializationStatus initStatus) =>
            {
                // This callback is called once the MobileAds SDK is initialized.
            });
        }


        /// <summary>
        /// Shows the app open ad.
        /// </summary>
        public void ShowAppOpenAd()
        {
            Debug.Log("AppOpen ShowAppOpenAd");

            if (appOpenAd != null && appOpenAd.CanShowAd())
            {
                
                Debug.Log("AppOpen Showed");
                appOpenAd.Show();
            }
            else
            {
                Debug.Log("AppOpen not Showed");
            }
        }
    
        private void RegisterEventHandlers(AppOpenAd ad)
        {
            Debug.Log("AppOpen RegisterEventHandlers");

            // Raised when the ad is estimated to have earned money.
            ad.OnAdPaid += (AdValue adValue) =>
            {
                Debug.Log(string.Format("AppOpen ad paid {0} {1}.",
                    adValue.Value,
                    adValue.CurrencyCode));
            };
            // Raised when an impression is recorded for an ad.
            ad.OnAdImpressionRecorded += () =>
            {
                Debug.Log("AppOpen ad recorded an impression.");
            };
            // Raised when a click is recorded for an ad.
            ad.OnAdClicked += () =>
            {
                Debug.Log("AppOpen ad was clicked.");
            };
            // Raised when an ad opened full screen content.
            ad.OnAdFullScreenContentOpened += () =>
            {
                Debug.Log("AppOpen ad full screen content opened.");
            };
            // Raised when the ad closed full screen content.
            ad.OnAdFullScreenContentClosed += () =>
            {
                Debug.Log("AppOpen ad full screen content closed.");

                // Reload the ad so that we can show another as soon as possible.
                LoadAppOpenAd();
            };
            // Raised when the ad failed to open full screen content.
            ad.OnAdFullScreenContentFailed += (AdError error) =>
            {
                Debug.LogError("AppOpen ad failed to open full screen content " +
                               "with error : " + error);

                // Reload the ad so that we can show another as soon as possible.
                LoadAppOpenAd();
            };
        }
    
    }
}
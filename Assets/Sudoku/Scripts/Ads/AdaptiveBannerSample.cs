using System.Collections.Generic;
using GoogleMobileAds.Api;
using UnityEngine;

namespace Sudoku.Scripts.Ads
{
    public class AdaptiveBannerSample : MonoBehaviour
    {
        private BannerView bannerView;

        public void ShowBanner()
        {
            if (bannerView == null)
            {
                Debug.Log("Banner: Requesting new banner...");
                RequestBanner();
            }
            else
            {
                Debug.Log("Banner: Showing existing banner...");
                bannerView.Show();
            }
        }

        public void HideBanner()
        {
            bannerView?.Hide();
            Debug.Log("Banner: Hiding banner...");
        }

        // Use this for initialization
        private void Start()
        {
            // Set your test devices.
            // https://developers.google.com/admob/unity/test-ads
            RequestConfiguration requestConfiguration = new RequestConfiguration
            {
                TestDeviceIds = new List<string>
                {
                    AdRequest.TestDeviceSimulator,
                    // Add your test device IDs (replace with your own device IDs).
#if UNITY_IPHONE
                    "96e23e80653bb28980d3f40beb58915c"
#elif UNITY_ANDROID
                    "75EF8D155528C04DACBBA6F36F433035"
#endif
                }
            };
            MobileAds.SetRequestConfiguration(requestConfiguration);
            Debug.Log("Banner: Request configuration set with test devices.");
        }

        public void RequestBanner()
        {
            // These ad units are configured to always serve test ads.
#if UNITY_ANDROID
            const string adUnitId = "ca-app-pub-3940256099942544/6300978111";
#elif UNITY_IPHONE
            const string adUnitId = "ca-app-pub-3940256099942544/2934735716";
#else
            const string adUnitId = "unused";
#endif

            // Clean up banner ad before creating a new one.
            if (bannerView != null)
            {
                bannerView.Destroy();
                Debug.Log("Banner: Previous banner ad destroyed.");
            }

            var adaptiveSize =
                AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);

            bannerView = new BannerView(adUnitId, adaptiveSize, AdPosition.Bottom);

            // Register for ad events.
            bannerView.OnBannerAdLoaded += OnBannerAdLoaded;
            bannerView.OnBannerAdLoadFailed += OnBannerAdLoadFailed;

            var adRequest = new AdRequest();

            // Load a banner ad.
            bannerView.LoadAd(adRequest);
            Debug.Log(string.Format("Banner: Loading ad with unit ID: {0}", adUnitId));
        }

        #region Banner callback handlers

        private void OnBannerAdLoaded()
        {
            Debug.Log($"Banner: Banner view loaded an ad with response: {bannerView.GetResponseInfo()}");
        }

        private void OnBannerAdLoadFailed(LoadAdError error)
        {
            Debug.LogError(string.Format("Banner: Banner view failed to load an ad with error: {0}",
                error));
        }

        #endregion
    }
}

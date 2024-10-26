using System;
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
                Debug.Log("Requesting new banner...");
                RequestBanner();
            }
            else
            {
                Debug.Log("Showing existing banner...");
                bannerView.Show();
            }
        }

        public void HideBanner()
        {
            bannerView?.Hide();
        }
        // Use this for initialization
        void Start()
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

            // Initialize the Google Mobile Ads SDK.
            MobileAds.Initialize((InitializationStatus status) => 
            {
                RequestBanner();
            });
        }

        /*public void OnGUI()
        {
            GUI.skin.label.fontSize = 60;
            Rect textOutputRect = new Rect(
                0.15f * Screen.width,
                0.25f * Screen.height,
                0.7f * Screen.width,
                0.3f * Screen.height);
            GUI.Label(textOutputRect, "Adaptive Banner Example");
        }*/

        private void RequestBanner()
        {
            // These ad units are configured to always serve test ads.
#if UNITY_EDITOR
            string adUnitId = "unused";
#elif UNITY_ANDROID
            string adUnitId = "ca-app-pub-3212738706492790/6113697308";
#elif UNITY_IPHONE
            string adUnitId = "ca-app-pub-3212738706492790/5381898163";
#else
            string adUnitId = "unexpected_platform";
#endif

            // Clean up banner ad before creating a new one.
            if (bannerView != null)
            {
                bannerView.Destroy();
            }

            AdSize adaptiveSize =
                AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);

            bannerView = new BannerView(adUnitId, adaptiveSize, AdPosition.Bottom);

            // Register for ad events.
            bannerView.OnBannerAdLoaded += OnBannerAdLoaded;
            bannerView.OnBannerAdLoadFailed += OnBannerAdLoadFailed;

            AdRequest adRequest = new AdRequest();

            // Load a banner ad.
            bannerView.LoadAd(adRequest);
        }

        #region Banner callback handlers

        private void OnBannerAdLoaded()
        {
            Debug.Log("Banner view loaded an ad with response : "
                      + bannerView.GetResponseInfo());
          
        }

        private void OnBannerAdLoadFailed(LoadAdError error)
        {
            Debug.LogError("Banner view failed to load an ad with error : "
                           + error);
        }

        #endregion
    }
}
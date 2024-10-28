using System.Collections;
using System.Collections.Generic;
using GoogleMobileAds.Api;
using UnityEngine;
using UnityEngine.UI;

namespace Sudoku.Scripts.Ads
{
    [AddComponentMenu("GoogleMobileAds/Samples/RewardedInterstitialAdController")]
    public class RewardedInterstitialAdController : MonoBehaviour
    {
        public GameObject AdLoadedStatus;
        
        public List<Image> AdStatusImages;
        public List<Text> AdStatusText;
        public List<Button> AdStatusButtons;

        private const float fadeDuration = 0.3f;

#if UNITY_ANDROID
        private const string _adUnitId = "ca-app-pub-3940256099942544/5354046379";
#elif UNITY_IPHONE
        private const string _adUnitId = "ca-app-pub-3940256099942544/6978759866";
#else
        private const string _adUnitId = "unused";
#endif

        private RewardedInterstitialAd rewardedInterstitialAd;

        public void LoadAd()
        {
            if (rewardedInterstitialAd != null)
            {
                DestroyAd();
            }

            Debug.Log("Loading rewarded interstitial ad.");

            var adRequest = new AdRequest();
            RewardedInterstitialAd.Load(_adUnitId, adRequest,
                (RewardedInterstitialAd ad, LoadAdError error) =>
                {
                    if (error != null)
                    {
                        Debug.LogError("Rewarded interstitial ad failed to load an ad with error: " + error);
                        SetAdStatus(false);
                        return;
                    }

                    if (ad == null)
                    {
                        Debug.LogError("Unexpected error: Rewarded interstitial load event fired with null ad.");
                        SetAdStatus(false);
                        return;
                    }

                    Debug.Log("Rewarded interstitial ad loaded with response: " + ad.GetResponseInfo());
                    rewardedInterstitialAd = ad;
                    RegisterEventHandlers(ad);

                    SetAdStatus(true);
                });
        }

        public void ShowAd()
        {
            if (rewardedInterstitialAd != null && rewardedInterstitialAd.CanShowAd())
            {
                rewardedInterstitialAd.Show((Reward reward) =>
                {
                    Debug.Log("Rewarded interstitial ad rewarded: " + reward.Amount);
                });
            }
            else
            {
                Debug.LogError("Rewarded interstitial ad is not ready yet.");
            }

            SetAdStatus(false);
        }

        public void DestroyAd()
        {
            if (rewardedInterstitialAd != null)
            {
                Debug.Log("Destroying rewarded interstitial ad.");
                rewardedInterstitialAd.Destroy();
                rewardedInterstitialAd = null;
            }

            SetAdStatus(false);
        }

        private void SetAdStatus(bool isLoaded)
        {
            if (AdLoadedStatus != null)
            {
                AdLoadedStatus.SetActive(isLoaded);
            }

            var targetAlpha = isLoaded ? 1f : 0.5f;
            foreach (var image in AdStatusImages)
            {
                if (image == null) continue;
                StartCoroutine(FadeImage(image, targetAlpha));
            }

            foreach (var text in AdStatusText)
            {
                if (text == null) continue;
                StartCoroutine(FadeText(text, targetAlpha));
            }

            foreach (var button in AdStatusButtons)
            {
                if (button == null) continue;
                button.interactable = isLoaded;
            }
        }

        private IEnumerator FadeImage(Image image, float targetAlpha)
        {
            float startAlpha = image.color.a;
            float elapsedTime = 0f;
            Color color = image.color;

            while (elapsedTime < fadeDuration)
            {
                color.a = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration);
                image.color = color;
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            color.a = targetAlpha;
            image.color = color;
        }

        private IEnumerator FadeText(Text text, float targetAlpha)
        {
            float startAlpha = text.color.a;
            float elapsedTime = 0f;
            Color color = text.color;

            while (elapsedTime < fadeDuration)
            {
                color.a = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration);
                text.color = color;
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            color.a = targetAlpha;
            text.color = color;
        }

        private void RegisterEventHandlers(RewardedInterstitialAd ad)
        {
            ad.OnAdPaid += (AdValue adValue) =>
            {
                Debug.Log($"Rewarded interstitial ad paid {adValue.Value} {adValue.CurrencyCode}.");
            };

            ad.OnAdImpressionRecorded += () =>
            {
                Debug.Log("Rewarded interstitial ad recorded an impression.");
            };

            ad.OnAdClicked += () =>
            {
                Debug.Log("Rewarded interstitial ad was clicked.");
            };

            ad.OnAdFullScreenContentOpened += () =>
            {
                Debug.Log("Rewarded interstitial ad full screen content opened.");
            };

            ad.OnAdFullScreenContentClosed += () =>
            {
                Debug.Log("Rewarded interstitial ad full screen content closed.");
                LoadAd();
            };

            ad.OnAdFullScreenContentFailed += (AdError error) =>
            {
                Debug.LogError("Rewarded interstitial ad failed to open full screen content with error: " + error);
            };
        }
    }
}

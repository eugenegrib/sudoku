
using UnityEngine.UI;

using Sudoku.Scripts.Ads;

namespace Sudoku.Scripts
{
    using GoogleMobileAds.Api;
    using GoogleMobileAds.Common;
    using UnityEngine;

    public class MainScene : MonoBehaviour
    {
        AppOpenAdController appOpenAdController;
        public void Start()
        {      

            // TODO: Request an app open ad.
            MobileAds.Initialize((initStatus) =>
            {
                AppStateEventNotifier.AppStateChanged += OnAppStateChanged;
            });
        }

        public void OnAppStateChanged(AppState state)
        {
            if (state == AppState.Foreground)
            {
                appOpenAdController.ShowAppOpenAd();
            }
        }
    }
}
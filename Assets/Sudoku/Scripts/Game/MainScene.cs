using System;
using dotmob;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using Sudoku.Scripts.Ads;
using UnityEngine;

namespace Sudoku.Scripts.Game
{
    public class MainScene : SingletonComponent<MainScene>
    {
        [SerializeField] public AdaptiveBannerSample adaptiveBannerSample;
        [SerializeField] public AppOpenAdController appOpenAdController;

        public void Start()
        {
            MobileAds.Initialize((initStatus) => { appOpenAdController.LoadAppOpenAd(); });
        }
    }
}
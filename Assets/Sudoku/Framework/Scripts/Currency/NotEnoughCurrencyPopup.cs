using System;
using Sudoku.Scripts.Ads;
using UnityEngine;
using UnityEngine.Localization;

namespace Sudoku.Framework.Scripts.Currency
{
    public class NotEnoughCurrencyPopup : Popup.Popup
    {
        #region Inspector Variables

        [Space] [SerializeField] private LocalizedString titleText = null;
        [SerializeField] private LocalizedString messageText = null;
        [SerializeField] private LocalizedString rewardAdButtonText = null;
        [SerializeField] private GameObject rewardAdButton = null;
        [SerializeField] private GameObject storeButton = null;
        [SerializeField] private GameObject buttonsContainer = null;

        #endregion

        #region Member Variables

        private string currencyId;
        private int rewardAmount;

        #endregion

        #region Public Methods

        public override void OnShowing(object[] inData)
        {
            base.OnShowing(inData);

            currencyId = inData[0] as string;

            titleText = inData[1] as LocalizedString;
            messageText = inData[2] as LocalizedString;

            bool showStoreButton = (bool)inData[3];
            //bool showRewardAdButton	= (bool)inData[4] && MobileAdsManager.Instance.AreRewardAdsEnabled;

            rewardAdButtonText = inData[5] as LocalizedString;

            rewardAmount = (int)inData[6];

            storeButton.SetActive(showStoreButton);
        }

        private RewardedInterstitialAdController rewardedInterstitialAdController;

        [Obsolete("Obsolete")]
        public void OnRewardAdButtonClick()
        {
            rewardedInterstitialAdController = FindObjectOfType<RewardedInterstitialAdController>();

            rewardedInterstitialAdController.ShowAd();

            Hide(false);

            CurrencyManager.Instance.Give(currencyId, rewardAmount);
        }


        #endregion
        
    }
}
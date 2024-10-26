using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace dotmob
{
	public class NotEnoughCurrencyPopup : Popup
	{
		#region Inspector Variables

		[Space]

		[SerializeField] private Text titleText			= null;
		[SerializeField] private Text messageText			= null;
		[SerializeField] private Text rewardAdButtonText	= null;
		[SerializeField] private GameObject rewardAdButton		= null;
		[SerializeField] private GameObject storeButton			= null;
		[SerializeField] private GameObject buttonsContainer	= null;

		#endregion

		#region Member Variables

		private string	currencyId;
		private int		rewardAmount;

		#endregion

		#region Public Methods

		public override void OnShowing(object[] inData)
		{
			base.OnShowing(inData);

			currencyId = inData[0] as string;

			titleText.text		= inData[1] as string;
			messageText.text	= inData[2] as string;

			bool showStoreButton	= (bool)inData[3];
			//bool showRewardAdButton	= (bool)inData[4] && MobileAdsManager.Instance.AreRewardAdsEnabled;

			rewardAdButtonText.text = inData[5] as string;

			rewardAmount = (int)inData[6];

			storeButton.SetActive(showStoreButton);
	
		}

		public void OnRewardAdButtonClick()
		{

            /*
            if (!API.IsRewardedVideoAvailable())
            {
				rewardAdButton.SetActive(false);
				Debug.LogError("[NotEnoughCurrencyPopup] The reward button was clicked but there is no ad loaded to show.");
				return;
			}
			API.ShowRewardedVideo(CompleteMethod);
			*/
			

			Hide(false);
		}

		private void CompleteMethod(bool completed)
		{

			if (completed == true)
			{
				//give the reward
				CurrencyManager.Instance.Give(currencyId, rewardAmount);
			}
			else
			{
				//no reward }
			}
		}
				#endregion

				#region Private Methods

				private void OnRewardAdLoaded()
		{
			rewardAdButton.SetActive(true);
		}

		private void OnRewardAdClosed()
		{
			rewardAdButton.SetActive(false);
		}

		private void OnRewardAdGranted(string rewardId, double amount)
		{
			// Give the currency to the player
			CurrencyManager.Instance.Give(currencyId, rewardAmount);
		}

		private void OnAdsRemoved()
		{
			//MobileAdsManager.Instance.OnRewardAdLoaded -= OnRewardAdLoaded;
			rewardAdButton.SetActive(false);
		}

		#endregion
	}
}

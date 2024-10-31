using System;
using dotmob;
using Sudoku.Framework.Scripts.Popup;
using Sudoku.Scripts.Ads;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Sudoku.Scripts.Game
{
    public class GameOverPopup : Popup
    {
        [FormerlySerializedAs("ContinueButton")]
        public Button continueButton;

        Button button;
        public event Action OnAdClosed;

        private void HandleAdClosed()
        {
            Debug.Log("Interstitial ad closed.");
            OnAdClosed?.Invoke(); // Вызываем колбэк
        }

        private void Start()
        {
            button = continueButton.GetComponent<Button>();
            button.onClick.AddListener(TaskOnClick);
         
        }
        [SerializeField]  private InterstitialAdController interstitialAdController;

        private void TaskOnClick()
        {
            Hide(true);

            interstitialAdController.ShowAd(() => {
            });
        }
    }
}
using dotmob;
using Gley.MobileAds;
using Gley.MobileAds.Scripts.ToUse;
using UnityEngine.UI;

namespace Sudoku.Scripts.Game
{
    public class GameOverPopup : Popup
    {

        public Button ContinueButton;
        Button button;
        

        private void Start()
        {
            button = ContinueButton.GetComponent<Button>();
            button.onClick.AddListener(TaskOnClick);
        }

        private void TaskOnClick()
        {
            // Debug.Log("XEM quang cao de tiep tuc");
            if (API.IsRewardedVideoAvailable())
            {
                button.gameObject.SetActive(true);
                API.ShowRewardedVideo(CompleteMethod);
            }
            else
            {
                button.gameObject.SetActive(false);
            }
        }

        private void CompleteMethod(bool completed)
        {
            if (completed == true)
            {
                Hide(true);
            }
            else
            {
                Hide(false);
            }
        }
    }
}

using dotmob;
using UnityEngine.UI;

namespace Sudoku.Scripts.Game
{
    public class PausePopup : Popup
    {
        public Button ContinueButton;
        public Button NewGameButton;

        Button continueButton;
        Button newGameButton;

        private void Start()
        {
            GameManager.Instance.PauseGame(); // Останавливаем игру при открытии

            continueButton = ContinueButton.GetComponent<Button>();
            continueButton.onClick.AddListener(continueClicked);

            newGameButton = NewGameButton.GetComponent<Button>();
            newGameButton.onClick.AddListener(newGameClicked);
        }


        public override void Hide(bool result)
        {
            base.Hide(result);
            /*
            GameManager.Instance.ResumeGame(); // Возобновляем игру при закрытии
        */
        }

        public void newGameClicked()
        {
            
            base.Hide(true);
            GameManager.Instance.RestartGame();   
            /*// Показываем рекламу для продолжения
            if (API.IsRewardedVideoAvailable())
            {
                newGameButton.gameObject.SetActive(true);
            }
            else
            {
                newGameButton.gameObject.SetActive(false);
            }*/
        }

        public void continueClicked()
        {
            base.Hide(true);
            GameManager.Instance.ResumeGame();         }

        private void CompleteMethod(bool completed)
        {
            if (completed)
            {
                Hide(true); // Закрываем popup и продолжаем игру
            }
            else
            {
                Hide(false);
            }
        }
    }
}
using dotmob;
using Sudoku.Framework.Scripts.Popup;
using UnityEngine.UI;

namespace Sudoku.Scripts.Game
{
    public class PausePopup : Popup
    {
        private void Start()
        {
            GameManager.Instance.PauseGame(); // Останавливаем игру при открытии

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
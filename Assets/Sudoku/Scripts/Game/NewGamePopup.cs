using dotmob;
using Sudoku.Framework.Scripts.Popup;
using Sudoku.Framework.Scripts.Screen;
using UnityEngine.UI;

namespace Sudoku.Scripts.Game
{
    public class NewGamePopup : Popup
    {
        public Button ContinueButton;
        public Button NewGameButton;

        Button continueButton;
        Button newGameButton;

        private void Start()
        {
            GameManager.Instance.PauseGame(); 
            continueButton = ContinueButton.GetComponent<Button>();
            continueButton.onClick.AddListener(continueClicked);
            newGameButton = NewGameButton.GetComponent<Button>();
            newGameButton.onClick.AddListener(newGameClicked);
        }


        public override void Hide(bool result)
        {
            base.Hide(result);
        }

        public void newGameClicked()
        {
            base.Hide(true);
            ScreenManager.Instance.OnGameSelectNewGame(); // Вызываем метод начала новой игры из ScreenManager
        }

        public void continueClicked()
        {
            base.Hide(true);
            GameManager.Instance.ResumeGame();         }
    }
}

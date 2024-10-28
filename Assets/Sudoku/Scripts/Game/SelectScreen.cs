using dotmob;
using Sudoku.Framework.Scripts.Screen;
using UnityEngine;
using Screen = Sudoku.Framework.Scripts.Screen.Screen;
using Screen_Screen = Sudoku.Framework.Scripts.Screen.Screen;

namespace Sudoku.Scripts.Game

{
    public class SelectScreen : Screen_Screen
    {
        [SerializeField] private GameObject topbar; // Верхняя панель

        protected override void Start()
        {
        
        }
    
        // Метод для обработки нажатия кнопки "Новая игра"
        public void OnNewGameButtonClick(string groupName)
        {        
            ScreenManager.Instance.OpenHomeScreen();

            GameManager.Instance.PlayNewGame(groupName); // Вызываем метод начала новой игры из ScreenManager
            topbar.SetActive(true);

        }

        public void Show(bool back, bool immediate)
        {
            base.Show(back, immediate);
        }
    }
}

using dotmob;
using Sudoku.Framework.Scripts.Screen;
using UnityEngine;
using UnityEngine.UI;
using Screen = Sudoku.Framework.Scripts.Screen.Screen;
using Screen_Screen = Sudoku.Framework.Scripts.Screen.Screen;

namespace Sudoku.Scripts.Game
{
    public class GameMenuScreen : Screen_Screen
    {
        [SerializeField] private Button newGameButtonOnly;  // Кнопка "Новая игра"

        [SerializeField] private Button continueButton; // Кнопка "Продолжить игру"
        [SerializeField] private Button newGameButton;  // Кнопка "Новая игра"
        [SerializeField] private Text gameMenuMessage;  // Текстовое сообщение на экране меню

        protected override void Start()
        {
            // Привязка действий к кнопкам
            continueButton.onClick.AddListener(OnContinueButtonClick);
            newGameButton.onClick.AddListener(OnNewGameButtonClick);
            newGameButtonOnly.onClick.AddListener(OnNewGameButtonClick);

            // Проверяем, есть ли сохраненная игра, чтобы показать или скрыть кнопку продолжения
            if (GameManager.Instance.ActivePuzzleData == null)
            {
                continueButton.gameObject.SetActive(false); // Скрываем кнопку "Продолжить", если нет сохраненной игры
                newGameButton.gameObject.SetActive(false); // Скрываем кнопку "Продолжить", если нет сохраненной игры
                newGameButtonOnly.gameObject.SetActive(true); // Скрываем кнопку "Продолжить", если нет сохраненной игры
            }
            else
            {
                continueButton.gameObject.SetActive(true); // Скрываем кнопку "Продолжить", если нет сохраненной игры
                newGameButton.gameObject.SetActive(true); // Скрываем кнопку "Продолжить", если нет сохраненной игры
                newGameButtonOnly.gameObject.SetActive(false); // Скрываем кнопку "Продолжить", если нет сохраненной игры

            }
        }

        // Метод для обработки нажатия кнопки "Продолжить игру"
        private void OnContinueButtonClick()
        {
            ScreenManager.Instance.OnGameSelectContinue(); // Вызываем метод продолжения игры из ScreenManager
        }

        // Метод для обработки нажатия кнопки "Новая игра"
        private void OnNewGameButtonClick( )
        {
            ScreenManager.Instance.OnGameSelectNewGame(); // Вызываем метод начала новой игры из ScreenManager
        }

        public  void Show(bool back, bool immediate)
        {
            base.Show(back, immediate);
            gameMenuMessage.text = "Select an option"; // Можно задать любое сообщение для экрана меню
        }
      
    }
}
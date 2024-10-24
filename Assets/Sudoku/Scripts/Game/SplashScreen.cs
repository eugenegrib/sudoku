
    using UnityEngine;
    using UnityEngine.UI;

    namespace dotmob.Sudoku
    {
        public class SplashScreen : Screen
        {
            [SerializeField] private Text splashMessage;

            public override void Show(bool back, bool immediate)
            {
                base.Show(back, immediate);
                splashMessage.text = "Welcome to the Game!"; // или любой другой текст
            }

            private void Update()
            {
                // Логика обновления, если необходимо
            }
        }
    }


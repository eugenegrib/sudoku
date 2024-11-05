using UnityEngine;
using UnityEngine.UI;
using Screen = Sudoku.Framework.Scripts.Screen.Screen;

    namespace Sudoku.Scripts.Game
    {
        public class SplashScreen : Screen
        {
            [SerializeField] private Text splashMessage;

            public override void Show(bool back, bool immediate)
            {
                base.Show(back, immediate);
            }

            private void Update()
            {
                // Логика обновления, если необходимо
            }
        }
    }


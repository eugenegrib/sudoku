﻿using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;
using Screen = Sudoku.Framework.Scripts.Screen.Screen;

namespace Sudoku.Scripts.Game
{
    public class GameScreen : Screen
    {
        #region Inspector Variables

        [Space] [SerializeField] private Text difficultyText = null;
        [SerializeField] private Text timeText = null;
        [SerializeField] private Text mistake = null;
        [SerializeField] public LocalizedString mistakeTitle = null;
        #endregion

        #region Unity Methods

        private void Start()
        {
        }

        private void Update()
        {
            mistake.text = mistakeTitle.GetLocalizedString() + " "+ GameManager.Instance.checkMistake.ToString() + "/2";
            timeText.text = GameManager.Instance.ActivePuzzleTimeStr;
            if (GameManager.Instance.ActivePuzzleData != null)
            {
                difficultyText.text = GameManager.Instance.ActivePuzzleDifficultyStr;
            }
        }

        #endregion

        #region Public Methods

        public override void Show(bool back, bool immediate)
        {
            base.Show(back, immediate);

            if (GameManager.Instance.ActivePuzzleData != null)
            {
                difficultyText.text = GameManager.Instance.ActivePuzzleDifficultyStr;
            }
        }

        #endregion
    }
}
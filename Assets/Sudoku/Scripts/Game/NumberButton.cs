﻿using System.Collections;
using System.Collections.Generic;
using Sudoku.Scripts.Theme;
using UnityEngine;
using UnityEngine.UI;

namespace dotmob.Sudoku
{
    public class NumberButton : ClickableListItem, IThemeBehaviour
    {
        #region Inspector Variables

        [SerializeField] private Text numberText = null;

        #endregion

        #region Member Variables

        private CanvasGroup cg;

        #endregion

        #region Public Methods

        public void Setup(int number)
        {
            numberText.text = number.ToString();
            SetVisible(true);

            // Регистрируемся в ThemeManager, чтобы получать уведомления о смене темы
            ThemeManager.Instance.Register(this);
        }

        public void SetVisible(bool isVisible)
        {
            if (cg == null)
            {
                cg = gameObject.GetComponent<CanvasGroup>();

                if (cg == null)
                {
                    cg = gameObject.AddComponent<CanvasGroup>();
                }
            }

            cg.alpha = isVisible ? 1 : 0;
            cg.interactable = isVisible;
            cg.blocksRaycasts = isVisible;
        }

        /// <summary>
        /// Changes the color of the number text manually
        /// </summary>
        public void SetTextColor(Color color)
        {
            if (numberText != null)
            {
                numberText.color = color;
                numberText.fontStyle = FontStyle.Normal;
            }
        }

        /// <summary>
        /// Notify when theme changes, and update the text color accordingly
        /// </summary>
        public void NotifyThemeChanged()
        {
            // Получаем цвет для этого элемента по его идентификатору (например, "numberButtonColor")
            if (ThemeManager.Instance.GetItemColor("numberButtonColor", out Color newColor))
            {
                SetTextColor(newColor);
            }
        }

        private void OnDestroy()
        {
            // Отписываемся от ThemeManager, чтобы избежать утечек памяти
            ThemeManager.Instance?.Unregister(this);
        }

        #endregion
    }
}
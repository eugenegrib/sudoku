using System.Collections.Generic;
using dotmob.Sudoku;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Sudoku.Scripts.Theme
{
    public class ThemeGraphicBehaviour : ThemeBehaviour
    {
        #region Member Variables

        private List<Graphic> graphics;  // Список всех графических компонентов, которые мы будем менять

        #endregion

        #region Unity Methods

        private void Awake()
        {
            // Собираем только компоненты на этом объекте, а не на дочерних
            graphics = new List<Graphic>(gameObject.GetComponents<Graphic>());

            if (graphics.Count == 0)
            {
                Debug.LogError("[ThemeGraphicBehaviour] No Graphic components found on this GameObject, gameObject.name: " + gameObject.name);
            }
        }

        #endregion

        #region Protected Methods

        protected override void SetColor(Color color)
        {
            // Устанавливаем цвет только для текущих графических компонентов
            foreach (var graphic in graphics)
            {
                if (graphic != null)
                {
                    graphic.color = color;
                }
            }
        }

        #endregion
    }
}
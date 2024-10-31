using dotmob;
using dotmob.Sudoku;
using Sudoku.Scripts.Theme;
using UnityEngine;
using UnityEngine.UI;

// Убедитесь, что пространство имен Theme подключено

namespace Sudoku.Scripts.Game
{
    public class StatsListItem : MonoBehaviour, IThemeBehaviour // Реализуем интерфейс IThemeBehaviour
    {
        #region Inspector Variables

        [SerializeField] private Text groupNameText = null;
        [SerializeField] private Text puzzleCompletedText = null;
        [SerializeField] private Text bestTimeText = null;
        [SerializeField] private Text averageTimeText = null;

        private string groupNameTextId = "groupNameColor"; // Идентификатор цвета для groupNameText
        private string puzzleCompletedTextId = "puzzleCompletedColor"; // Идентификатор цвета для puzzleCompletedText
        private string bestTimeTextId = "bestTimeColor"; // Идентификатор цвета для bestTimeText
        private string averageTimeTextId = "averageTimeColor"; // Идентификатор цвета для averageTimeText

        #endregion

        #region Public Methods

        public void Setup(PuzzleGroupData puzzleGroup)
        {
            groupNameText.text = puzzleGroup.displayNameLocalized.GetLocalizedString();
            puzzleCompletedText.text = puzzleGroup.PuzzlesCompleted.ToString();
            bestTimeText.text = Utilities.FormatTimer(puzzleGroup.MinTime);

            if (puzzleGroup.PuzzlesCompleted == 0)
            {
                averageTimeText.text = "00:00";
            }
            else
            {
                averageTimeText.text = Utilities.FormatTimer(puzzleGroup.TotalTime / puzzleGroup.PuzzlesCompleted);
            }

            NotifyThemeChanged(); // Уведомляем об изменении темы для установки цветов
        }

        #endregion

        #region IThemeBehaviour Implementation

        public void NotifyThemeChanged()
        {
            // Запрашиваем цвета у ThemeManager
            if (ThemeManager.Instance.GetItemColor(groupNameTextId, out Color groupNameColor))
            {
                groupNameText.color = groupNameColor;
            }

            if (ThemeManager.Instance.GetItemColor(puzzleCompletedTextId, out Color puzzleCompletedColor))
            {
                puzzleCompletedText.color = puzzleCompletedColor;
            }

            if (ThemeManager.Instance.GetItemColor(bestTimeTextId, out Color bestTimeColor))
            {
                bestTimeText.color = bestTimeColor;
            }

            if (ThemeManager.Instance.GetItemColor(averageTimeTextId, out Color averageTimeColor))
            {
                averageTimeText.color = averageTimeColor;
            }
        }
        void Start()
        {
            ThemeManager.Instance.Register(this);
        }

        void OnDestroy()
        {
            ThemeManager.Instance.Unregister(this);
        }


        #endregion
    }
    
}

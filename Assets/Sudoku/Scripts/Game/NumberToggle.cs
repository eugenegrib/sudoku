using dotmob;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Sudoku.Framework.Scripts.UI
{
    public class NumbersToggle : UIMonoBehaviour, IPointerClickHandler
    {
        #region Inspector Variables

        [SerializeField] private Image numbersOnImage = null;
        [SerializeField] private Image numbersOffImage = null;

        #endregion

        #region Member Variables

        private bool isNumbersOn = true;

        #endregion

        #region Properties

        public System.Action<bool> OnNumbersToggleChanged { get; set; }

        #endregion

        #region Unity Methods

        private void Start()
        {
            UpdateNumbersVisuals(isNumbersOn);
        }

        #endregion

        #region Public Methods

        public void OnPointerClick(PointerEventData eventData)
        {
            ToggleNumbers(!isNumbersOn);
        }

        public void SetNumbersToggle(bool on)
        {
            isNumbersOn = on;
            UpdateNumbersVisuals(on);
            OnNumbersToggleChanged?.Invoke(on);
        }

        #endregion

        #region Private Methods

        private void ToggleNumbers(bool state)
        {
            SetNumbersToggle(state);
        }

        private void UpdateNumbersVisuals(bool state)
        {
            numbersOffImage.gameObject.SetActive(!state);
            numbersOnImage.gameObject.SetActive(state);
        }

        #endregion
    }
}
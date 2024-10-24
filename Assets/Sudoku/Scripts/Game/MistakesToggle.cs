using dotmob;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Sudoku.Framework.Scripts.UI
{
    public class MistakesToggle : UIMonoBehaviour, IPointerClickHandler
    {
        #region Inspector Variables

        [SerializeField] private Image mistakesOnImage = null;
        [SerializeField] private Image mistakesOffImage = null;

        #endregion

        #region Member Variables

        private bool isMistakesOn = true;

        #endregion

        #region Properties

        public System.Action<bool> OnMistakesToggleChanged { get; set; }

        #endregion

        #region Unity Methods

        private void Start()
        {
            UpdateMistakesVisuals(isMistakesOn);
        }

        #endregion

        #region Public Methods

        public void OnPointerClick(PointerEventData eventData)
        {
            ToggleMistakes(!isMistakesOn);
        }

        public void SetMistakesToggle(bool on)
        {
            isMistakesOn = on;
            UpdateMistakesVisuals(on);
            OnMistakesToggleChanged?.Invoke(on);
        }

        #endregion

        #region Private Methods

        private void ToggleMistakes(bool state)
        {
            SetMistakesToggle(state);
        }

        private void UpdateMistakesVisuals(bool state)
        {
            mistakesOffImage.gameObject.SetActive(!state);
            mistakesOnImage.gameObject.SetActive(state);
        }

        #endregion
    }
}
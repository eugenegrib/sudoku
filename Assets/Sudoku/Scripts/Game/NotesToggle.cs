using dotmob;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Sudoku.Framework.Scripts.UI
{
    public class NotesToggle : UIMonoBehaviour, IPointerClickHandler
    {
        #region Inspector Variables

        [SerializeField] private Image notesOnImage = null;
        [SerializeField] private Image notesOffImage = null;

        #endregion

        #region Member Variables

        private bool isNotesOn = true;

        #endregion

        #region Properties

        public System.Action<bool> OnNotesToggleChanged { get; set; }

        #endregion

        #region Unity Methods

        private void Start()
        {
            UpdateNotesVisuals(isNotesOn);
        }

        #endregion

        #region Public Methods

        public void OnPointerClick(PointerEventData eventData)
        {
            ToggleNotes(!isNotesOn);
        }

        public void SetNotesToggle(bool on)
        {
            isNotesOn = on;
            UpdateNotesVisuals(on);
            OnNotesToggleChanged?.Invoke(on);
        }

        #endregion

        #region Private Methods

        private void ToggleNotes(bool state)
        {
            SetNotesToggle(state);
        }

        private void UpdateNotesVisuals(bool state)
        {
            notesOffImage.gameObject.SetActive(!state);
            notesOnImage.gameObject.SetActive(state);
        }

        #endregion
    }
}
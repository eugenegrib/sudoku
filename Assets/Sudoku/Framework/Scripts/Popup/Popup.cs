using dotmob;
using Sudoku.Framework.Scripts.UI;
using UnityEngine;

namespace Sudoku.Framework.Scripts.Popup
{
    public class Popup : UIMonoBehaviour
    {
        #region Enums

        private enum State
        {
            Shown,
            Hidden,
            Showing,
            Hidding
        }

        #endregion

        #region Inspector Variables

        [SerializeField] protected bool canAndroidBackClosePopup;
        [SerializeField] protected PopupAnimator popupAnimator;

        #endregion

        #region Member Variables

        private bool isInitialized;
        private State state;
        private PopupClosed callback;

        #endregion

        #region Properties

        public bool CanAndroidBackClosePopup { get { return canAndroidBackClosePopup; } }

        #endregion

        #region Delegates

        public delegate void PopupClosed(bool cancelled, object[] outData);

        #endregion

        #region Public Methods

        public virtual void Initialize()
        {
            gameObject.SetActive(false);
            state = State.Hidden;
        }

        public void Show()
        {
            Show(null, null);
        }

        public bool Show(object[] inData, PopupClosed callback)
        {
            if (state != State.Hidden)
            {
                return false;
            }

            this.callback = callback;
            this.state = State.Showing;

            // Показать объект попапа
            gameObject.SetActive(true);

            popupAnimator.ShowAnim(() =>
            {
                state = State.Shown;
                OnShowing(inData);
            });

            return true;
        }

        public virtual void Hide(bool cancelled)
        {
            Hide(cancelled, null);
        }

        public void HideWithAction(string action)
        {
            Hide(false, new object[] { action });
        }

        public virtual void OnShowing(object[] inData)
        {
        }

        public virtual void OnHiding()
        {
            PopupManager.Instance.OnPopupHiding(this);
        }

        #endregion

        #region Private Methods

        public void Hide(bool cancelled, object[] outData)
        {
            // Проверка состояния
            if (state != State.Shown && state != State.Showing)
                return;

            state = State.Hidding;
            callback?.Invoke(cancelled, outData);

            popupAnimator.HideAnim(() =>
            {
                state = State.Hidden;
                gameObject.SetActive(false);
                OnHiding();
            });
        }

        #endregion
    }
}

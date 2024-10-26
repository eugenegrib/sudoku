using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Gley.MobileAds;
using Gley.MobileAds.Scripts.ToUse;
using Sudoku.Framework.Scripts.Popup;

namespace dotmob
{
	public class Popup : UIMonoBehaviour
	{
		#region Enums

		protected enum AnimType
		{
			Fade,
			Zoom
		}

		private enum State
		{
			Shown,
			Hidden,
			Showing,
			Hidding
		}

		#endregion

		#region Inspector Variables

		[SerializeField] protected bool				canAndroidBackClosePopup;

		[Header("Anim Settings")]
		[SerializeField] protected float			animDuration;
		[SerializeField] protected AnimType			animType;
		[SerializeField] protected AnimationCurve	animCurve;
		[SerializeField] protected RectTransform	animContainer;

		#endregion

		#region Member Variables

		private bool		isInitialized;
		private State		state;
		private PopupClosed	callback;

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
			CG.alpha = 0f;
			state = State.Hidden;
		}

		public void Show()
		{
			Show(null, null);
			//Time.timeScale = 1;
		}

		public bool Show(object[] inData, PopupClosed callback)
		{
			//Advertisements.Instance.HideBanner();
			API.HideBanner();
			
			if (state != State.Hidden)
			{
				return false;
			}

			this.callback	= callback;
			this.state		= State.Showing;

			// Show the popup object
			gameObject.SetActive(true);

			switch (animType)
			{
				case AnimType.Fade:
					DoFadeAnim();
					break;
				case AnimType.Zoom:
					DoZoomAnim();
					break;
			}

			OnShowing(inData);

			return true;
		}

		public virtual void Hide(bool cancelled)
		{
			//Advertisements.Instance.ShowBanner(BannerPosition.BOTTOM, BannerType.Banner);
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

		private void DoFadeAnim()
{
    // Анимация самого попапа
    UIAnimation anim = UIAnimation.Alpha(CG, 0f, 1f, animDuration);
    anim.startOnFirstFrame = true;
    anim.OnAnimationFinished += (GameObject obj) => { state = State.Shown; };
    anim.Play();

    // Анимация для всех дочерних объектов
    foreach (Transform child in animContainer)
    {
        CanvasGroup childCanvasGroup = child.GetComponent<CanvasGroup>();
        if (childCanvasGroup != null)
        {
            UIAnimation childAnim = UIAnimation.Alpha(childCanvasGroup, 0f, 1f, animDuration);
            childAnim.startOnFirstFrame = true;
            childAnim.Play();
        }
    }
}

private void DoZoomAnim()
{
    // Анимация альфа-канала и масштабирования для pop-up
    UIAnimation anim = UIAnimation.Alpha(CG, 0f, 1f, animDuration);
    anim.style = UIAnimation.Style.EaseOut;
    anim.startOnFirstFrame = true;
    anim.Play();

    anim = UIAnimation.ScaleX(animContainer, 0f, 1f, animDuration);
    anim.style = UIAnimation.Style.Custom;
    anim.animationCurve = animCurve;
    anim.startOnFirstFrame = true;
    anim.Play();

    anim = UIAnimation.ScaleY(animContainer, 0f, 1f, animDuration);
    anim.style = UIAnimation.Style.Custom;
    anim.animationCurve = animCurve;
    anim.startOnFirstFrame = true;
    anim.OnAnimationFinished += (GameObject obj) => { state = State.Shown; };
    anim.Play();

    // Анимация для всех дочерних объектов
    foreach (Transform child in animContainer)
    {
        CanvasGroup childCanvasGroup = child.GetComponent<CanvasGroup>();
        if (childCanvasGroup != null)
        {
            UIAnimation childAnim = UIAnimation.Alpha(childCanvasGroup, 0f, 1f, animDuration);
            childAnim.startOnFirstFrame = true;
            childAnim.Play();
        }
    }
}

public void Hide(bool cancelled, object[] outData)
{
    API.ShowBanner(BannerPosition.Bottom, BannerType.Adaptive);

    // Проверка состояния
    if (state != State.Shown && state != State.Showing)
        return;

    state = State.Hidding;
    callback?.Invoke(cancelled, outData);

    // Анимация для самого контейнера
    UIAnimation anim = UIAnimation.Alpha(CG, 1f, 0f, animDuration);
    anim.style = UIAnimation.Style.EaseOut;
    anim.startOnFirstFrame = true;

    anim.OnAnimationFinished += (GameObject target) => 
    {
        state = State.Hidden;
        gameObject.SetActive(false);
    };
    anim.Play();

    // Анимация для всех дочерних объектов
    foreach (Transform child in animContainer)
    {
        CanvasGroup childCanvasGroup = child.GetComponent<CanvasGroup>();
        if (childCanvasGroup != null)
        {
            UIAnimation childAnim = UIAnimation.Alpha(childCanvasGroup, 1f, 0f, animDuration);
            childAnim.style = UIAnimation.Style.EaseOut;
            childAnim.startOnFirstFrame = true;
            childAnim.Play();
        }
    }

    OnHiding();
}


		#endregion
	}
}
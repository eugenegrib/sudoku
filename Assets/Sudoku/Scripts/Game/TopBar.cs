﻿using System.Collections;
using System.Collections.Generic;
using Sudoku.Framework.Scripts.Screen;
using UnityEngine;

namespace dotmob.Sudoku
{
	public class TopBar : MonoBehaviour
	{
		#region Inspector Variables

		//[SerializeField] private Image backButton = null;

		#endregion

		#region Public Methods

		private void Start()
		{
			//backButton.alpha = 0f;

			ScreenManager.Instance.OnSwitchingScreens += OnSwitchingScreens;
		}

		#endregion

		#region Private Methods

		private void OnSwitchingScreens(string fromScreen, string toScreen)
		{
			if (fromScreen == "main")
			{
				//UIAnimation anim = UIAnimation.Alpha(backButton, 0f, 1f, 0.5f);

				//anim.style = UIAnimation.Style.EaseOut;

				//anim.Play();
			}
			else if (toScreen == "main")
			{
			//	UIAnimation anim = UIAnimation.Alpha(backButton, 1f, 0f, 0.5f);

			//	anim.style = UIAnimation.Style.EaseOut;

			//	anim.Play();
			}
		}

		#endregion
	}
}

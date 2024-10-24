using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Loading : MonoBehaviour
{
    public float loadingDelay = 2f;

	public GameObject GDPRPanel;

    // Start is called before the first frame update
    void Start()
    {
		//CheckForGDPR();
        Invoke("StartGame", loadingDelay);
    }

    void StartGame()
    {
        SceneManager.LoadSceneAsync(1);
    }



	//GDPR
	//void CheckForGDPR()
	//{
	//	//Debug.Log("vao day be :" + Advertisements.Instance.UserConsentWasSet());

	//	if (Advertisements.Instance.UserConsentWasSet() == false)
	//	{
	//		GDPRPanel.gameObject.SetActive(true);
	//		// show gdpr popup

	//		//pause the game

	//		Time.timeScale = 0;
 //       }
 //       else
 //       {
	//		GDPRPanel.gameObject.SetActive(false);
	//	}
	//}

	//Popup events
	public void OnUserClickAccept()
	{
		//Advertisements.Instance.SetUserConsent(true);
		//hide gdpr popup
		//PopupManager.Instance.CloseActivePopup();
		GDPRPanel.gameObject.SetActive(false);
		//play the game
		Time.timeScale = 1;
	}

	public void OnUserClickCancel()
	{
		//Advertisements.Instance.SetUserConsent(false);
		//hide gdpr popup
		//GDPR_Popup.SetActive(false);
		GDPRPanel.gameObject.SetActive(false);
		//play the game
		Time.timeScale = 1;
	}
}

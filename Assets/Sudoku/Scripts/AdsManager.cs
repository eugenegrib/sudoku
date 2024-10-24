using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gley.MobileAds;
using Gley.MobileAds.Scripts.ToUse;

public class AdsManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        API.Initialize();

        Debug.Log("GPDR status :" + API.GDPRConsentWasSet());

        if (!API.GDPRConsentWasSet())
        {
            API.ShowBuiltInConsentPopup(PopupCloseds);
        }

    }


    private void PopupCloseds()
    {

    }
}

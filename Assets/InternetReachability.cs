using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InternetReachability : MonoBehaviour
{
	bool isEnabled = true;

	void Update ()
	{
		if (isEnabled) {
			try {
				var status = Network.TestConnection ();
				Debug.Log ("Minha tag: " + status.ToString ());
			} catch {
				Debug.Log ("Error. Check internet connection!");
				PageNavFrameWork.PageNav.GetPageNavInstance ().SetInternetErrorVisibility (true);
				isEnabled = false;
			}
//			if (Application.internetReachability == NetworkReachability.NotReachable) {
//				Debug.Log ("Error. Check internet connection!");
//				PageNavFrameWork.PageNav.GetPageNavInstance ().SetInternetErrorVisibility (true);
//				isEnabled = false;
//			}
		}


	}
}

using UnityEngine;
using System.Collections;
using PageNavFrameWork;
using System.Collections.Generic;
using UnityEngine.UI;

public class CompleteFacebookLoginPageController : PageController
{
	public Text phone;
	public GameObject phoneError;
	string userID = "";
	string userName = "";

	public void OnCompleteLoginClick ()
	{
		if (phone.text.Length < 8) {
			phoneError.SetActive (true);
		} else {
			Loading = true;
			phoneError.SetActive (false);
			DataManager.CreateNewUserAndLogin (userID, userName, phone.text);
			Loading = false;
			Constants.LoadHomePage ();
		}
	}

	public override void InstantiatedWithArgs (Dictionary<string,object> args)
	{
		userName = (string)args ["name"];
		userID = (string)args ["id"];
	}
}

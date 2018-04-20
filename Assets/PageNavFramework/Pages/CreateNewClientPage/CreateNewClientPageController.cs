using UnityEngine;
using System.Collections;
using PageNavFrameWork;
using UnityEngine.UI;
using System.Collections.Generic;

public class CreateNewClientPageController : PageController
{
	public Text userName;
	public GameObject userNameError;
	public Text phone;
	public GameObject phoneError;

	bool isFromClientsPage = false;

	bool VerifyErrors ()
	{
		if (string.IsNullOrEmpty (userName.text)) {
			userNameError.SetActive (true);
			return true;
		} else {
			userNameError.SetActive (false);
		}
		if (phone.text.Length < 8) {
			phoneError.SetActive (true);
			return true;
		} else {
			phoneError.SetActive (false);
		}
		return false;
	}

	public void CreateNewClientToCompany ()
	{
		if (!VerifyErrors ()) {
			Loading = true;
			DataManager.CreateNewClientToCompany (userName.text, phone.text, delegate {
				Loading = false;
				if (isFromClientsPage) {
					Constants.LoadHomePage ();
				}
				CloseModal ();
			}, delegate(string error) {
				Loading = false;
				CloseModal ();
			});
		}
	}

	public override void InstantiatedWithArgs (Dictionary<string,object> args)
	{
		foreach (var key in args.Keys) {
			isFromClientsPage = (bool)args [key];
		}
	}
}

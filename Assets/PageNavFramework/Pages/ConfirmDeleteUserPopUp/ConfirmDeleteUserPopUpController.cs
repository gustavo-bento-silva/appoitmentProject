using UnityEngine;
using System.Collections;
using PageNavFrameWork;
using System.Collections.Generic;

public class ConfirmDeleteUserPopUpController : PageController
{
	UserModel user;

	void Start ()
	{

	}

	public void OnYesClick ()
	{
		Loading = true;
		DataManager.RemoveResponsibleFromCompany (user, delegate() {
			Success = true;
			Loading = false;
			CloseModal ();
			Constants.LoadHomePage ();
		}, delegate(string error) {
			Loading = false;
			Error = true;
			CloseModal ();
			Constants.LoadHomePage ();
		});
	}

	public void OnNoClick ()
	{
		CloseModal ();
	}

	public override void InstantiatedWithArgs (Dictionary<string,object> args)
	{
		foreach (var key in args.Keys) {
			user = (UserModel)args [key];
		}
	}
}

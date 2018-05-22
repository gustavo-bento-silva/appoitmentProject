using UnityEngine;
using System.Collections;
using PageNavFrameWork;
using System.Collections.Generic;
using UnityEngine.UI;

public class ConfirmDeleteUserPopUpController : PageController
{
	public Text message;
	UserModel user;

	void Start ()
	{
		message.text = string.Format ("Deseja relamente deletar a conta de {0}?", user.name);
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
		PopPageFromStack ();
	}

	public override void InstantiatedWithArgs (Dictionary<string,object> args)
	{
		foreach (var key in args.Keys) {
			user = (UserModel)args [key];
		}
	}
}

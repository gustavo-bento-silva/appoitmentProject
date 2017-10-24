using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LoginPopup : MonoBehaviour {
	Text email;
	Text password;

	void LoginClick ()
	{
		FireBaseManager.GetFireBaseInstance ().UserLogin (email.text, password.text);
	}
}

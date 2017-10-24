using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CreateNewUserPopup : MonoBehaviour {
	Text email;
	Text password;

	void CreateNewUserClick ()
	{
		FireBaseManager.GetFireBaseInstance ().CreateNewUserWithEmailAndPassword(email.text, password.text);
	}
}

using UnityEngine;
using System.Collections;
using PageNavFrameWork;
using UnityEngine.UI;

public class LoginWithEmailPopupController : PageController
{

	void Start ()
	{

	}

	void Update ()
	{

	}

	public void CheckEmail (GameObject email)
	{
		var inputField = email.GetComponent<InputField> ();
		if (string.IsNullOrEmpty (inputField.text)) {
			//TODO: exibir erro
			Debug.Log ("Email inválido!");
		}
	}

	public void CheckPassword (GameObject password)
	{
		var inputField = password.GetComponent<InputField> ();
		if (inputField.text.Length <= 6) {
			//TODO: exibir erro
			Debug.Log ("A senha deve conter mais que 6 caracteres");
		}
	}
}

using UnityEngine;
using System.Collections;
using PageNavFrameWork;
using UnityEngine.UI;

public class LoginWithEmailPopupController : PageController
{
	public GameObject email;
	public GameObject password;
	public GameObject emailError;
	public GameObject passwordError;

	public void OnLoginClick ()
	{
		var emailText = email.GetComponent<InputField> ().text;
		var passwordText = password.GetComponent<InputField> ().text;
		bool everyThingIsRight = true;

		if (string.IsNullOrEmpty (emailText) || !emailText.Contains ("@")) {
			emailError.SetActive (true);
			everyThingIsRight = false;
		} else {
			emailError.SetActive (false);
			everyThingIsRight = true;
		}
		if (passwordText.Length < 6) {
			passwordError.SetActive (true);
			everyThingIsRight = false;
		} else {
			passwordError.SetActive (false);
			everyThingIsRight = true;
		}

		if (everyThingIsRight) {
			Loading = true;
			FirebaseAuth.GetFireBaseAuthInstance ().UserLogin (email.GetComponent<InputField> ().text, email.GetComponent<InputField> ().text, delegate {
				CloseModal ();
				Loading = false;
				Success = true;
			}, delegate(string error) {
				Loading = false;
				Error = true;
			});
		}		
	}
}
using UnityEngine;
using System.Collections;
using PageNavFrameWork;
using UnityEngine.UI;

public class CreateNewUserPopupController : PageController
{

	public GameObject name;
	public GameObject nameError;
	public GameObject email;
	public GameObject emailError;
	public GameObject phone;
	public GameObject phoneError;
	public GameObject password;
	public GameObject passwordError;

	public void CreateNewUser ()
	{
		var nameText = name.GetComponent<InputField> ().text;
		var phoneText = phone.GetComponent<InputField> ().text;
		var emailText = email.GetComponent<InputField> ().text;
		var passwordText = password.GetComponent<InputField> ().text;
		bool everyThingIsRight = true;

		if (string.IsNullOrEmpty (nameText)) {
			nameError.SetActive (true);
			everyThingIsRight = false;
		} else {
			nameError.SetActive (false);
			everyThingIsRight = true;
		}
		if (passwordText.Length < 8) {
			phoneError.SetActive (true);
			everyThingIsRight = false;
		} else {
			phoneError.SetActive (false);
			everyThingIsRight = true;
		}
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
			FirebaseAuth.GetFireBaseAuthInstance ().CreateNewUserWithEmailAndPassword (nameText, phoneText, emailText, passwordText, Constants.UserType.Client, delegate {
				Loading = false;
				Success = true;
				CloseModal ();
			}, delegate(string error) {
				Loading = false;
				Error = true;
			});
		}
	}

}

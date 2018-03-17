using UnityEngine;
using System.Collections;
using PageNavFrameWork;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CreateNewUserPopupController : PageController
{
	public string homeScene;
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
		int everyThingIsRight = 0;

		if (string.IsNullOrEmpty (nameText)) {
			nameError.SetActive (true);
		} else {
			nameError.SetActive (false);
			everyThingIsRight++;
		}
		if (phoneText.Length < 8) {
			phoneError.SetActive (true);
		} else {
			phoneError.SetActive (false);
			everyThingIsRight++;
		}
		if (string.IsNullOrEmpty (emailText) || !emailText.Contains ("@")) {
			emailError.SetActive (true);
		} else {
			emailError.SetActive (false);
			everyThingIsRight++;
		}
		if (passwordText.Length < 6) {
			passwordError.SetActive (true);
		} else {
			passwordError.SetActive (false);
			everyThingIsRight++;
		}
		if (everyThingIsRight >= 4) {
			Loading = true;
			FirebaseAuth.GetFireBaseAuthInstance ().CreateNewUserWithEmailAndPassword (nameText, phoneText, emailText, passwordText, Constants.UserType.Client, delegate(string userID) {
				Loading = false;
				Success = true;
				CloseModal ();
				LoadHomeSceneAsync ();	
			}, delegate(string error) {
				Loading = false;
				Error = true;
			});
		}
	}

	void LoadHomeSceneAsync ()
	{
		SceneManager.LoadSceneAsync (homeScene);
	}

}

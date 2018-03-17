using UnityEngine;
using System.Collections;
using PageNavFrameWork;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoginWithEmailPopupController : PageController
{
	public string homeScene;
	public GameObject email;
	public GameObject password;
	public GameObject emailError;
	public GameObject passwordError;

	public void OnLoginClick ()
	{
		var emailText = email.GetComponent<InputField> ().text;
		var passwordText = password.GetComponent<InputField> ().text;
		int everyThingIsRight = 0;

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

		if (everyThingIsRight >= 2) {
			Loading = true;
			FirebaseAuth.GetFireBaseAuthInstance ().UserLogin (email.GetComponent<InputField> ().text, password.GetComponent<InputField> ().text, delegate {
				CloseModal ();
				Loading = false;
				Success = true;
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

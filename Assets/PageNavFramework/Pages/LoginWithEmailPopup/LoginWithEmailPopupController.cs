using UnityEngine;
using System.Collections;
using PageNavFrameWork;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoginWithEmailPopupController : PageController
{
	public string homeScene;
	public string loginScene;
	public GameObject container;
	public GameObject forgotPassword;
	public GameObject success;
	public Text forgotEmail;
	public GameObject forgotEmailError;
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
			FirebaseAuth.GetFireBaseAuthInstance ().UserLogin (email.GetComponent<InputField> ().text, password.GetComponent<InputField> ().text, delegate (string id) {
				DataManager.LoadUserInfoAux (id, delegate {
					Loading = false;
					LoadHomeSceneAsync ();
				}, delegate(string error) {
					Loading = false;
					Error = true;
				});
			}, delegate(string error) {
				Loading = false;
				OpenErrorPopup (error);
			});
		}		
	}

	public void ForgotPasswordClick ()
	{
		forgotPassword.SetActive (true);
		container.SetActive (false);
	}

	public void OnConfirmForgotPasswordClick ()
	{
		if (string.IsNullOrEmpty (forgotEmail.text) || !forgotEmail.text.Contains ("@")) {
			forgotEmailError.SetActive (true);
		} else {
			Loading = true;
			forgotEmailError.SetActive (false);
			FirebaseAuth.GetFireBaseAuthInstance ().ForgotPassword (forgotEmail.text, delegate() {
				success.SetActive (true);
				forgotPassword.SetActive (false);
				Loading = false;
			}, delegate(string error) {
				Error = true;
				Loading = false;
			});
		}
	}

	public void LoadLoginSceneAsync ()
	{
		SceneManager.LoadSceneAsync (loginScene);
	}

	void LoadHomeSceneAsync ()
	{
		SceneManager.LoadSceneAsync (homeScene);
	}
}

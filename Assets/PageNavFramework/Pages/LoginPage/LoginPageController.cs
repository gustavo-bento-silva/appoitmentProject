using UnityEngine;
using System.Collections;
using PageNavFrameWork;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginPageController : PageController
{
	public InputField idTest;
	public string homeScene;
	public GameObject successMessage;
	public GameObject forgotPassword;
	public Text forgotEmail;
	public GameObject forgotEmailError;
	public GameObject email;
	public GameObject password;
	public GameObject emailError;
	public GameObject passwordError;

	void Start()
	{
		Loading = false;
	}

	public void LoginTest()
	{
		Loading = true;
		FireBaseManager.GetFireBaseInstance().IsThereUser(idTest.text, delegate (bool isThereUser)
		{
			Debug.Log("MyTag: Is there user with id: " + DataManager.userID + " - " + isThereUser);
			if (isThereUser)
			{
				DataManager.userID = idTest.text;
				Loading = false;
				ChangeScene();
			}
			else
			{
				DataManager.userID = idTest.text;
				DataManager.CreateNewUserAndLogin(idTest.text, "userName" + idTest.text, idTest.text);
				Loading = false;
				ChangeScene();
			}
		}, delegate (string error)
		{
			Debug.Log(error);
		});
	}

	public void FacebookLogin()
	{
		Loading = true;
		FirebaseAuth.GetFireBaseAuthInstance().FacebookLogin(delegate (string userId, string userName)
		{
			FireBaseManager.GetFireBaseInstance().IsThereUser(userId, delegate (bool isThereUser)
			{
				Debug.Log("MyTag: isThereUserWithID " + isThereUser);
				if (isThereUser)
				{
					DataManager.userID = userId;
					Loading = false;
					ChangeScene();
				}
				else
				{
					DataManager.userID = userId;
					Loading = false;
					var dict = new Dictionary<string, object>();
					dict.Add("name", userName);
					dict.Add("id", userId);
					PageNav.GetPageNavInstance().PushPageToStackWithArgs(PagesEnum.CompleteFacebookLoginPage, dict);
				}
			}, delegate (string error)
			{
				Loading = false;
				Error = true;
			});
			//			DataManager.GetUserById (userId, delegate(UserModel user) {
			//				if (user == null) {
			//					Loading = false;
			//					var dict = new Dictionary<string, object> ();
			//					dict.Add ("name", userName);
			//					dict.Add ("id", userId);
			//					PageNav.GetPageNavInstance ().PushPageToStackWithArgs (PagesEnum.CompleteFacebookLoginPage, dict);
			//				} else {
			//					DataManager.LoadUserInfoAux (userId, delegate {
			//						Loading = false;
			//						ChangeScene ();
			//					}, delegate(string error) {
			//						Loading = false;
			//						Error = true;
			//					});
			//				}
			//			});
		}, delegate (string error)
		{
			Loading = false;
			Error = true;
		});
	}

	public void OnAnonymousLoginClick()
	{
		DataManager.userID = "-1";
		Loading = false;
		ChangeScene();
	}

	public void OnLoginClick()
	{
		var emailText = email.GetComponent<InputField>().text;
		var passwordText = password.GetComponent<InputField>().text;
		int everyThingIsRight = 0;

		if (string.IsNullOrEmpty(emailText) || !emailText.Contains("@"))
		{
			emailError.SetActive(true);
		}
		else
		{
			emailError.SetActive(false);
			everyThingIsRight++;
		}
		if (passwordText.Length < 6)
		{
			passwordError.SetActive(true);
		}
		else
		{
			passwordError.SetActive(false);
			everyThingIsRight++;
		}

		if (everyThingIsRight >= 2)
		{
			Loading = true;
			Debug.Log("MyTag: userlogin will be called");
			FirebaseAuth.GetFireBaseAuthInstance().UserLogin(email.GetComponent<InputField>().text, password.GetComponent<InputField>().text, delegate (string id)
			{
				Debug.Log("MyTag: userlogin was successfully - id: " + id);
				DataManager.userID = id;
				Loading = false;
				ChangeScene();
			}, delegate (string error)
			{
				Loading = false;
				OpenErrorPopup(error);
			});
		}
	}

	public void ForgotPasswordClick()
	{
		forgotPassword.SetActive(true);
	}

	public void ForgotPasswordDisable()
	{
		forgotPassword.SetActive(false);
	}

	public void SuccessDisable()
	{
		successMessage.SetActive(false);
	}

	public void OnConfirmForgotPasswordClick()
	{
		if (string.IsNullOrEmpty(forgotEmail.text) || !forgotEmail.text.Contains("@"))
		{
			forgotEmailError.SetActive(true);
		}
		else
		{
			Loading = true;
			forgotEmailError.SetActive(false);
			FirebaseAuth.GetFireBaseAuthInstance().ForgotPassword(forgotEmail.text, delegate ()
			{
				successMessage.SetActive(true);
				forgotPassword.SetActive(false);
				Loading = false;
			}, delegate (string error)
			{
				Error = true;
				Loading = false;
			});
		}
	}


	void ChangeScene()
	{
		SceneManager.LoadSceneAsync(homeScene);
	}

}

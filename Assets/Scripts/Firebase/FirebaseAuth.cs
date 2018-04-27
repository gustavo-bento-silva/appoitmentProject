using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;
using System;
using Firebase;

public class FirebaseAuth : MonoBehaviour
{

	Firebase.Auth.FirebaseAuth auth;
	Firebase.Auth.FirebaseUser user;

	public static FirebaseAuth _instance;

	public static FirebaseAuth GetFireBaseAuthInstance ()
	{
		return _instance;
	}

	void Awake ()
	{
		auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
		auth.StateChanged += AuthStateChanged;
		AuthStateChanged (this, null);

		if (_instance == null) {
			_instance = this;
		}
	}

	// Track state changes of the auth object.
	public void AuthStateChanged (object sender, System.EventArgs eventArgs)
	{
		if (auth.CurrentUser != user) {
			bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null;
			if (!signedIn && user != null) {
				PlayerPreferences.userIsLogged = false;
			}
			user = auth.CurrentUser;
			if (signedIn) {
				PlayerPreferences.userIsLogged = true;
			}
		}
	}

	void OnDestroy ()
	{
		auth.StateChanged -= AuthStateChanged;
		auth = null;
	}

	public void ForgotPassword (string email, Delegates.GeneralListenerSuccess successListener, Delegates.GeneralListenerFail failListener)
	{
		if (user != null) {
			auth.SendPasswordResetEmailAsync (email).ContinueWith (task => {
				if (task.IsCanceled) {
					Debug.LogError ("SendPasswordResetEmailAsync was canceled.");
					failListener ("SendPasswordResetEmailAsync was canceled.");
					return;
				}
				if (task.IsFaulted) {
					Debug.LogError ("SendPasswordResetEmailAsync encountered an error: " + task.Exception);
					failListener (GetErrorMessage (task.Exception.InnerExceptions [0] as FirebaseException));
					return;
				}

				Debug.Log ("Password reset email sent successfully.");
				successListener ();
			});
		}
	}

	public void UserLogin (string email, string password, Delegates.UserLoginSuccess successListener, Delegates.UserLoginFail failListener)
	{
		auth.SignInWithEmailAndPasswordAsync (email, password).ContinueWith (task => {
			if (task.IsCanceled) {
				Debug.LogError ("SignInWithEmailAndPasswordAsync was canceled.");
				failListener ("SignInWithEmailAndPasswordAsync was canceled.");
				return;
			}
			if (task.IsFaulted || task.Exception != null) {
				Debug.LogError ("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
				failListener (GetErrorMessage (task.Exception.InnerExceptions [0] as FirebaseException));
				return;
			}
			user = task.Result;
			var id = auth.CurrentUser.UserId;
			successListener (id);
			Debug.LogFormat ("User signed in successfully: {0} ({1})",
				user.DisplayName, user.UserId);
		});
	}

	public void CreateNewCompanyWithEmailAndPassword (string companyName, string email, string password, Delegates.CreateNewUser success, Delegates.GeneralListenerFail fail)
	{
		auth.CreateUserWithEmailAndPasswordAsync (email, password).ContinueWith (task => {
			if (task.IsCanceled) {
				Debug.LogError ("CreateUserWithEmailAndPasswordAsync was canceled.");
				fail ("Criação cancelada");
				return;
			}
			if (task.IsFaulted) {
				fail (GetErrorMessage (task.Exception.InnerExceptions [0] as FirebaseException));
				Debug.LogError ("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
				return;
			}

			// Firebase user has been created.
			user = task.Result;

			var profile = new Firebase.Auth.UserProfile {
				DisplayName = companyName
			};
			UpdateUserProfile (profile);

			success (user.UserId);
			Debug.LogFormat ("Firebase company created successfully: {0} ({1})",
				user.DisplayName, user.UserId);
		});
	}

	public void CreateNewUserWithEmailAndPassword (string name, string phone, string email, string password, Constants.UserType userType, Delegates.CreateNewUser success, Delegates.GeneralListenerFail fail)
	{
		auth.CreateUserWithEmailAndPasswordAsync (email, password).ContinueWith (task => {
			if (task.IsCanceled) {
				Debug.LogError ("CreateUserWithEmailAndPasswordAsync was canceled.");
				fail ("Criação cancelada");
				return;
			}
			if (task.IsFaulted) {
				fail (GetErrorMessage (task.Exception.InnerExceptions [0] as FirebaseException));
				Debug.LogError ("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
				return;
			}

			// Firebase user has been created.
			user = task.Result;
			if (userType == Constants.UserType.User) {
				DataManager.currentUser = FireBaseManager.GetFireBaseInstance ().CreateNewUser (auth.CurrentUser.UserId, name, phone);
			}
		
			var profile = new Firebase.Auth.UserProfile {
				DisplayName = name
			};
			UpdateUserProfile (profile);

			success (user.UserId);
			Debug.LogFormat ("Firebase user created successfully: {0} ({1})",
				user.DisplayName, user.UserId);
		});
	}

	public void UpdateUserProfile (Firebase.Auth.UserProfile profile)
	{
		user.UpdateUserProfileAsync (profile).ContinueWith (task => {
			if (task.IsCanceled) {
				Debug.LogError ("UpdateUserProfileAsync was canceled.");
				return;
			}
			if (task.IsFaulted) {
				Debug.LogError ("UpdateUserProfileAsync encountered an error: " + task.Exception);
				return;
			}

			Debug.Log ("User profile updated successfully.");
		});
	}

	public string GetErrorMessage (FirebaseException exception)
	{
		Debug.Log (exception.ToString ());
		if (exception != null) {
			var errorCode = (AuthError)exception.ErrorCode;
			return GetErrorMessage (errorCode);
		}

		return exception.ToString ();
	}

	private string GetErrorMessage (AuthError errorCode)
	{
		var message = "";
		Debug.Log ("MyTag!!" + errorCode.ToString ());
		switch (errorCode) {
		case AuthError.AccountExistsWithDifferentCredentials:
			message = "Já existe uma conta com credenciais diferentes.";
			break;
		case AuthError.MissingPassword:
			message = "É obrigatório o uso de uma senha";
			break;
		case AuthError.WeakPassword:
			message = "Senha muito fraca, é necessário ao menos 6 caracteres.";
			break;
		case AuthError.WrongPassword:
			message = "Senha incorreta";
			break;
		case AuthError.EmailAlreadyInUse:
			message = "Esse email já está sendo usado.";
			break;
		case AuthError.InvalidEmail:
			message = "Email inválido";
			break;
		case AuthError.MissingEmail:
			message = "É obrigatório o uso de uma senha";
			break;
		case AuthError.UserNotFound:
			message = "Usuário não encontrado";
			break;
		case AuthError.NetworkRequestFailed:
			message = "Sem conexão! Verifique a internet e tente novamente!";
			break;
		default:
			message = "Ocorreu um erro. Tente novamente mais tarde!";
			break;
		}
		return message;
	}
}

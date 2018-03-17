using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

	public void UserLogin (string email, string password, Delegates.UserLoginSuccess successListener, Delegates.UserLoginFail failListener)
	{
		auth.SignInWithEmailAndPasswordAsync (email, password).ContinueWith (task => {
			if (task.IsCanceled) {
				Debug.LogError ("SignInWithEmailAndPasswordAsync was canceled.");
				failListener ("SignInWithEmailAndPasswordAsync was canceled.");
				return;
			}
			if (task.IsFaulted) {
//				task.Exception as Firebase.FirebaseException
				Debug.LogError ("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
				failListener (task.Exception.ToString ());
				return;
			}

			user = task.Result;
			var id = auth.CurrentUser.UserId;
			DataManager.LoadUserInfo (id);
			successListener ();
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
				fail ("Ocorreu um erro na criação do usuário!" + task.Exception);
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
				fail ("Ocorreu um erro na criação do usuário!" + task.Exception);
				Debug.LogError ("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
				return;
			}

			// Firebase user has been created.
			user = task.Result;
			if (userType == Constants.UserType.Client) {
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
}

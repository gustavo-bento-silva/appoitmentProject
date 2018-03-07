using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirebaseAuth : MonoBehaviour
{

	Firebase.Auth.FirebaseAuth auth;
	public Firebase.Auth.FirebaseUser user;

	public static FirebaseAuth _instance;

	public static FirebaseAuth GetFireBaseAuthInstance ()
	{
		return _instance;
	}

	void Awake ()
	{
		
		auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
		
		if (_instance == null) {
			_instance = this;
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
			if (task.IsFaulted) {
				Debug.LogError ("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
				failListener (task.Exception.ToString ());
				return;
			}

			user = task.Result;
			PlayerData.auth = auth.CurrentUser;
			FireBaseManager.GetFireBaseInstance ().GetUserByID (user.UserId, delegate(UserModel user) {
				PlayerData.user = user;
			});
			successListener ();
			Debug.LogFormat ("User signed in successfully: {0} ({1})",
				user.DisplayName, user.UserId);
		});
	}

	public void CreateNewUserWithEmailAndPassword (string name, string phone, string email, string password, Constants.UserType userType, Delegates.GeneralListenerSuccess success, Delegates.GeneralListenerFail fail)
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
			PlayerData.auth = auth.CurrentUser;
			if (userType == Constants.UserType.Client) {
				PlayerData.user = FireBaseManager.GetFireBaseInstance ().CreateNewUser (name, phone);
			}  /*else if (userType == Constants.UserType.Responsable) {
				FireBaseManager.GetFireBaseInstance ().CreateNewResponsableToCompany (name);
			}*/

			var profile = new Firebase.Auth.UserProfile {
				DisplayName = name
			};
			UpdateUserProfile (profile);

			success ();
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirebaseAuth : MonoBehaviour {

	Firebase.Auth.FirebaseAuth auth;
	public Firebase.Auth.FirebaseUser user;

	public static FirebaseAuth _instance;

	public static FirebaseAuth GetFireBaseAuthInstance(){
		return _instance;
	}

	void Awake ()
	{
		
		auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
		
		if (_instance == null) {
			_instance = this;
		}
	}
	
	public void UserLogin(string email, string password){
		auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
			if (task.IsCanceled) {
				Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
				return;
			}
			if (task.IsFaulted) {
				Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
				return;
			}

			user = task.Result;
			Debug.LogFormat("User signed in successfully: {0} ({1})",
				user.DisplayName, user.UserId);
		});
	}

	public void CreateNewUserWithEmailAndPassword(string email, string password)
	{
		auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
			if (task.IsCanceled) {
				Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
				return;
			}
			if (task.IsFaulted) {
				Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
				return;
			}

			// Firebase user has been created.
			user = task.Result;
//			CreateNewUser(user.UserId, user.DisplayName);
			Debug.LogFormat("Firebase user created successfully: {0} ({1})",
				user.DisplayName, user.UserId);
		});
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;

public class FacebookManager : MonoBehaviour
{

	private static FacebookManager _instance;
	Delegates.UserLoginFail failcallback;
	Delegates.UserLoginSuccess successCalback;

	public static FacebookManager FacebookManagerInstance()
	{
		return _instance;
	}

	void Awake()
	{
		if (_instance == null)
		{
			_instance = this;
		}
		if (!FB.IsInitialized)
		{
			FB.Init(InitCallback);
		}
	}

	void InitCallback()
	{
		if (FB.IsInitialized)
		{
			Debug.Log("Facebook SDK was successfully initialized");
		}
		else
		{
			Debug.Log("Failed to Initialize the Facebook SDK");
		}
	}

	public void FacebookLogin(Delegates.UserLoginSuccess success, Delegates.UserLoginFail fail)
	{
		if (FB.IsLoggedIn)
			FB.LogOut();

		successCalback = success;
		failcallback = fail;
		var perms = new List<string>() { "public_profile", "email" };
		FB.LogInWithReadPermissions(perms, AuthCallback);
	}

	public void FacebookLogout()
	{
		if (FB.IsLoggedIn)
			FB.LogOut();
	}

	private void AuthCallback(ILoginResult result)
	{
		if (FB.IsLoggedIn)
		{
			// AccessToken class will have session details
			var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
			successCalback(aToken.TokenString);
			// Print current access token's User ID
			Debug.Log("UserID: " + aToken.UserId + "TokenID: " + aToken.TokenString);
			// Print current access token's granted permissions
			foreach (string perm in aToken.Permissions)
			{
				Debug.Log(perm);
			}
		}
		else
		{
			failcallback("User cancelled login");
			Debug.Log("User cancelled login");
		}
	}
}

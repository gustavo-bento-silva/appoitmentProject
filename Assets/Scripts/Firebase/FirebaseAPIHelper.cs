using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class FirebaseAPIHelper : MonoBehaviour
{

	string url = "https://us-central1-appointmentproject-a7233.cloudfunctions.net/addMessage?text=VamosLa";
	string addUserURL = "https://us-central1-appointmentproject-a7233.cloudfunctions.net/addUserAuth";
	string removeUserURL = "https://us-central1-appointmentproject-a7233.cloudfunctions.net/deleteUserAuth";

	public static FirebaseAPIHelper _instance;

	public static FirebaseAPIHelper GetFireBaseAPIHelperInstance ()
	{
		return _instance;
	}

	void Awake ()
	{
		if (_instance == null) {
			_instance = this;
		}
	}

	public void RemoveUser (string userID, Delegates.GeneralListenerSuccess success, Delegates.GeneralListenerFail fail)
	{
		StartCoroutine (RemoveUserAux (userID, success, fail));
	}

	public void AddUser (string email, string password, Delegates.CreateNewUser success, Delegates.GeneralListenerFail fail)
	{
		StartCoroutine (AddUserAux (email, password, success, fail));
	}

	IEnumerator CallServer ()
	{
		using (WWW www = new WWW (url)) {
			yield return www;
			Debug.Log (www.text);
		}
	}

	IEnumerator AddUserAux (string email, string password, Delegates.CreateNewUser success, Delegates.GeneralListenerFail fail)
	{
		WWWForm form = new WWWForm ();
		form.AddField ("email", email);
		form.AddField ("password", password);

		using (UnityWebRequest w = UnityWebRequest.Post (addUserURL, form)) {
			yield return w.Send ();
			if (w.isNetworkError || w.isHttpError) {
				Debug.Log (w.error);
				fail (w.error.ToString ());
			} else {
				Debug.Log ("User created! id: " + w.downloadHandler.text);
				var id = w.downloadHandler.text.Split (':');
				var idFormatted = id [1].Replace ("{", "");
				idFormatted = idFormatted.Replace ("}", "");
				idFormatted = idFormatted.Replace ("\"", "");
				success (idFormatted);
			}
		}
	}

	IEnumerator RemoveUserAux (string userID, Delegates.GeneralListenerSuccess success, Delegates.GeneralListenerFail fail)
	{
		WWWForm form = new WWWForm ();
		form.AddField ("uid", userID);

		using (UnityWebRequest w = UnityWebRequest.Post (removeUserURL, form)) {
			yield return w.Send ();
			if (w.isNetworkError || w.isHttpError) {
				Debug.Log (w.error);
				fail (w.error.ToString ());
			} else {
				Debug.Log ("User created!");
				success ();
			}
		}
	}
}

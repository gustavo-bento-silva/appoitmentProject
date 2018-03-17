using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirebaseMessaging : MonoBehaviour
{

	void Awake ()
	{
		InitializeFirebase ();
	}
	// Setup message event handlers.
	void InitializeFirebase ()
	{
		Firebase.Messaging.FirebaseMessaging.MessageReceived += OnMessageReceived;
		Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;
		Firebase.Messaging.FirebaseMessaging.Subscribe ("Teste");
		Debug.Log ("Firebase Messaging Initialized");
	}

	public virtual void OnMessageReceived (object sender, Firebase.Messaging.MessageReceivedEventArgs e)
	{
		Debug.Log ("Received a new message");
		var notification = e.Message.Notification;
		if (notification != null) {
			Debug.Log ("title: " + notification.Title);
			Debug.Log ("body: " + notification.Body);
		}
		if (e.Message.From.Length > 0)
			Debug.Log ("from: " + e.Message.From);
		if (e.Message.Link != null) {
			Debug.Log ("link: " + e.Message.Link.ToString ());
		}
		if (e.Message.Data.Count > 0) {
			Debug.Log ("data:");
			foreach (System.Collections.Generic.KeyValuePair<string, string> iter in
				e.Message.Data) {
				Debug.Log ("  " + iter.Key + ": " + iter.Value);
			}
		}
	}

	public virtual void OnTokenReceived (object sender, Firebase.Messaging.TokenReceivedEventArgs token)
	{
		Debug.Log ("Received Registration Token: " + token.Token);
	}

	// End our messaging session when the program exits.
	public void OnDestroy ()
	{
		Firebase.Messaging.FirebaseMessaging.MessageReceived -= OnMessageReceived;
		Firebase.Messaging.FirebaseMessaging.TokenReceived -= OnTokenReceived;
	}
}

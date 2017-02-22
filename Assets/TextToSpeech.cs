using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;

public class TextToSpeech : MonoBehaviour {

	public string words = "Hello";
	public AudioSource audio;

	void Start()
	{
		audio = gameObject.GetComponent<AudioSource>();
	}

	public void OnSpeak()
	{
		StartCoroutine (Speak ());
	}

	IEnumerator Speak ()
	{
		// Remove the "spaces" in excess
		Regex rgx = new Regex ("\\s+");
		// Replace the "spaces" with "% 20" for the link Can be interpreted
		string result = rgx.Replace (words, "%20");
		string url = "http://translate.google.com/translate_tts?tl=en&q=" + result;
		WWW www = new WWW (url);
		yield return www;
		audio.clip = www.GetAudioClip (false, false, AudioType.WAV);
		audio.Play ();
	}

}

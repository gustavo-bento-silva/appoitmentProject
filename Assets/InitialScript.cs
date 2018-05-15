using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InitialScript : MonoBehaviour
{

	void Start ()
	{
		if (PlayerPreferences.userIsLogged) {
			Debug.Log ("MyTag: User is logged with id: " + DataManager.userID);
			SceneManager.LoadSceneAsync ("MainScene");
		} else {
			SceneManager.LoadSceneAsync ("LoginScene");
		}
	}

}

using UnityEngine;
using System.Collections;
using PageNavFrameWork;
using UnityEngine.SceneManagement;

public class SuccessPopupController : PageController
{

	public string sceneName;

	void Start ()
	{

	}

	void Update ()
	{

	}

	public void LoadHomeScene ()
	{
		SceneManager.LoadSceneAsync (sceneName);
	}
}

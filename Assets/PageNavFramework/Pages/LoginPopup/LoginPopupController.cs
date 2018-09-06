using UnityEngine;
using System.Collections;
using PageNavFrameWork;
using UnityEngine.SceneManagement;

public class LoginPopupController : PageController
{

	void Start()
	{

	}

	void Update()
	{

	}

	public void OnLoginClick()
	{
		SceneManager.LoadSceneAsync("LoginScene");
	}
}

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Constants
{
	public const string dateformat = "dd-MM-yyyy";

	public enum UserType
	{
		Client,
		User,
		Responsible,
		Company,
		Admin,
		None
	}

	public static void LoadHomePage ()
	{
		SceneManager.LoadSceneAsync ("MainScene");
	}
	
}

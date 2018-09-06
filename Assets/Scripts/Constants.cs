using System.Collections;
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
		Anonymous,
		None
	}

	public enum MessageType
	{
		ScheduleAppointment,
		RemoveAppointment
	}

	public static void LoadHomePage()
	{
		SceneManager.LoadSceneAsync("MainScene");
	}

}

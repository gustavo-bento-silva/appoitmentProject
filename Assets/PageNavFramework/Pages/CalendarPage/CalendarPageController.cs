using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PageNavFrameWork;
using UnityEngine.UI;

public class CalendarPageController : PageController
{

	public Dropdown dropdown;
	public GameObject calendars;

	private int actualPositionIndex = 0;
	private int positionXOffset = 1241;

	void Start ()
	{
		Delegates.GetAllResponsibles getAllResponsibles = GetEmployerList;
		DataManager.GetAllResponsables (getAllResponsibles);
	}

	void Update ()
	{

	}

	void GetEmployerList (List<ResponsableModel> responsiblesList)
	{
		List<String> namesList = new List<string> ();
		dropdown.ClearOptions ();
		foreach (var employee in responsiblesList) {
			namesList.Add (employee.name);
		}
		
		dropdown.AddOptions (namesList);
		dropdown.onValueChanged.AddListener (GetEmployeeSelected);

	}

	void GetEmployeeSelected (int newPosition)
	{
		Debug.Log (newPosition);
	}

	public void OnNextButtonClick ()
	{
		actualPositionIndex++;
		var position = calendars.transform.localPosition.x - positionXOffset;
		iTween.MoveTo (calendars, iTween.Hash ("x", position, "islocal", true, "time", 0.7, "easetype", iTween.EaseType.easeInOutBack));
	}

	public void OnBackButtonClick ()
	{
		actualPositionIndex--;
		var position = calendars.transform.localPosition.x + positionXOffset;
		iTween.MoveTo (calendars, iTween.Hash ("x", position, "islocal", true, "time", 0.7, "easetype", iTween.EaseType.easeInOutBack));
		
	}
		
}

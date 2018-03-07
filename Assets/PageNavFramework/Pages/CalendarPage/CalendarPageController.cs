using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PageNavFrameWork;
using UnityEngine.UI;

public class CalendarPageController : PageController
{

	public Dropdown responsibleDropdown;
	public Dropdown servicesDropdown;
	public GameObject calendars;

	private int actualPositionIndex = 0;
	private int positionXOffset = 1241;

	void Start ()
	{
		Loading = true;
		Delegates.GetAllResponsibles getAllResponsibles = GetEmployerList;
		DataManager.GetAllResponsablesFromCompany (getAllResponsibles);
	}

	void Update ()
	{

	}

	void GetEmployerList (List<ResponsibleModel> responsiblesList)
	{
		DataManager.responsibles = responsiblesList;
		List<String> namesList = new List<string> ();
		responsibleDropdown.ClearOptions ();
		foreach (var employee in responsiblesList) {
			namesList.Add (employee.name);
		}
		
		responsibleDropdown.AddOptions (namesList);
		DataManager.currentResponsible = DataManager.responsibles [0];
		GetAllServices ();
		responsibleDropdown.onValueChanged.AddListener (GetEmployeeSelected);

	}

	void GetAllServices ()
	{
		Delegates.GetAllServicesProvided getAllResponsiblesWithServices = CallbackGetAllServices;
		DataManager.GetServicesFromAllResponsibles (getAllResponsiblesWithServices);
	}

	void CallbackGetAllServices (List<ResponsibleModel> responsibles)
	{
		FillDropDownServices ();
		servicesDropdown.onValueChanged.AddListener (GetServiceSelected);
	}

	void FillDropDownServices ()
	{
		List<String> namesList = new List<string> ();
		servicesDropdown.ClearOptions ();
		foreach (ServicesProvidedModel service in DataManager.currentResponsible.servicesProvided.Values) {
			namesList.Add (service.name);
		}

		servicesDropdown.AddOptions (namesList);
		GetServiceSelected (0);
		Loading = false;
	}


	void GetServiceSelected (int newPosition)
	{
		DataManager.currentResponsible = DataManager.responsibles [newPosition];
	}

	void GetEmployeeSelected (int newPosition)
	{
		DataManager.currentResponsible = DataManager.responsibles [newPosition];
		FillDropDownServices ();
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

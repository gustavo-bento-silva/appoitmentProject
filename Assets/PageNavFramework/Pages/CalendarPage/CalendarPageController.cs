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
	public CalendarController[] calendarsController;

	private int actualPositionIndex = 0;
	private int positionXOffset = 1127;

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
		FillCalendars ();
	}

	void FillCalendars ()
	{
		foreach (var calendarController in calendarsController) {
			calendarController.StartFillCalendar ();
		}
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
		Loading = true;
		var servicesList = new List<ServicesProvidedModel> ();
		foreach (var service in DataManager.currentResponsible.servicesProvided.Values) {
			servicesList.Add ((ServicesProvidedModel)service);
		}

		DataManager.currentservice = servicesList [newPosition];
		Loading = false;
	}

	void GetEmployeeSelected (int newPosition)
	{
		Loading = true;
		DataManager.currentResponsible = DataManager.responsibles [newPosition];
		FillDropDownServices ();
		FillCalendars ();
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

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PageNavFrameWork;
using UnityEngine.UI;

public class CalendarPageController : PageController
{
	public GameObject container;
	public Dropdown responsibleDropdown;
	public Dropdown servicesDropdown;
	public GameObject calendars;
	public CalendarController[] calendarsController;

	public Transform ResponsibleCellPrefab;
	public RectTransform ResponsibleScrollContentList;

	public Transform ServiceCellPrefab;
	public RectTransform ServiceScrollContentList;

	List<ResponsibleModel> responsibles = new List<ResponsibleModel> ();
	List <GameObject> responsibleCell = new List<GameObject> ();

	List<ServicesProvidedModel> services = new List<ServicesProvidedModel> ();
	List <GameObject> servicesCell = new List<GameObject> ();

	private int actualPositionIndex = 0;
	private int actualPagePosition = 0;
	private int positionXOffset = 1127;

	void Start ()
	{
		Loading = true;
		Delegates.GetAllResponsibles getAllResponsibles = GetEmployerList;
		DataManager.GetAllResponsablesFromCompany (getAllResponsibles);
	}

	void GetEmployerList (List<ResponsibleModel> responsiblesList)
	{
		DataManager.responsibles = responsiblesList;
		GetAllServices ();
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

	void CallbackGetAllServices (List<ResponsibleModel> mResponsibles)
	{
		responsibles = mResponsibles;
		FillEmployerList ();
	}

	void FillEmployerList ()
	{
		var index = 0;
		responsibles.ForEach (x => {
			responsibleCell.Add (SelectResponsibleCellController.Instantiate (ResponsibleCellPrefab, x, index, delegate(ResponsibleModel responsible, int mindex) {
				OnResponsibleSelected (responsible, mindex);
			}));
			index++;
		});
		StartCoroutine (OnFillEmployerList ());
	}

	IEnumerator OnFillEmployerList ()
	{
		yield return new WaitForSeconds (1f);
		responsibleCell.ForEach (x => x.transform.SetParent (ResponsibleScrollContentList, false));
		ReadjustScrollSize (responsibleCell.Count);
		Loading = false;
	}

	void FillServiceList ()
	{
		Loading = true;
		var index = 0;
		services.ForEach (x => {
			servicesCell.Add (SelectServiceCellController.Instantiate (ServiceCellPrefab, x, index, delegate(ServicesProvidedModel serviceprovided, int mIndex) {
				OnServiceSelected (serviceprovided, mIndex);
			}));
			index++;
		});
		StartCoroutine (OnFillServiceList ());
	}

	IEnumerator OnFillServiceList ()
	{
		yield return new WaitForSeconds (1f);
		servicesCell.ForEach (x => x.transform.SetParent (ServiceScrollContentList, false));
		ReadjustScrollSize (servicesCell.Count);
		Loading = false;
	}


	void FillDropDownServices ()
	{
		List<String> namesList = new List<string> ();
		servicesDropdown.ClearOptions ();
		foreach (ServicesProvidedModel service in DataManager.currentResponsible.servicesProvided.Values) {
			var servicePrice = float.Parse (service.price) % 1;
			namesList.Add (string.Format ("{0} - R${1},{2}", service.name, Mathf.Floor (float.Parse (service.price)), servicePrice.ToString ("00")));
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

	public void OnServiceSelected (ServicesProvidedModel service, int index)
	{
		DataManager.currentservice = service;
		GoToNextPage ();
	}

	public void OnResponsibleSelected (ResponsibleModel responsible, int index)
	{
		DataManager.currentResponsible = responsible;
		services.Clear ();
		foreach (var key in responsible.servicesProvided.Keys) {
			services.Add ((ServicesProvidedModel)responsible.servicesProvided [key]);
		}
		FillCalendars ();
		FillServiceList ();
		GoToNextPage ();
	}

	public void GoToNextPage ()
	{
		actualPagePosition++;
		var position = container.transform.localPosition.x - 1451.8f;
		iTween.MoveTo (container, iTween.Hash ("x", position, "islocal", true, "time", 0.7, "easetype", iTween.EaseType.easeInOutBack));
	}

	public void GoToPrevioslyPage ()
	{
		if (actualPagePosition == 2) {
			GoCalendarsToOriginalPosition ();
		}
		actualPagePosition--;
		var position = container.transform.localPosition.x + 1451.8f;
		iTween.MoveTo (container, iTween.Hash ("x", position, "islocal", true, "time", 0.7, "easetype", iTween.EaseType.easeInOutBack));
	}

	public void GoCalendarsToOriginalPosition ()
	{
		var myOffset = 0;
		myOffset = actualPositionIndex * positionXOffset;
		var position = calendars.transform.localPosition.x + myOffset;
		iTween.MoveTo (calendars, iTween.Hash ("x", position, "islocal", true, "time", 0.7, "easetype", iTween.EaseType.easeInOutBack));
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

	void ReadjustScrollSize (int size)
	{
		ResponsibleScrollContentList.anchorMax = new Vector2 (1, 1);
		ResponsibleScrollContentList.anchorMin = new Vector2 (0, 1);

		ResponsibleScrollContentList.offsetMax = new Vector2 (0, 0);
		var number = (((RectTransform)ResponsibleCellPrefab).rect.height * (size + 1));

		ResponsibleScrollContentList.offsetMin = new Vector2 (0, -number);
	}
		
}

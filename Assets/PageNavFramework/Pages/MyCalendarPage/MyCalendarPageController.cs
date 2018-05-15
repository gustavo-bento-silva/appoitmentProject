using UnityEngine;
using System.Collections;
using PageNavFrameWork;
using System.Collections.Generic;
using System;

public class MyCalendarPageController : PageController
{

	public GameObject container;
	public GameObject calendars;
	public CalendarController[] calendarsController;

	public Transform ResponsibleCellPrefab;
	public Transform ResponsibleManagerCellPrefab;
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
	private bool isFromCompanySelectPage = false;

	public override void InstantiatedWithArgs (Dictionary<string, object> args)
	{
		isFromCompanySelectPage = (bool)args ["isFromCompanySelectPage"];
	}

	void Start ()
	{
		Loading = true;
		Delegates.GetAllResponsibles getAllResponsibles = GetEmployerList;
		DataManager.GetAllResponsablesFromCompany (getAllResponsibles);
	}

	void GetEmployerList (List<ResponsibleModel> responsiblesList)
	{
		DataManager.responsibles = responsiblesList;
		if (isFromCompanySelectPage) {
			GetAllServices ();
		} else {
			responsibles = responsiblesList;
			FillEmployerList ();
		}
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
		responsibles.Clear ();
		responsibles = mResponsibles;
		FillEmployerList ();
	}

	void FillEmployerList ()
	{
		var index = 0;
		responsibles.ForEach (x => {
			GameObject go;
			if (isFromCompanySelectPage) {
				go = SelectResponsibleCellController.Instantiate (ResponsibleCellPrefab, x, index, delegate(ResponsibleModel responsible, int mindex) {
					OnResponsibleSelected (responsible, mindex);
				});
			} else {
				go = ResponsibleManagerCellController.Instantiate (ResponsibleManagerCellPrefab, x, index, delegate(ResponsibleModel responsible, int mindex) {
					OnResponsibleSelected (responsible, mindex);
				});
			}
			responsibleCell.Add (go);
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
		if (servicesCell.Count > 0) {
			servicesCell.ForEach (x => GameObject.Destroy (x));
		}
		servicesCell.Clear ();
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
		if (isFromCompanySelectPage) {
			services.Clear ();
			foreach (var key in responsible.servicesProvided.Keys) {
				services.Add ((ServicesProvidedModel)responsible.servicesProvided [key]);
			}
			FillServiceList ();
		}
		FillCalendars ();
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
		iTween.MoveTo (calendars, iTween.Hash ("x", 0, "islocal", true, "time", 0.5, "easetype", iTween.EaseType.easeInOutBack));
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
		var number = isFromCompanySelectPage ? (((RectTransform)ResponsibleCellPrefab).rect.height * (size + 1)) : (((RectTransform)ResponsibleManagerCellPrefab).rect.height * (size + 1));

		ResponsibleScrollContentList.offsetMin = new Vector2 (0, -number);
	}
}

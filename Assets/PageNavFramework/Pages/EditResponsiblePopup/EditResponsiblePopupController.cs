using UnityEngine;
using System.Collections;
using PageNavFrameWork;
using System.Collections.Generic;
using UnityEngine.UI;

public class EditResponsiblePopupController : PageController
{

	public Transform cellPrefab;
	public RectTransform scrollContentList;

	ResponsibleModel responsible;
	Delegates.OnSelectServiceClick selectServiceDelegate;
	List <GameObject> servicesProvidedCell = new List<GameObject> ();
	List<ServicesProvidedModel> servicesProvidedList = new List<ServicesProvidedModel> ();

	List<bool> servicesProvidedByResponsible = new List<bool> ();

	void Start ()
	{
		OnInitServicesWindow ();
	}

	void Update ()
	{

	}

	public void OnClickUpdateButton ()
	{
		Loading = true;
		DataManager.UpdateResponsibleServices (responsible, GetServices (), delegate() {
			Success = true;
			Loading = false;
		}, delegate(string error) {
			Loading = false;
			Error = true;
			CloseModal ();
			Constants.LoadHomePage ();
		});
	}

	void OnInitServicesWindow ()
	{
		Loading = true;
		selectServiceDelegate = HandleOnSelectServiceClick;
		if ((DataManager.currentUser as CompanyModel).servicesProvided != null) {
			foreach (var key in (DataManager.currentUser as CompanyModel).servicesProvided.Keys) {
				servicesProvidedList.Add ((ServicesProvidedModel)(DataManager.currentUser as CompanyModel).servicesProvided [key]);
			}
		}
		if (servicesProvidedList != null && servicesProvidedList.Count != 0) {
			FillServicesList ();
		} else {
			Loading = false;
		}
	}

	public List<ServicesProvidedModel> GetServices ()
	{
		int index = 0;
		List<ServicesProvidedModel> services = new List<ServicesProvidedModel> ();
		servicesProvidedByResponsible.ForEach (x => {
			if (x) {
				services.Add (servicesProvidedList [index]);
				index++;
			}
		});
		return services;
	}

	void HandleOnSelectServiceClick (ServicesProvidedModel serviceprovided, bool status)
	{
		var index = servicesProvidedList.IndexOf (serviceprovided);
		if (servicesProvidedByResponsible.Count > index) {
			servicesProvidedByResponsible [index] = status;
		}
	}

	void FillServicesList ()
	{
		servicesProvidedList.ForEach (x => {
			if (responsible.servicesProvided.ContainsKey (x.serviceID)) {
				var cell = ServicesProvidedCell.Instantiate (cellPrefab, x, selectServiceDelegate, true);
				servicesProvidedCell.Add (cell);
				servicesProvidedByResponsible.Add (true);
			} else {
				var cell = ServicesProvidedCell.Instantiate (cellPrefab, x, selectServiceDelegate);
				servicesProvidedCell.Add (cell);
				servicesProvidedByResponsible.Add (false);
			}
		});
		StartCoroutine (OnFillServicesList ());
	}

	IEnumerator OnFillServicesList ()
	{
		yield return new WaitForSeconds (1f);
		servicesProvidedCell.ForEach (x => x.transform.SetParent (scrollContentList, false));
		ReadjustScrollSize (servicesProvidedCell.Count);
		Loading = false;
	}

	void ReadjustScrollSize (int size)
	{
		scrollContentList.anchorMax = new Vector2 (1, 1);
		scrollContentList.anchorMin = new Vector2 (0, 1);

		scrollContentList.offsetMax = new Vector2 (0, 0);
		var number = (((RectTransform)cellPrefab).rect.height * (size + 1));

		scrollContentList.offsetMin = new Vector2 (0, -number);
	}

	public override void InstantiatedWithArgs (Dictionary<string,object> args)
	{
		foreach (var key in args.Keys) {
			responsible = (ResponsibleModel)args [key];
		}
	}
}

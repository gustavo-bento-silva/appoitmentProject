using UnityEngine;
using System.Collections;
using PageNavFrameWork;
using System.Collections.Generic;
using UnityEngine.UI;

public class CreateNewResponsibleToCompanyController : PageController
{
	public Text email;
	public Text password;
	public Text name;

	public GameObject container;

	public Transform cellPrefab;
	public RectTransform scrollContentList;
	public GameObject nullListMessage;

	public List<Toggle> daysWorked;
	public List<Text> initTimeToWork;
	public List<Text> endTimeToWor;

	private int actualPositionIndex = 0;
	private int positionXOffset = 1315;

	Delegates.OnSelectServiceClick selectServiceDelegate;
	List <GameObject> servicesProvidedCell = new List<GameObject> ();
	List<ServicesProvidedModel> servicesProvidedList = new List<ServicesProvidedModel> ();

	List<bool> servicesProvidedByResponsible = new List<bool> ();

	void Start ()
	{
	}

	void OnInitServicesWindow ()
	{
		selectServiceDelegate += HandleOnSelectServiceClick;
		Loading = true;
		if ((DataManager.currentUser as CompanyModel).servicesProvided != null) {
			foreach (var key in (DataManager.currentUser as CompanyModel).servicesProvided.Keys) {
				servicesProvidedList.Add ((ServicesProvidedModel)(DataManager.currentUser as CompanyModel).servicesProvided [key]);
			}
		}
		if (servicesProvidedList != null && servicesProvidedList.Count != 0) {
			FillServicesList ();
		} else {
			nullListMessage.SetActive (true);
		}
	}

	void HandleOnSelectServiceClick (ServicesProvidedModel serviceprovided, bool status)
	{
		servicesProvidedByResponsible [servicesProvidedList.IndexOf (serviceprovided)] = status;
	}

	void OnInitTimeToWorkScreen ()
	{
//		initTimeToWork = (DataManager.currentUser as CompanyModel).timeToBeginWork;
	}

	void OnDeleteUserClicked ()
	{
		//		PageNav.GetPageNavInstance().PushPageToStackWithArgs(PagesEnum.ConfirmDeleteUserPopUp)
	}

	void FillServicesList ()
	{
		servicesProvidedList.ForEach (x => {
			servicesProvidedCell.Add (ServicesProvidedCell.Instantiate (cellPrefab, x, selectServiceDelegate));
			servicesProvidedByResponsible.Add (false);
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

	public void OnNextButtonClick ()
	{
		actualPositionIndex++;
		var position = container.transform.localPosition.x - positionXOffset;
		iTween.MoveTo (container, iTween.Hash ("x", position, "islocal", true, "time", 0.7, "easetype", iTween.EaseType.easeInBack));
	}

	public void OnBackButtonClick ()
	{
		actualPositionIndex--;
		var position = container.transform.localPosition.x + positionXOffset;
		iTween.MoveTo (container, iTween.Hash ("x", position, "islocal", true, "time", 0.7, "easetype", iTween.EaseType.easeInBack));

	}
}

using UnityEngine;
using System.Collections;
using PageNavFrameWork;
using System.Collections.Generic;
using UnityEngine.UI;

public class CreateNewResponsibleToCompanyController : PageController
{
	public Text email;
	public InputField password;
	public Text name;

	public InputField initLunchTime;
	public InputField endLunchTime;

	public GameObject nameError;
	public GameObject emailError;
	public GameObject passwordError;
	public GameObject serviceError;
	public GameObject daysError;
	public GameObject lunchTimeError;

	public GameObject container;

	public Transform cellPrefab;
	public RectTransform scrollContentList;
	public GameObject nullListMessage;

	public List<Toggle> daysWorked;
	public List<Text> initTimeToWork;
	public List<Text> endTimeToWork;

	private int actualPositionIndex = 0;
	private int positionXOffset = 1315;

	Delegates.OnSelectServiceClick selectServiceDelegate;
	List <GameObject> servicesProvidedCell = new List<GameObject> ();
	List<ServicesProvidedModel> servicesProvidedList = new List<ServicesProvidedModel> ();

	List<bool> servicesProvidedByResponsible = new List<bool> ();

	void Start ()
	{
		OnDaysWorkedInit ();
		OnInitTimeToWorkScreen ();
		OnFinishTimeToWorkScreen ();
		OnInitServicesWindow ();

	}

	void OnInitServicesWindow ()
	{
		selectServiceDelegate += HandleOnSelectServiceClick;
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

	void OnDaysWorkedInit ()
	{
		var index = 0;
		(DataManager.currentUser as CompanyModel).daysOfWork.ForEach (x => {
			daysWorked [index++].isOn = x;
		});
	}

	void OnInitTimeToWorkScreen ()
	{
		int index = 0;
		(DataManager.currentUser as CompanyModel).timeToBeginWork.ForEach (x => {
			initTimeToWork [index++].text = x.ToString ();
		});
	}

	void OnFinishTimeToWorkScreen ()
	{
		int index = 0;
		(DataManager.currentUser as CompanyModel).timeToFinishWork.ForEach (x => {
			endTimeToWork [index++].text = x.ToString ();
		});
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
	}

	void CreateUserLogin ()
	{
		Loading = true;
		FirebaseAPIHelper.GetFireBaseAPIHelperInstance ().AddUser (email.text, password.text, delegate(string userID) {
			CreateNewResponsibleToCompany (userID);
		}, delegate(string error) {
			Error = true;
		});
	}

	void CreateNewResponsibleToCompany (string userID)
	{
		DataManager.CreateNewResponsibleToCompanyAsUser (userID, name.text, GetServices (), GetDaysWorked (), GetInitTime (), GetEndTime (), int.Parse (initLunchTime.text), int.Parse (endLunchTime.text));
		Loading = false;
		Constants.LoadHomePage ();
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
		bool everythingOk = true;
		if (actualPositionIndex == 0) {
			everythingOk = FirstStepVerify ();
		} else if (actualPositionIndex == 1) {
			everythingOk = SecondStepVerify ();
		} else if (actualPositionIndex == 2) {
			everythingOk = ThirdStepVerify ();
		} else if (actualPositionIndex == 2) {
			everythingOk = FourthStepVerify ();
		}

		if (everythingOk) {
			if (actualPositionIndex == 3) {
				CreateUserLogin ();
			} else {
				actualPositionIndex++;
				var position = container.transform.localPosition.x - positionXOffset;
				iTween.MoveTo (container, iTween.Hash ("x", position, "islocal", true, "time", 0.7, "easetype", iTween.EaseType.easeInBack));	
			}
		}
	}

	public void OnBackButtonClick ()
	{
		actualPositionIndex--;
		var position = container.transform.localPosition.x + positionXOffset;
		iTween.MoveTo (container, iTween.Hash ("x", position, "islocal", true, "time", 0.7, "easetype", iTween.EaseType.easeInBack));

	}

	public bool FirstStepVerify ()
	{
		if (string.IsNullOrEmpty (name.text)) {
			nameError.SetActive (true);
			return false;
		} else {
			nameError.SetActive (false);
		}
		if (string.IsNullOrEmpty (email.text) || !email.text.Contains ("@")) {
			emailError.SetActive (true);
			return false;
		} else {
			emailError.SetActive (false);
		}
		if (password.text.Length < 6) {
			passwordError.SetActive (true);
			return false;
		} else {
			passwordError.SetActive (false);
		}
		return true;
	}

	public bool SecondStepVerify ()
	{
		int isOn = 0;
		if (servicesProvidedByResponsible == null || servicesProvidedByResponsible.Count == 0) {
			serviceError.SetActive (true);
			return false;
		}
		servicesProvidedByResponsible.ForEach (x => {
			if (x)
				isOn++;
		});
		if (isOn > 0) {
			serviceError.SetActive (false);
			return true;
		} 
		serviceError.SetActive (true);
		return false;
	}

	public bool ThirdStepVerify ()
	{
		int isOn = 0;
		if (daysWorked == null || daysWorked.Count == 0) {
			daysError.SetActive (true);
			return false;
		}
		daysWorked.ForEach (x => {
			if (x.isOn)
				isOn++;
		});
		if (isOn > 0) {
			daysError.SetActive (false);
			return true;
		} 
		daysError.SetActive (true);
		return false;
	}

	public bool FourthStepVerify ()
	{
		if (int.Parse (initLunchTime.text) <= int.Parse (endLunchTime.text)) {
			lunchTimeError.SetActive (false);
			return true;
		} else {
			lunchTimeError.SetActive (true);
			return false;
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

	public List<bool> GetDaysWorked ()
	{
		List<bool> mDaysWorked = new List<bool> ();
		daysWorked.ForEach (x => {
			if (x.isOn) {
				mDaysWorked.Add (true);
			} else {
				mDaysWorked.Add (false);
			}
		});
		return mDaysWorked;
	}

	public List<int> GetInitTime ()
	{
		List<int> initTimeToWorkList = new List<int> ();
		initTimeToWork.ForEach (x => initTimeToWorkList.Add (int.Parse (x.text)));
		return initTimeToWorkList;
	}

	public List<int> GetEndTime ()
	{
		List<int> endTimeToWorkList = new List<int> ();
		endTimeToWork.ForEach (x => endTimeToWorkList.Add (int.Parse (x.text)));
		return endTimeToWorkList;
	}
}

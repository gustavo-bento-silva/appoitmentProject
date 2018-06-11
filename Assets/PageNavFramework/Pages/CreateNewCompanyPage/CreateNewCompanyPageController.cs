using UnityEngine;
using System.Collections;
using PageNavFrameWork;
using UnityEngine.UI;

public class CreateNewCompanyPageController : PageController
{
	public GameObject container;
	public InputField[] initTime;
	public InputField[] endTime;
	public Toggle[] daysWorked;

	public InputField email;
	public InputField password;
	public InputField city;
	public InputField address;
	public InputField cep;
	public InputField companyName;
	public InputField phone;

	public float offset;

	void Start ()
	{

	}

	void Update ()
	{

	}

	public void OnButtonClick ()
	{
		CreateNewCompany ();
	}

	public void ShowCompanyData ()
	{
		iTween.MoveTo (container, iTween.Hash ("x", container.transform.localPosition.x + offset, "islocal", true, "time", 0.7, "easetype", iTween.EaseType.easeInOutBack));
	}

	public void ShowTimeToWork ()
	{
		iTween.MoveTo (container, iTween.Hash ("x", container.transform.localPosition.x - offset, "islocal", true, "time", 0.7, "easetype", iTween.EaseType.easeInOutBack));
	}

	void CreateNewCompany ()
	{
		FirebaseAuth.GetFireBaseAuthInstance ().CreateNewCompanyWithEmailAndPassword (companyName.text, email.text, password.text, delegate (string userID) {
			CloseModal ();
			Loading = false;
			Success = true;
			CreateNewCompanyDataBase (userID);
		}, delegate(string error) {
			Loading = false;
			OpenErrorPopup (error);
		});
	}

	void CreateNewCompanyDataBase (string userID)
	{
		DataManager.CreateCompanyData (userID, companyName.text, phone.text, city.text, address.text, cep.text, GetInitialTime (), GetFinishTime (), GetDaysWorked ());
	}

	bool[] GetDaysWorked ()
	{
		var days = new bool[daysWorked.Length];
		int index = 0;
		foreach (var day in daysWorked) {
			days [index] = day.isOn;
			index++;
		}
		return days;
	}

	int[] GetInitialTime ()
	{
		var mInitTime = new int[initTime.Length];
		int index = 0;
		foreach (var time in initTime) {
			mInitTime [index] = int.Parse (time.text);
			index++;
		}
		return mInitTime;
	}

	int[] GetFinishTime ()
	{
		var mFinishTime = new int[endTime.Length];
		int index = 0;
		foreach (var time in endTime) {
			mFinishTime [index] = int.Parse (time.text);
			index++;
		}
		return mFinishTime;
	}
}

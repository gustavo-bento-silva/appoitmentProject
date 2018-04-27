using UnityEngine;
using System.Collections;
using PageNavFrameWork;
using UnityEngine.UI;

public class CreateNewCompanyPageController : PageController
{
	
	public string homeScene;
	public GameObject email;
	public GameObject password;
	public GameObject city;
	public GameObject address;
	public GameObject cep;
	public GameObject name;
	public GameObject phone;

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

	void CreateNewCompany ()
	{
		FirebaseAuth.GetFireBaseAuthInstance ().CreateNewCompanyWithEmailAndPassword (name.GetComponent<InputField> ().text, email.GetComponent<InputField> ().text, password.GetComponent<InputField> ().text, delegate (string userID) {
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
		DataManager.CreateCompanyDataWithMockData (userID);
	}
}

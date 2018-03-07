using UnityEngine;
using System.Collections;
using PageNavFrameWork;
using System.Collections.Generic;
using UnityEngine.UI;

public class SelectCompanyController : PageController
{
	public Dropdown dropDown;
	public Text description;

	void Start ()
	{
		Loading = true;
		GetAllCompanies ();
	}

	void Update ()
	{

	}

	void GetAllCompanies ()
	{
		FireBaseManager.GetFireBaseInstance ().GetAllCompanies (delegate(List<CompanyModel> companies) {
			DataManager.companiesList = companies;
			List<string> companiesName = new List<string> ();
			companies.ForEach ((x) => companiesName.Add (x.name));
			dropDown.ClearOptions ();
			dropDown.AddOptions (companiesName);
			OnCompanySelected ();
			Loading = false;
		}, delegate (string error) {
			Loading = false;
			Error = true;
		});
	}

	void UpdateCompanyDescription (CompanyModel company)
	{
		description.text = string.Format ("{0}\nTelefone: {1}\n{2} - {3}", company.name, company.phone, company.address, company.city);
		description.gameObject.SetActive (true);
	}

	public void OnCompanySelected ()
	{
		Debug.Log (dropDown.value);
		UpdateCompanyDescription (DataManager.companiesList [dropDown.value]);
		DataManager.companyData = DataManager.companiesList [dropDown.value];
		
	}
}

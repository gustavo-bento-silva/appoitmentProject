using UnityEngine;
using System.Collections;
using PageNavFrameWork;
using System.Collections.Generic;
using UnityEngine.UI;

public class SelectCompanyController : PageController
{

	public Transform cellPrefab;
	public RectTransform scrollContentList;
	public GameObject nullListMessage;

	public Dropdown dropDown;
	public Text description;

	Color selectedColor = new Color (27f / 255.0f, 184f / 255.0f, 157f / 255.0f);

	List <GameObject> companyCell = new List<GameObject> ();

	void Start ()
	{
		CheckCompanies ();
	}

	void Update ()
	{

	}

	void CheckCompanies ()
	{
		Loading = true;
		FireBaseManager.GetFireBaseInstance ().GetAllCompanies (delegate(List<CompanyModel> companies) {
			DataManager.companiesList = companies;
			FillList ();
		}, delegate (string error) {
			Loading = false;
			Error = true;
		});
	}

	void FillList ()
	{
		var index = 0;
		DataManager.companiesList.ForEach (x => {
			companyCell.Add (CompanyCell.Instantiate (cellPrefab, x, index, delegate(CompanyModel company, int mindex) {
				OnCompanySelected (company, mindex);
			}));
			index++;
		});
		StartCoroutine (OnFillList ());
	}

	IEnumerator OnFillList ()
	{
		yield return new WaitForSeconds (1f);
		companyCell.ForEach (x => x.transform.SetParent (scrollContentList, false));
		ReadjustScrollSize (companyCell.Count);
		Loading = false;
	}

	void ChangeSelectCompanyColor (int index)
	{
		var mindex = 0;
		companyCell.ForEach (x => {
			if (index == mindex) {
				x.GetComponent<Image> ().color = selectedColor;
			} else {
				x.GetComponent<Image> ().color = Color.white;
			}
			mindex++;
		});
	}

	void OnCompanySelected (CompanyModel company, int index)
	{
		DataManager.companyData = company;
		ChangeSelectCompanyColor (index);
		UpdateCompanyDescription (company);
	}

	void ReadjustScrollSize (int size)
	{
		scrollContentList.anchorMax = new Vector2 (1, 1);
		scrollContentList.anchorMin = new Vector2 (0, 1);

		scrollContentList.offsetMax = new Vector2 (0, 0);
		var number = (((RectTransform)cellPrefab).rect.height * (size + 1));

		scrollContentList.offsetMin = new Vector2 (0, -number);
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

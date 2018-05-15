using UnityEngine;
using System.Collections;
using PageNavFrameWork;
using System.Collections.Generic;
using UnityEngine.UI;

public class SelectCompanyController : PageController
{
	public InputField search;
	public Transform cellPrefab;
	public RectTransform scrollContentList;
	public GameObject nullListMessage;

	public Dropdown dropDown;
	public Text description;

	Color selectedColor = new Color (213f / 255.0f, 204f / 255.0f, 84f / 255.0f);

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
		var dict = new Dictionary<string, object> ();
		dict.Add ("isFromCompanySelectPage", (object)true);
		PageNav.GetPageNavInstance ().PushPageToStackWithArgs (PagesEnum.CalendarPage, dict);
	}

	void ReadjustScrollSize (int size)
	{
		scrollContentList.anchorMax = new Vector2 (1, 1);
		scrollContentList.anchorMin = new Vector2 (0, 1);

		scrollContentList.offsetMax = new Vector2 (0, 0);
		var number = (((RectTransform)cellPrefab).rect.height * (size + 1));

		scrollContentList.offsetMin = new Vector2 (0, -number);
	}

	void UpdateCompanyDescription (CompanyModel company)
	{
		description.text = string.Format ("{0}\nTelefone: {1}\n{2} - {3}", company.name, company.phone, company.address, company.city);
		description.gameObject.SetActive (true);
	}

	public void Search ()
	{
		if (!string.IsNullOrEmpty (search.text)) {
			if (companyCell != null && companyCell.Count > 0) {
				companyCell.ForEach (x => {
					if (!x.GetComponent<CompanyCell> ().companyName.text.ToLower ().Contains (search.text.ToLower ())) {
						x.SetActive (false);
					} else {
						x.SetActive (true);
					}
				});
			}
		} else {
			if (companyCell != null && companyCell.Count > 0) {
				companyCell.ForEach (x => {
					x.SetActive (true);
				});
			}
		}
	}
}

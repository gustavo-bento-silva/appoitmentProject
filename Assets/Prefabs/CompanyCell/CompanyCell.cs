using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CompanyCell : MonoBehaviour
{
	
	public Text companyName;
	public Text companyAddress;
	public GameObject placeHolder;
	public Image companyImage;
	public CompanyModel companyModel;
	public int index;
	public Delegates.OnSelectCompanyClick onSelectedCompanyclick;

	public void OnCompanyClick ()
	{
		onSelectedCompanyclick (companyModel, index);
	}

	public void LoadCompanyImage (string companyID)
	{
		FirebaseStorageManager.GetFireBaseInstance ().LoadImage (companyID, delegate(Sprite sprite) {
			placeHolder.SetActive (false);
			companyImage.gameObject.SetActive (true);
			companyImage.sprite = sprite;
		});
	}

	public static GameObject Instantiate (Transform CellPrefabTransform, CompanyModel company, int index, Delegates.OnSelectCompanyClick onSelectedCompanyClick)
	{
		GameObject go = GameObject.Instantiate (CellPrefabTransform).gameObject;
		var myCompanyController = go.GetComponent<CompanyCell> ();
		myCompanyController.LoadCompanyImage (company.userID);
		myCompanyController.companyName.text = company.name;
		myCompanyController.companyAddress.text = company.address;
		myCompanyController.companyModel = company;
		myCompanyController.index = index;
		myCompanyController.onSelectedCompanyclick = onSelectedCompanyClick;
		return go;
	}
}

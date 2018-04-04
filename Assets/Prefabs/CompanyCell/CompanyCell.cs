using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CompanyCell : MonoBehaviour
{
	
	public Text companyName;
	public Text companyAddress;
	public CompanyModel companyModel;
	public int index;
	public Delegates.OnSelectCompanyClick onSelectedCompanyclick;

	public void OnCompanyClick ()
	{
		onSelectedCompanyclick (companyModel, index);
	}

	public static GameObject Instantiate (Transform CellPrefabTransform, CompanyModel company, int index, Delegates.OnSelectCompanyClick onSelectedCompanyClick)
	{
		GameObject go = GameObject.Instantiate (CellPrefabTransform).gameObject;
		var myCompanyController = go.GetComponent<CompanyCell> ();
		myCompanyController.companyName.text = company.name;
		myCompanyController.companyAddress.text = company.address;
		myCompanyController.companyModel = company;
		myCompanyController.index = index;
		myCompanyController.onSelectedCompanyclick = onSelectedCompanyClick;
		return go;
	}
}

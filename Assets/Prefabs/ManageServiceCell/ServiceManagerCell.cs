using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ServiceManagerCell : MonoBehaviour
{
	public Text service;
	public ServicesProvidedModel serviceModel;


	public void OnRemoveClick ()
	{
		DataManager.RemoveServiceFromCompanyAsUser (serviceModel.serviceID);
	}

	public static GameObject Instantiate (Transform CellPrefabTransform, ServicesProvidedModel serviceModel)
	{
		GameObject go = GameObject.Instantiate (CellPrefabTransform).gameObject;
		var myMessageController = go.GetComponent<ServiceManagerCell> ();
		myMessageController.service.text = serviceModel.name;
		myMessageController.serviceModel = serviceModel;
		return go;
	}
}

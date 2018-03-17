using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ServicesProvidedCell : MonoBehaviour
{

	public Text serviceName;
	public ServicesProvidedModel serviceModel;
	public Delegates.OnSelectServiceClick serviceCallback;

	public void OnServiceClick ()
	{
		var toogle = GetComponentInChildren<Toggle> ();
		serviceCallback (serviceModel, toogle.isOn);
	}

	public static GameObject Instantiate (Transform CellPrefabTransform, ServicesProvidedModel user, Delegates.OnSelectServiceClick serviceClickCallback)
	{
		GameObject go = GameObject.Instantiate (CellPrefabTransform).gameObject;
		var myServiceProvidedCellController = go.GetComponent<ServicesProvidedCell> ();
		myServiceProvidedCellController.serviceName.text = user.name;
		myServiceProvidedCellController.serviceModel = user;
		myServiceProvidedCellController.serviceCallback = serviceClickCallback;
		return go;
	}
}

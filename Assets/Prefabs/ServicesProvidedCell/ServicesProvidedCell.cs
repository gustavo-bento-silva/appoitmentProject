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

	public static GameObject Instantiate (Transform CellPrefabTransform, ServicesProvidedModel service, Delegates.OnSelectServiceClick serviceClickCallback, bool isOn = false)
	{
		GameObject go = GameObject.Instantiate (CellPrefabTransform).gameObject;
		var myServiceProvidedCellController = go.GetComponent<ServicesProvidedCell> ();
		var servicePrice = float.Parse (service.price) % 1;
		string text = string.Format ("{0} - R${1},{2}", service.name, Mathf.Floor (float.Parse (service.price)), servicePrice.ToString ("00"));
		myServiceProvidedCellController.serviceName.text = text;
		myServiceProvidedCellController.serviceModel = service;
		myServiceProvidedCellController.serviceCallback = serviceClickCallback;
		if (isOn) {
			go.GetComponentInChildren<Toggle> ().isOn = true;
		}
		return go;
	}
}

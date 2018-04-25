﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PageNavFrameWork;

public class ServiceManagerCell : MonoBehaviour
{
	public Text service;
	public ServicesProvidedModel mServiceModel;


	public void OnRemoveClick ()
	{
		DataManager.RemoveServiceFromCompanyAsUser (mServiceModel.serviceID);
	}

	public void OnEditClick ()
	{
		var dict = new Dictionary<string, object> ();
		dict.Add (mServiceModel.serviceID, mServiceModel);
		PageNav.GetPageNavInstance ().PushPageToStackWithArgs (PagesEnum.EditServicePopup, dict);
	}

	public static GameObject Instantiate (Transform CellPrefabTransform, ServicesProvidedModel serviceModel)
	{
		GameObject go = GameObject.Instantiate (CellPrefabTransform).gameObject;
		var myMessageController = go.GetComponent<ServiceManagerCell> ();
		string text = "";
		if (!string.IsNullOrEmpty (serviceModel.price)) {
			var servicePrice = float.Parse (serviceModel.price) % 1;
			text = string.Format ("{0} - R${1},{2} \n Duração: {3}h", serviceModel.name, Mathf.Floor (float.Parse (serviceModel.price)), servicePrice.ToString ("00"), serviceModel.duration);
		}
		myMessageController.service.text = text;
		myMessageController.mServiceModel = serviceModel;
		return go;
	}
}

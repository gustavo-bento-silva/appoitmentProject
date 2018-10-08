using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PageNavFrameWork;

public class ServiceManagerCell : MonoBehaviour
{
	public Text service;
	public ServicesProvidedModel mServiceModel;


	public void OnRemoveClick()
	{
		PageNav.GetPageNavInstance().SetLoadingVisibility(true);
		DataManager.RemoveServiceFromCompanyAsUser(mServiceModel.serviceID, delegate ()
		{
			PageNav.GetPageNavInstance().SetLoadingVisibility(false);
			Constants.LoadHomePage();
		}, delegate (string error)
		{
			PageNav.GetPageNavInstance().SetLoadingVisibility(false);
			PageNav.GetPageNavInstance().SetErrorVisibility(true);
		});
	}

	public void OnEditClick()
	{
		var dict = new Dictionary<string, object>();
		dict.Add(mServiceModel.serviceID, mServiceModel);
		PageNav.GetPageNavInstance().PushPageToStackWithArgs(PagesEnum.EditServicePopup, dict);
	}

	public static GameObject Instantiate(Transform CellPrefabTransform, ServicesProvidedModel serviceModel)
	{
		GameObject go = GameObject.Instantiate(CellPrefabTransform).gameObject;
		var myMessageController = go.GetComponent<ServiceManagerCell>();
		string text = "";
		if (!string.IsNullOrEmpty(serviceModel.price))
		{
			var servicePrice = Mathf.Round((float.Parse(serviceModel.price) % 1) * 100);
			text = string.Format("{0} - R${1},{2} \n Duração: {3}h", serviceModel.name, Mathf.Floor(float.Parse(serviceModel.price)), servicePrice.ToString("00"), serviceModel.duration);
		}
		else
		{
			text = serviceModel.name;
		}
		myMessageController.service.text = text;
		myMessageController.mServiceModel = serviceModel;
		return go;
	}
}

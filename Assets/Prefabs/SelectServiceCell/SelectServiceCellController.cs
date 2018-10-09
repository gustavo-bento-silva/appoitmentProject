using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectServiceCellController : MonoBehaviour
{

	public Text serviceName;
	public ServicesProvidedModel serviceModel;
	public int index;
	public Delegates.OnSelectServiceFromResponsibleClick onSelectedServiceclick;

	public void OnServiceClick()
	{
		onSelectedServiceclick(serviceModel, index);
	}

	public static GameObject Instantiate(Transform CellPrefabTransform, ServicesProvidedModel service, int index, Delegates.OnSelectServiceFromResponsibleClick mOnSelectServiceClick)
	{
		GameObject go = GameObject.Instantiate(CellPrefabTransform).gameObject;
		var myServiceCellController = go.GetComponent<SelectServiceCellController>();
		if (service.price != null)
		{
			if (!string.IsNullOrEmpty(service.price))
			{
				var servicePrice = float.Parse(service.price) % 1;
				myServiceCellController.serviceName.text = string.Format("{0} - R${1},{2}", service.name, Mathf.Floor(float.Parse(service.price)), servicePrice.ToString("00"));
			}
			else
			{
				myServiceCellController.serviceName.text = service.name;
			}
		}
		else
		{
			myServiceCellController.serviceName.text = service.name;
		}
		myServiceCellController.onSelectedServiceclick = mOnSelectServiceClick;
		myServiceCellController.index = index;
		myServiceCellController.serviceModel = service;
		return go;
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectResponsibleCellController : MonoBehaviour
{
	public Text userName;
	public Text functions;
	public ResponsibleModel userModel;
	public int index;
	public Delegates.OnSelectResponsibleClick onSelectedResponsibleclick;

	public void OnResponsibleClick ()
	{
		onSelectedResponsibleclick (userModel, index);
	}

	public static GameObject Instantiate (Transform CellPrefabTransform, ResponsibleModel user, int index, Delegates.OnSelectResponsibleClick mOnSelectResponsibleClick)
	{
		GameObject go = GameObject.Instantiate (CellPrefabTransform).gameObject;
		var myResponsibleCellController = go.GetComponent<SelectResponsibleCellController> ();
		myResponsibleCellController.userName.text = user.name;
		myResponsibleCellController.onSelectedResponsibleclick = mOnSelectResponsibleClick;
		if (user.servicesProvided != null) {
			foreach (var key in user.servicesProvided.Keys) {
				myResponsibleCellController.functions.text += (user.servicesProvided [key] as ServicesProvidedModel).name;
				myResponsibleCellController.functions.text += "\n";
			}
		}
		myResponsibleCellController.index = index;
		myResponsibleCellController.userModel = user;
		return go;
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PageNavFrameWork;

public class ResponsibleManagerCellController : MonoBehaviour
{
	public Text userName;
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
		var myResponsibleCellController = go.GetComponent<ResponsibleManagerCellController> ();
		myResponsibleCellController.userName.text = user.name;
		myResponsibleCellController.onSelectedResponsibleclick = mOnSelectResponsibleClick;
		myResponsibleCellController.index = index;
		myResponsibleCellController.userModel = user;
		return go;
	}
}

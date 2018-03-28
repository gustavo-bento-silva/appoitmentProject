using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClientCellController : MonoBehaviour
{

	public Text userName;
	public Text phone;
	public UserModel userModel;


	public void OnSelectClick ()
	{
		
	}

	public static GameObject Instantiate (Transform CellPrefabTransform, UserModel user)
	{
		GameObject go = GameObject.Instantiate (CellPrefabTransform).gameObject;
		var myResponsibleCellController = go.GetComponent<ClientCellController> ();
		myResponsibleCellController.userName.text = user.name;
		myResponsibleCellController.phone.text = "Telefone: " + user.phone;
		myResponsibleCellController.userModel = user;
		return go;
	}

}

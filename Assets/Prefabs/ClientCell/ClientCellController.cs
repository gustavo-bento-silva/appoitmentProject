using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClientCellController : MonoBehaviour
{

	public Text userName;
	public Text phone;
	public UserModel userModel;
	public Delegates.OnSelectClientClick selectClientClick;

	public void OnDeleteClick ()
	{
		PageNavFrameWork.PageNav.GetPageNavInstance ().SetLoadingVisibility (true);
		DataManager.DeleteClient (userModel, delegate() {
			GameObject.Destroy (gameObject);
			PageNavFrameWork.PageNav.GetPageNavInstance ().SetLoadingVisibility (false);
		}, delegate(string error) {
			PageNavFrameWork.PageNav.GetPageNavInstance ().SetErrorVisibility (true);
			PageNavFrameWork.PageNav.GetPageNavInstance ().SetLoadingVisibility (false);
		});
	}

	public void OnSelectClick ()
	{
		selectClientClick (userModel);
	}

	public static GameObject Instantiate (Transform CellPrefabTransform, UserModel user, Delegates.OnSelectClientClick onSelectClientClick)
	{
		GameObject go = GameObject.Instantiate (CellPrefabTransform).gameObject;
		var myResponsibleCellController = go.GetComponent<ClientCellController> ();
		myResponsibleCellController.userName.text = user.name;
		myResponsibleCellController.selectClientClick = onSelectClientClick;
		myResponsibleCellController.phone.text = "Telefone: " + user.phone;
		myResponsibleCellController.userModel = user;
		return go;
	}

}

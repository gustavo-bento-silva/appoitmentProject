using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PageNavFrameWork;

public class ResponsibleCellControler : MonoBehaviour
{
	public Text userName;
	public UserModel userModel;


	public void OnRemoveClick ()
	{
		var dict = new Dictionary<string, object> ();
		dict.Add (userModel.userID, userModel);
		PageNav.GetPageNavInstance ().PushPageToStackWithArgs (PagesEnum.ConfirmDeleteUserPopUp, dict);
//		PageNav.GetPageNavInstance ().PushPageToStackWithArgs (PagesEnum.ConfirmDeleteUserPopUp);
//		PageNavFrameWork.PageNav.GetPageNavInstance ().SetLoadingVisibility (true);
//		DataManager.RemoveMessage (userModel, delegate {
//			GameObject.Destroy (gameObject);
//			PageNavFrameWork.PageNav.GetPageNavInstance ().SetLoadingVisibility (false);
//		});
	}

	public static GameObject Instantiate (Transform CellPrefabTransform, UserModel user)
	{
		GameObject go = GameObject.Instantiate (CellPrefabTransform).gameObject;
		var myResponsibleCellController = go.GetComponent<ResponsibleCellControler> ();
		myResponsibleCellController.userName.text = user.name;
		myResponsibleCellController.userModel = user;
		return go;
	}


}

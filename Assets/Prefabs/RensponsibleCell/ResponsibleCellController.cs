using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PageNavFrameWork;

public class ResponsibleCellController : MonoBehaviour
{
	public Text userName;
	public Text functions;
	public UserModel userModel;

	public void OnEditResponsible ()
	{
		var dict = new Dictionary<string, object> ();
		dict.Add (userModel.userID, userModel);
		PageNav.GetPageNavInstance ().PushPageToStackWithArgs (PagesEnum.EditResponsiblePopup, dict);
	}

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

	public static GameObject Instantiate (Transform CellPrefabTransform, ResponsibleModel user)
	{
		GameObject go = GameObject.Instantiate (CellPrefabTransform).gameObject;
		var myResponsibleCellController = go.GetComponent<ResponsibleCellController> ();
		myResponsibleCellController.userName.text = user.name;
		if (user.servicesProvided != null) {
			foreach (var key in user.servicesProvided.Keys) {
				if (string.IsNullOrEmpty (myResponsibleCellController.functions.text)) {
					myResponsibleCellController.functions.text = (user.servicesProvided [key] as ServicesProvidedModel).name;
				} else {
					myResponsibleCellController.functions.text = myResponsibleCellController.functions.text + " / " + (user.servicesProvided [key] as ServicesProvidedModel).name;
				}
			}
		}
		myResponsibleCellController.userModel = user;
		return go;
	}


}

using UnityEngine;
using System.Collections;
using PageNavFrameWork;
using System.Collections.Generic;
using UnityEngine.UI;

public class SelectClientPopupController : PageController
{


	public Transform cellPrefab;
	public RectTransform scrollContentList;
	public GameObject nullListMessage;
	public Button nextButton;

	List <GameObject> userCell = new List<GameObject> ();
	List<UserModel> clientsList = new List<UserModel> ();
	UserModel userSelected;

	void Start ()
	{
		Loading = true;
		if (DataManager.currentUser.userType == Constants.UserType.Company.ToString ()) {
			if ((DataManager.currentUser as CompanyModel).clients != null && (DataManager.currentUser as CompanyModel).clients.Count > 0) {
				var list = (DataManager.currentUser as CompanyModel).clients.Keys;
				foreach (var clientKey in list) {
					clientsList.Add ((UserModel)(DataManager.currentUser as CompanyModel).clients [clientKey]);
				}
				FillList ();
			} else {
				nullListMessage.SetActive (true);
				Loading = false;
			}
		} else {
			DataManager.GetAllClientsFromCompany ((DataManager.currentUser as ResponsibleModel).companyID, delegate(List<UserModel> users) {
				clientsList = users;
				if (clientsList.Count > 0) {
					FillList ();
				} else {
					nullListMessage.SetActive (true);
					Loading = false;
				}
			});
		}
	
	}

	void FillList ()
	{
		clientsList.ForEach (x => userCell.Add (ClientCellController.Instantiate (cellPrefab, x)));
		StartCoroutine (OnFillList ());
	}

	IEnumerator OnFillList ()
	{
		yield return new WaitForSeconds (1f);
		userCell.ForEach (x => x.transform.SetParent (scrollContentList, false));
		ReadjustScrollSize (userCell.Count);
		Loading = false;
	}

	void ReadjustScrollSize (int size)
	{
		scrollContentList.anchorMax = new Vector2 (1, 1);
		scrollContentList.anchorMin = new Vector2 (0, 1);

		scrollContentList.offsetMax = new Vector2 (0, 0);
		var number = (((RectTransform)cellPrefab).rect.height * (size + 1));

		scrollContentList.offsetMin = new Vector2 (0, -number);
	}

	public void OnClientSelectedClick (GameObject client)
	{
		userSelected = client.GetComponent<ClientCellController> ().userModel;
		nextButton.interactable = true;
	}

	public void OnNextButtonClicked ()
	{
		DataManager.CreateNewAppointment (userSelected, delegate {
			Loading = false;
			DropAllPagesFromStack ();
			StartCoroutine (MyCloseModal ());
		}, delegate (string error) {
			Loading = false;
			CloseModal ();
		});
	}

	IEnumerator MyCloseModal ()
	{
		yield return new WaitForSeconds (0.3f);
		CloseModal ();
	}
}

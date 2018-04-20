using UnityEngine;
using System.Collections;
using PageNavFrameWork;
using System.Collections.Generic;
using UnityEngine.UI;

public class SelectClientPopupController : PageController
{

	public InputField search;
	public Transform cellPrefab;
	public RectTransform scrollContentList;
	public GameObject nullListMessage;
	public Button nextButton;

	List <GameObject> userCell = new List<GameObject> ();
	List<UserModel> clientsList = new List<UserModel> ();

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
			var user = (DataManager.currentUser as ResponsibleModel);
			DataManager.GetAllClientsFromCompany (user.companyID, delegate(List<UserModel> users) {
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

	public void Search ()
	{
		if (!string.IsNullOrEmpty (search.text)) {
			if (userCell != null && userCell.Count > 0) {
				userCell.ForEach (x => {
					if (!x.GetComponent<ClientCellController> ().userName.text.ToLower ().Contains (search.text.ToLower ())) {
						x.SetActive (false);
					} else {
						x.SetActive (true);
					}
				});
			}
		} else {
			if (userCell != null && userCell.Count > 0) {
				userCell.ForEach (x => {
					x.SetActive (true);
				});
			}
		}
	}

	void FillList ()
	{
		clientsList.ForEach (x => userCell.Add (ClientCellController.Instantiate (cellPrefab, x, delegate(UserModel user) {
			OnNextButtonClicked (user);
		})));
		StartCoroutine (OnFillList ());
	}

	IEnumerator OnFillList ()
	{
		yield return new WaitForSeconds (1f);
		userCell.ForEach (x => {
			x.transform.SetParent (scrollContentList, false);
		});
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

	public void OnNextButtonClicked (UserModel client)
	{
		Loading = true;
		DataManager.CreateNewAppointment (client, delegate {
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
		GameObject.Destroy (this.gameObject);
	}
}

using UnityEngine;
using System.Collections;
using PageNavFrameWork;
using System.Collections.Generic;

public class ManageResponsiblePageController : PageController
{
	public Transform cellPrefab;
	public RectTransform scrollContentList;
	public GameObject nullListMessage;

	List <GameObject> userCell = new List<GameObject> ();

	void Start ()
	{
		Loading = true;
		if (DataManager.responsibles != null && DataManager.responsibles.Count != 0) {
			FillList ();
		} else {
			nullListMessage.SetActive (true);
		}
	}

	void OnDeleteUserClicked ()
	{
//		PageNav.GetPageNavInstance().PushPageToStackWithArgs(PagesEnum.ConfirmDeleteUserPopUp)
	}

	void FillList ()
	{
		DataManager.responsibles.ForEach (x => userCell.Add (ResponsibleCellControler.Instantiate (cellPrefab, x)));
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

}

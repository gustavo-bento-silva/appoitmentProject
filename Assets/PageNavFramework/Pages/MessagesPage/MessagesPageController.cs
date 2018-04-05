using UnityEngine;
using System.Collections;
using PageNavFrameWork;
using System.Collections.Generic;

public class MessagesPageController : PageController
{

	public Transform cellPrefab;
	public RectTransform scrollContentList;
	public GameObject nullListMessage;

	List <GameObject> messageCell = new List<GameObject> ();

	void Start ()
	{
		CheckMessages ();
	}

	void Update ()
	{

	}

	void CheckMessages ()
	{
		Loading = true;
		if (DataManager.currentUser != null) {
			if (DataManager.userMessages.Count >= 1) {
				nullListMessage.SetActive (false);
				FillList ();
			} else {
				Loading = false;
			}
		} else {
			Loading = false;
		}
	}

	void FillList ()
	{
		DataManager.userMessages.ForEach (x => messageCell.Add (MessageCellControler.Instantiate (cellPrefab, x.message, x)));
		StartCoroutine (OnFillList ());
	}

	IEnumerator OnFillList ()
	{
		yield return new WaitForSeconds (1f);
		messageCell.ForEach (x => x.transform.SetParent (scrollContentList, false));
		ReadjustScrollSize (messageCell.Count);
		DataManager.SetMessagesToRead ();
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

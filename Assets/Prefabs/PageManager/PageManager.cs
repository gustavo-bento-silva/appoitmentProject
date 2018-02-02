using UnityEngine;
using System.Collections;

public enum Pages
{

}

public enum Popups
{
	Login,
	NewUser,
	Null
}

public class PageManager : MonoBehaviour
{

	public static PageManager _instance = null;

	public GameObject[] pagesPrefabs;
	public GameObject[] popupPrefabs;
	Popups currentPopupIndex = Popups.Null;
	GameObject currentPopup;

	public static PageManager GetPageManagerInstance ()
	{
		return _instance;
	}

	void Awake ()
	{
		if (_instance == null) {
			_instance = this;
		}
	}

	public void ShowPopup (Popups popup)
	{
		StartCoroutine (ShowPopupAux (popup));
	}

	public IEnumerator ShowPopupAux (Popups popup)
	{
		if (currentPopupIndex != Popups.Null) {
			currentPopup.GetComponent<PopupController> ().ClosePopUp ();
			yield return new WaitForSeconds (0.5f);
		}
		GameObject popupObject = GameObject.Instantiate (popupPrefabs [(int)popup]);
		popupObject.transform.SetParent (gameObject.transform.parent, false);

		popupObject.GetComponent<PopupController> ().OpenPopUp ();

		currentPopupIndex = popup;
		currentPopup = popupObject;

		yield return new WaitForEndOfFrame ();
	}

	public void CloseCurrentPopUp ()
	{
		currentPopup.GetComponent<PopupController> ().ClosePopUp ();
		currentPopupIndex = Popups.Null;
	}


}

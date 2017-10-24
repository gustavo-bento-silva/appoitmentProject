using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PopupController : MonoBehaviour
{

	public GameObject content;
	public bool scaleToZeroOnInit = true;
	public Popups nextPopup;

	private Vector3 originalScale = new Vector3(1, 1, 1);

	void Awake ()
	{
		if (scaleToZeroOnInit) {
			content.transform.localScale = Vector3.zero;
		}
	}

	public void ClosePopUp ()
	{
		iTween.ScaleTo (content, iTween.Hash ("scale", Vector3.zero, "time", 0.4f, "oncompletetarget", this.gameObject, "oncomplete", "OnDestroyPopUp"));
	}

	public void OpenPopUp ()
	{
		iTween.Stop ();
		iTween.ScaleTo (content, iTween.Hash ("scale", originalScale, "time", 0.4f));
	}

	public void OnDestroyPopUp ()
	{
		GameObject.Destroy (content);
	}

	public void OpenNextPopup ()
	{
		PageManager.GetPageManagerInstance ().ShowPopup (nextPopup);
	}
}

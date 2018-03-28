using UnityEngine;
using System.Collections;
using PageNavFrameWork;
using UnityEngine.UI;

public class CreateServicePopupController : PageController
{
	public Text name;
	public GameObject nameError;
	public Text price;
	public Text duration;
	public GameObject durationError;

	float durantionValue = 0;

	void Start ()
	{

	}

	public void OnButtonClick ()
	{
		if (VerifyError ()) {
			durantionValue = float.Parse (duration.text);
			var service = new ServicesProvidedModel (name.text, durantionValue, price.text);
			DataManager.AddServiceToCompanyAsUser (service);
			Success = true;
			DropAllPagesFromStack ();
			Constants.LoadHomePage ();
			CloseModal ();
		}
	}

	bool VerifyError ()
	{
		if (string.IsNullOrEmpty (name.text)) {
			nameError.SetActive (true);
			return false;
		} else {
			nameError.SetActive (false);
		}
		if (string.IsNullOrEmpty (duration.text)) {
			durationError.SetActive (true);
			return false;
		} else {
			durationError.SetActive (false);
		}
		return true;
	}
}

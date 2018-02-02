using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using PageNavFrameWork;

public class NavigationBarControl : MonoBehaviour {
	public float fadeTime = 0.5f;
	public Color textColor = new Color(0,0,0,1);
	public bool DebugPageNames = false;
	[SerializeField]
	private Text title;
	[SerializeField]
	private Button backButton;
	private Text backButtonText;
	private PageNav pageNavInstance;
	private int oldStackSize = 0;
	private bool animating = false;
	private bool onPageChange = false;

	
	void Start () {
		backButtonText = backButton.GetComponentInChildren<Text> ();
		pageNavInstance = PageNav.GetPageNavInstance ();
		backButton.interactable = false;
		backButtonText.color = new Color (textColor.r, textColor.g, textColor.b, 0);
		oldStackSize = pageNavInstance.PageStackLength;
	}
	
	void Update () {

		var currentStackLength = pageNavInstance.PageStackLength;
		if(currentStackLength!=oldStackSize){
			oldStackSize = currentStackLength;
			onPageChange = true;
			iTween.Stop (this.gameObject);
			animating = false;
		}
		if (pageNavInstance.PageStackLength > 1) {
			if (!animating && onPageChange) {
				animating = true;
				var col = new Color (textColor.r, textColor.g, textColor.b, 1);
				Hashtable tweenParams = new Hashtable ();
				tweenParams.Add ("from", backButtonText.color);
				tweenParams.Add ("to", col);
				tweenParams.Add ("time", fadeTime);
				tweenParams.Add ("onupdate", "OnColorUpdated");
				iTween.ValueTo (this.gameObject, tweenParams);
				tweenParams.Add("oncomplete","FinishedColorFade");
				backButton.interactable = true;
			}
		} else {
			if (!animating && onPageChange) {
				animating = true;
				var col = new Color (textColor.r, textColor.g, textColor.b, 0);
				Hashtable tweenParams = new Hashtable ();
				tweenParams.Add ("from", backButtonText.color);
				tweenParams.Add ("to", col);
				tweenParams.Add ("time", fadeTime);
				tweenParams.Add ("onupdate", "OnColorUpdated");
				tweenParams.Add("oncomplete","FinishedColorFade");
				iTween.ValueTo (this.gameObject, tweenParams);
				backButton.interactable = false;
			}
		}

		if(string.IsNullOrEmpty(pageNavInstance.GetCurrentPage ().Title)){ 
			if (DebugPageNames) {
				title.text = pageNavInstance.GetCurrentPage ().name;
			} else {
				title.text = "";
			}
		}else{
			title.text = pageNavInstance.GetCurrentPage ().Title;
		}
	}

	public void PopPageFromStack(){
		pageNavInstance.PopPageFromStack ();
		onPageChange = true;
	}

	private void OnColorUpdated(Color color)
	{
		backButtonText.color = color;
	}

	private void FinishedColorFade()
	{
		animating = false;
		onPageChange = false;
	}
}

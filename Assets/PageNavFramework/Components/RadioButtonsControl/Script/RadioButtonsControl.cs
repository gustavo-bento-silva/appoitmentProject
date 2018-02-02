using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class RadioButtonsControl : MonoBehaviour {

	[System.Serializable]
	public class RadioButton{
		bool selected;
		Image sprite;

		public RadioButton(bool selected, Image sprite){
			this.sprite = sprite;
			this.selected = selected;
		}
	}

	public RectTransform Content;
	public bool InitiateSelected;
	public bool AutoRun;
	public int Selected = 0;
	public Color NormalColor;
	public Color SelectedColor;
	public UnityEvent OnSelect;

	
	void Start () {
		if(AutoRun){
			Initialize ();
		}
	}
	
	
	void Update () {
	
	}

	void Initialize(){
		GetContentForButtons ();

	}

	void GetContentForButtons(){
		int count = 0;
		foreach(RectTransform rt in Content){
			Image image = rt.gameObject.GetComponentInChildren<Image> ();
			if (InitiateSelected) {
				if (count == Selected) {
					image.color = SelectedColor;
				} else {
					image.color = NormalColor;
				}
			} else {
				Color myColor = NormalColor;
				myColor.a = 1;
				image.color = myColor;
			}
			if (rt.gameObject.GetComponent<Button> () == null) {
				Button button = rt.gameObject.AddComponent<Button> ();
				button.onClick.AddListener (ButtonClicked);

			} else {
				Button button = rt.gameObject.GetComponent<Button> ();
				button.onClick.AddListener ((ButtonClicked));
			}

			count++;
		}
	}

	public void ButtonClicked(){
		Debug.Log ("Button Clicked event");
		int count = 0;
		Image image;
		Selected = 0;
		foreach(RectTransform rt in Content){
			image = rt.GetComponentInChildren<Image> ();
			if (EventSystem.current.currentSelectedGameObject == rt.gameObject) {
				image.color = SelectedColor;
				Selected = count;
			} else {
				image.color = NormalColor;
			}
			count++;
		}
		string method = OnSelect.GetPersistentMethodName (0);
		object target = OnSelect.GetPersistentTarget (0);
		(target as MonoBehaviour).InvokeWithArgument(method, new IntArg(Selected) as Object) ;

	}
}

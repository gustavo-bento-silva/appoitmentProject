using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class StarsControl : MonoBehaviour {

	public bool ContinousUpdate = false;
	public Color DisabledColor; 
	public Color EnabledColor;
	public GameObject[] Stars;
	private List<Image> _Images = new List<Image> ();

	public int StarValue = 0;

	
	void Start () {
		if (Stars.Length == 0) {
			//use hierarquy childrean as data instead of inspector data.
			UseChildrenAsStars ();
		} else {
			//make a list from inpector data
			foreach (GameObject go in Stars) {
				_Images.Add (go.GetComponent<Image> ());
			}
		}
		StandarizeStarValue ();
		for(int i = 0; i<Stars.Length ;i++){
			_Images [i].color = DisabledColor;
		}
	}
	
	
	void Update () {
		//update Star Status:
		if(ContinousUpdate){
			StandarizeStarValue();
			for(int i = 0; i<Stars.Length ;i++){
				if(i < StarValue){
					_Images [i].color = EnabledColor;
				}else{
					_Images [i].color = DisabledColor;
				}
			}
		}
	}

	void StandarizeStarValue(){
		if(StarValue < 0){
			StarValue = 0;
		}else if(StarValue > Stars.Length){
			StarValue = Stars.Length;
		}
	}

	void UseChildrenAsStars ()
	{
		List<GameObject> list = new List<GameObject> ();
		foreach (RectTransform rt in this.transform) {
			list.Add (rt.gameObject);
			_Images.Add (rt.gameObject.GetComponent<Image> ());
		}
		Stars = list.ToArray ();
	}
}

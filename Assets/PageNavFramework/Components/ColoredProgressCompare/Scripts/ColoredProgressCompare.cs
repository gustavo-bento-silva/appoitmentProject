using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ColoredProgressCompare : MonoBehaviour {

	public float points1Value;
	public float points2Value;
	public ProgressBarControl progressBar;
	public Text point1Text;
	public Text point2Text;
	public Image backgroundLeft;
	public Image backgroundRight;
	public bool update;
	public Color colorNeutral;
	public Color colorLeft;
	public Color colorRight;

	void Start () {
		calculateComparison ();
	}

	void Update () {
		if(update){
			calculateComparison ();
		}
	}

	public void calculateComparison(){
		if (points1Value == 0 && points2Value == 0) {
			progressBar.progress = 50;
		} else {
			progressBar.progress = (points1Value/(points1Value+points2Value))*100;
		}
		if(points1Value > points2Value){
			backgroundLeft.color = colorLeft;
			backgroundRight.color = colorNeutral;
		}else if(points1Value < points2Value){
			backgroundLeft.color = colorNeutral;
			backgroundRight.color = colorRight;
		}else{
			backgroundLeft.color = colorLeft;
			backgroundRight.color = colorRight;
		}
		point1Text.text = points1Value.ToString ();
		point2Text.text = points2Value.ToString ();
	}
}

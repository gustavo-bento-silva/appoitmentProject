using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;

public class ProgressBarControl : MonoBehaviour {
	
	public RectTransform ProgressBar;
	[Range(0,100)]
	public float progress = 50;

	private Text progressText;
	private float ProgressBarParentWidth;
	private float initRightSideOffsetMinY;
	private float initProrgessBarOffsetMinY;


	void Start () {
		progressText = gameObject.GetComponentInChildren<Text>();
		ProgressBarParentWidth = ProgressBar.gameObject.GetComponentInParent<RectTransform> ().rect.width;
		initProrgessBarOffsetMinY = ProgressBar.offsetMin.y;

		SetProgressBarWidth (progress);
	}
	
	
	void Update () {
		SetProgressBarWidth (progress);
	}

	void SetProgressBarWidth(float amount){
		float percentage = (100-amount)/100;
		ProgressBar.offsetMax = - new Vector2 (percentage*ProgressBarParentWidth ,initProrgessBarOffsetMinY);
		ProgressBar.offsetMin = new Vector2 (0,-initProrgessBarOffsetMinY);
	}

	void AddProgress(float amount){

	}
}

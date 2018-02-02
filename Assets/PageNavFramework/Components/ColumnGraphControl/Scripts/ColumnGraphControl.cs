using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(RectTransform	))]
public class ColumnGraphControl : MonoBehaviour {

	#region ColumnEntity
	[System.Serializable]
	public class ColumnEntry{
		public RectTransform Column;
		[Range(0,100)]
		public int amount = 0;
	}
	#endregion
	public bool FluidBehaviour = true;
	public bool BeginAnimated = true;
	public float ColumnSideBorder = 5;
	public float TimeToFinishAnimating = 2;
	public float WaitBeforeAnimating = 1;
	public iTween.EaseType easeType = iTween.EaseType.easeOutQuart;
	public ColumnEntry[] ColumnList;

	private RectTransform  rectTransform;
	private float SpaceWidth = 0;
	private float ColumnWidth = 0;
	private bool HasAnimationFinished = false;

	void Awake(){
		this.rectTransform = GetComponent<RectTransform>();
	}

	
	void Start () {
		CalculatePositionsAndSizes ();
		if (BeginAnimated) {
			SetColumnPositionsInitial ();
		} else {
			SetColumnPositions ();
		}
	}
	
	
	void Update () {
		if (FluidBehaviour && HasAnimationFinished) {
			CalculatePositionsAndSizes ();
			SetColumnPositions ();
		}
		
	}

	void CalculatePositionsAndSizes(){
		Rect graphicRect = rectTransform.rect;
		float MaxWidthNormalized = (1.0f / (ColumnList.Length)) - (ColumnSideBorder/graphicRect.width);
		ColumnWidth = MaxWidthNormalized * graphicRect.width;
		SpaceWidth = (graphicRect.width - (ColumnWidth * ColumnList.Length)) / (ColumnList.Length + 1);
	}

	void SetColumnPositionsInitial(){
		float currentPosition = SpaceWidth;
		foreach(ColumnEntry ce in ColumnList){
			RectTransform column = ce.Column;
			column.anchorMin = new Vector2 (0,0);
			column.anchorMax = new Vector2 (1,1);
			column.offsetMin = new Vector2 (currentPosition,0);// Left, Botton
			column.offsetMax = new Vector2 (-(rectTransform.rect.width - (currentPosition+ColumnWidth)),-rectTransform.rect.height); //Rigth,Top
			currentPosition += SpaceWidth + ColumnWidth;
		}
		StartCoroutine (AnimateColumns ());

	}

	void SetColumnPositions(){
		float currentPosition = SpaceWidth;
		foreach(ColumnEntry ce in ColumnList){
			RectTransform column = ce.Column;
			column.anchorMin = new Vector2 (0,0);
			column.anchorMax = new Vector2 (1,1);
			column.offsetMin = new Vector2 (currentPosition,0);// Left, Botton
			column.offsetMax = new Vector2 (-(rectTransform.rect.width - (currentPosition+ColumnWidth)),-rectTransform.rect.height*(100-ce.amount)/100); //Rigth,Top
			currentPosition += SpaceWidth + ColumnWidth;
		}

	}

	IEnumerator AnimateColumns(){
		yield return new WaitForSeconds (WaitBeforeAnimating);
		iTween.ValueTo( gameObject, iTween.Hash(
			"from", 0,
			"to", 1000,
			"time", TimeToFinishAnimating,
			"onupdatetarget", gameObject,
			"onupdate", "tweenOnUpdateCallBack",
			"oncompletetarget", gameObject,
			"oncomplete", "FinishedAnimating",
			"easetype", easeType
		));
	}


	void tweenOnUpdateCallBack( int newValue )
	{
		float percentage = newValue / 1000f; 
		foreach(ColumnEntry ce in ColumnList){
			RectTransform column = ce.Column;
			column.offsetMax = new Vector2 (column.offsetMax.x,-((rectTransform.rect.height*(100-ce.amount)/100)+((rectTransform.rect.height*(ce.amount*(1-percentage))/100)))); //Rigth,Top
		}
	}

	void FinishedAnimating(){
		HasAnimationFinished = true;
	}


	/// <summary>
	/// Updates the columns and animate again.
	/// </summary>
	public void UpdateColumns(){
		SetColumnPositionsInitial ();
	}

}

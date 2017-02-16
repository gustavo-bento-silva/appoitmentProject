using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;


public class CarouselControl : MonoBehaviour {

	enum State{Waiting,Pressed,Release};

	public bool autorun = true;
	public float sensitivity = 20;
	public int StartingElement = 0;
	public float SpaceBetweenElements = 0;
	public float PullTime = 10;
	public iTween.EaseType SnapEaseType;
	public RectTransform Middle;
	public RectTransform Content;
	public RectTransform[] Elements;

	private bool _initialized = false;
	private int _currentElement = 0;
	private ScrollRect _scroll;
	private float _ElemWidth;
	private State _state = State.Waiting;

	private Vector2 pressPosition;
	private Vector2 releasePosition;
	
	void Awake () {
		_currentElement = StartingElement;
		_scroll= gameObject.GetComponent<ScrollRect> ();
		if(autorun){
			Initialize ();
			if(_currentElement != 0){
				SnapToCurrentElement ();
			}
		}
	}
	
	
	void Update () {
		if(_state == State.Release){
			_state = State.Waiting;
			SnapToCurrentElement ();
		}
	}

	public void Initialize(){
		_initialized = true;
		GetChildrenAsObjects ();
		ArrangeElementPositions ();
	}

	void GetChildrenAsObjects(){
		List<RectTransform> list = new List<RectTransform> ();
		foreach(RectTransform rt in Content){
			TestForAnchorPos (rt);
			if(rt.gameObject.activeSelf){
				list.Add (rt);
			}

		}
		Elements = list.ToArray ();
	}

	void TestForAnchorPos(RectTransform rt){
		if(rt.anchorMax != new Vector2(1,1) || rt.anchorMin != Vector2.zero){
			throw new UnityException("The anchor type of all the children RectTransform inside Content must be Stretch-Stretch");
		}
	}


	void ArrangeElementPositions(){
		_ElemWidth = Content.rect.width;
		float FullWidth = (Elements.Length - 1) *(_ElemWidth+SpaceBetweenElements);
		float Ypos = 0;
		int count = 0;
		WrapWithContent (FullWidth);
		foreach(RectTransform rt in Elements){
			rt.offsetMax = - new Vector2 (FullWidth - (Ypos),0);
			rt.offsetMin = new Vector2 (Ypos,0);
			Ypos += _ElemWidth + SpaceBetweenElements;
			count++;
		}
	}

	void WrapWithContent(float width){
		Content.offsetMin = new Vector2 (0,0);
		Content.offsetMax = new Vector2 (width,0);
		Content.anchorMin = new Vector2 (0,0);
		Content.anchorMax = new Vector2 (1,1);
	}

	public void PressFinger(){
		_state = State.Pressed;
		pressPosition = _scroll.content.offsetMin;
		iTween.StopByName ("CarouselControlSnap");
	}

	public void ReleaseFinger(){
		releasePosition = _scroll.content.offsetMin - pressPosition;
		if(releasePosition.magnitude >= sensitivity){
			if(releasePosition.x > 0){
				if(_currentElement != 0){
					_currentElement--;
				}
			}else{
				if(_currentElement != Elements.Length - 1){
					_currentElement++;
				}
			}
		}
		_state = State.Release;
	}

	void SnapToCurrentElement(){
		float FullWidth = (Elements.Length - 1) *(_ElemWidth+SpaceBetweenElements);
		float distanceFromMiddle = (FullWidth/2)-(_ElemWidth+SpaceBetweenElements)*_currentElement;
		_scroll.velocity = Vector2.zero;
		iTween.MoveTo (Content.gameObject, iTween.Hash ("name","CarouselControlSnap","x",  distanceFromMiddle, "time", PullTime, "easetype", SnapEaseType, "islocal", true, "OnComplete", "ZeroScrollVelocity","oncompletetarget",this.gameObject));
	}

	void ZeroScrollVelocity(){
		_scroll.velocity = Vector2.zero;
	}

	int GetClosestElementToMiddle (){
		float mindist = Vector3.Distance(Elements[0].position,Middle.position);
		int minElem = 0;
		for(int i=1;i<Elements.Length;i++){
			float newDist = Vector2.Distance (Elements [i].position, Middle.position);
			if(Mathf.Abs(newDist) < Mathf.Abs(mindist)){
				mindist = newDist;
				minElem = i;
			}
		}
		return minElem;
	}

	public void ScrollToObjectWithIndex(int index){
		_currentElement = index;
		SnapToCurrentElement();
	}
}

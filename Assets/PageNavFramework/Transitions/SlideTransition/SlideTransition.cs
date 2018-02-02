using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "SlideTransition", menuName = "PageNav/Transition/SlideTransition", order = 1)]
public class SlideTransition : PageTransition {

	public enum Direction{Rigth, Left, Up, Down};

	[Range(0.1f,10f)]
	public float duration = 0.5f;
	[Header("Transition enter Settings")]
	public Direction directionEntering = Direction.Rigth;
	public iTween.EaseType easeTypeEntering = iTween.EaseType.easeOutExpo;
	[Header("Transition exiting Settings")]
	public Direction directionExiting = Direction.Rigth;
	public iTween.EaseType easeTypeExiting = iTween.EaseType.easeInExpo;

	public override IEnumerator BeginTransitionTo (RectTransform targetTransform)
	{
		Vector3 newPagePos = NewPage.transform.position;
		Vector3 oldPos = OldPage.transform.position;
		Rect NewPageRect = ((RectTransform)NewPage.transform).rect;
		Vector3 initPos = new Vector3 (newPagePos.x+NewPageRect.width,newPagePos.y,newPagePos.z);
		NewPage.transform.position = initPos;

		GameObject go = new GameObject();
		go.name = "TransitionHelperObjectTo";
		var mh = go.AddComponent<MotionHelper> ();
		mh.transition = this;

		float offset = 0;

		switch(directionEntering){
		case Direction.Left:
			offset = - ((RectTransform)NewPage.transform).rect.width;
			((RectTransform)NewPage.transform).offsetMax = new Vector2(offset, 0);
			((RectTransform)NewPage.transform).offsetMin = new Vector2(offset, 0);
			break;
		case Direction.Rigth:
			offset = ((RectTransform)NewPage.transform).rect.width;
			((RectTransform)NewPage.transform).offsetMax = new Vector2(offset, 0);
			((RectTransform)NewPage.transform).offsetMin = new Vector2(offset, 0);
			break;
		case Direction.Up:
			offset = ((RectTransform)NewPage.transform).rect.height;
			((RectTransform)NewPage.transform).offsetMax = new Vector2(0,offset);
			((RectTransform)NewPage.transform).offsetMin = new Vector2(0,offset);
			break;
		case Direction.Down:
			offset = - ((RectTransform)NewPage.transform).rect.height;
			((RectTransform)NewPage.transform).offsetMax = new Vector2(0,offset);
			((RectTransform)NewPage.transform).offsetMin = new Vector2(0,offset);
			break;
		}

		iTween.MoveTo (NewPage.gameObject, iTween.Hash ("position",  targetTransform.position, "time", duration, "easetype", easeTypeEntering.ToString(), "islocal", false,"OnComplete", "FinishMotion","oncompletetarget",go));

		yield return null;
	}

	public override IEnumerator BeginTransitionFrom (RectTransform targetTransform)
	{
		Vector3 newPagePos = NewPage.transform.position;
		Vector3 oldPos = OldPage.transform.position;
		Rect NewPageRect = ((RectTransform)NewPage.transform).rect;
		Vector3 initPos;
		Vector3 zero = Vector2.zero;
		GameObject go = new GameObject();
		go.name = "TransitionHelperObjectFrom";
		var mh = go.AddComponent<MotionHelper> ();
		mh.transition = this;

		float offset = 0;

		switch(directionExiting){
		case Direction.Left:
			offset =  - ((RectTransform)NewPage.transform).rect.width;
			((RectTransform)NewPage.transform).offsetMax = new Vector2 (offset, 0);
			((RectTransform)NewPage.transform).offsetMin = new Vector2 (offset, 0);
			break;
		case Direction.Rigth:
			offset =  ((RectTransform)NewPage.transform).rect.width;
			((RectTransform)NewPage.transform).offsetMax = new Vector2 (offset, 0);
			((RectTransform)NewPage.transform).offsetMin = new Vector2 (offset, 0);
			break;
		case Direction.Up:
			offset =  ((RectTransform)NewPage.transform).rect.height;
			((RectTransform)NewPage.transform).offsetMax = new Vector2 (0, offset);
			((RectTransform)NewPage.transform).offsetMin = new Vector2 (0, offset);
			break;
		case Direction.Down:
			offset =  -((RectTransform)NewPage.transform).rect.height;
			((RectTransform)NewPage.transform).offsetMax = new Vector2 (0, offset);
			((RectTransform)NewPage.transform).offsetMin = new Vector2 (0, offset);
			break;
		}
		initPos = NewPage.transform.position;
		((RectTransform)NewPage.transform).offsetMax = zero;
		((RectTransform)NewPage.transform).offsetMin = zero;
		iTween.MoveTo (NewPage.gameObject, iTween.Hash ("position",  initPos, "time", duration, "easetype", easeTypeExiting.ToString(), "islocal", false,"OnComplete", "FinishMotion","oncompletetarget",go));


		yield return null;
	}

	public class MotionHelper : MonoBehaviour{

		public SlideTransition transition;

		public void FinishMotion(){
			transition.FinishTransition ();
			Destroy(this.gameObject);
		}

	}
}
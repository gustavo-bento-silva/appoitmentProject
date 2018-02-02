using UnityEngine;
using System.Collections;
using UnityEditor;

//Big thanks to Phedg1, a StackOverflow user, from whom I adapted thos script.
//This is the thread from where i got the code:
// http://answers.unity3d.com/questions/1100603/how-to-make-anchor-snap-to-self-rect-transform-in.html

public class AnchorToCornersScript{

	public static bool manualRefresh = true;
	public static Rect anchorRect;
	public static Vector2 anchorVector;
	private static Rect anchorRectOld;
	private static Vector2 anchorVectorOld;
	private static RectTransform ownRectTransform;
	private static RectTransform parentRectTransform;
	private static Vector2 pivotOld;
	private static Vector2 offsetMinOld;
	private static Vector2 offsetMaxOld;

	public static GameObject gameObject;

	public static void executeScript(){
		Transform transform = gameObject.transform;
		ownRectTransform = gameObject.GetComponent<RectTransform>();
		parentRectTransform = transform.parent.gameObject.GetComponent<RectTransform>();
		if (ownRectTransform.offsetMin != offsetMinOld || ownRectTransform.offsetMax != offsetMaxOld) {
			CalculateCurrentWH();
			CalculateCurrentXY();
		}
		if (ownRectTransform.pivot != pivotOld || anchorVector != anchorVectorOld) {
			CalculateCurrentXY();
			pivotOld = ownRectTransform.pivot;
			anchorVectorOld = anchorVector;
		}
		if (anchorRect != anchorRectOld) {
			AnchorsToCorners();
			anchorRectOld = anchorRect;
		}

		CalculateCurrentWH();
		CalculateCurrentXY();
		AnchorsToCorners();
	}

	private static void CalculateCurrentXY() {
		float pivotX = anchorRect.width * ownRectTransform.pivot.x;
		float pivotY = anchorRect.height * (1 - ownRectTransform.pivot.y);
		Vector2 newXY = new Vector2(ownRectTransform.anchorMin.x * parentRectTransform.rect.width + ownRectTransform.offsetMin.x + pivotX - parentRectTransform.rect.width * anchorVector.x,
			- (1 - ownRectTransform.anchorMax.y) * parentRectTransform.rect.height + ownRectTransform.offsetMax.y - pivotY + parentRectTransform.rect.height * (1 - anchorVector.y));
		anchorRect.x = newXY.x;
		anchorRect.y = newXY.y;
		anchorRectOld = anchorRect;
	}

	private static void CalculateCurrentWH() {
		anchorRect.width = ownRectTransform.rect.width;
		anchorRect.height = ownRectTransform.rect.height;
		anchorRectOld = anchorRect;
	}

	private static void AnchorsToCorners() {
		float pivotX = anchorRect.width * ownRectTransform.pivot.x;
		float pivotY = anchorRect.height * (1 - ownRectTransform.pivot.y) ;
		ownRectTransform.anchorMin = new Vector2(0f, 1f);
		ownRectTransform.anchorMax = new Vector2(0f, 1f);
		ownRectTransform.offsetMin = new Vector2(anchorRect.x / ownRectTransform.localScale.x, anchorRect.y / ownRectTransform.localScale.y - anchorRect.height);
		ownRectTransform.offsetMax = new Vector2(anchorRect.x / ownRectTransform.localScale.x + anchorRect.width, anchorRect.y / ownRectTransform.localScale.y);
		ownRectTransform.anchorMin = new Vector2(ownRectTransform.anchorMin.x + anchorVector.x + (ownRectTransform.offsetMin.x - pivotX) / parentRectTransform.rect.width * ownRectTransform.localScale.x,
			ownRectTransform.anchorMin.y - (1 - anchorVector.y) + (ownRectTransform.offsetMin.y + pivotY) / parentRectTransform.rect.height * ownRectTransform.localScale.y);
		ownRectTransform.anchorMax = new Vector2(ownRectTransform.anchorMax.x + anchorVector.x + (ownRectTransform.offsetMax.x - pivotX) / parentRectTransform.rect.width * ownRectTransform.localScale.x,
			ownRectTransform.anchorMax.y - (1 - anchorVector.y) + (ownRectTransform.offsetMax.y + pivotY) / parentRectTransform.rect.height * ownRectTransform.localScale.y);
		ownRectTransform.offsetMin = new Vector2((0 - ownRectTransform.pivot.x) * anchorRect.width * (1 - ownRectTransform.localScale.x), (0 - ownRectTransform.pivot.y) * anchorRect.height * (1 - ownRectTransform.localScale.y));
		ownRectTransform.offsetMax = new Vector2((1 - ownRectTransform.pivot.x) * anchorRect.width * (1 - ownRectTransform.localScale.x), (1 - ownRectTransform.pivot.y) * anchorRect.height * (1 - ownRectTransform.localScale.y));

		offsetMinOld = ownRectTransform.offsetMin;
		offsetMaxOld = ownRectTransform.offsetMax;
	}
}

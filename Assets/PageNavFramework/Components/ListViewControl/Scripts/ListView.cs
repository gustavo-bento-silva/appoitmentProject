using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;
using PageNavFrameWork;

namespace PageNavFrameWork{
	[System.Serializable]
	[RequireComponent(typeof(ScrollRect))]
	public class ListView : MonoBehaviour {

		//default settings:
		[Header("Default settings")]
		public RectTransform ContentRecTransform;
		[HideInInspector]
		public bool AlwaysScroll = true;
		[HideInInspector]
		public RectTransform[] Content = null;

		//delegates:
		[Header("Delegates")]
		[MethodDelegate("Returns the number of elements in the list.",typeof(int))]
		public MethodDelegate NumberOfCellsDelegate;
		[MethodDelegate("Returns the PREFAB for the GameObject that wil be used as an element.",typeof(GameObject))]
		public MethodDelegate PrefabForCell;
		[MethodDelegate("Each cell will be passed for the script delegate to be filled with info, and its correspondent index.",typeof(void),typeof(GameObject),typeof(int))]
		public MethodDelegate PopulateEachCell;
		[MethodDelegate("Returns the distance between cells.",typeof(float))]
		public MethodDelegate DistanceBetweenCells;

		int ListLength = 0;
		GameObject CellPrefab = null;
		float DistanceBetweenElements = 10;

		//Refresh settings
		[Header("Refresh settings")]
		public bool userefresh = false;
		public bool RotateRefreshImage = false;
		public float TopOffsetForRefreshing = 50;
		public Image refreshImage;
		public UnityEvent OnRefresh;

		[HideInInspector]
		public bool refreshLock = false;

		private List<GameObject> cellsInList = new List<GameObject> ();

		void Start () {
			StartCoroutine(initializeList ());
		}

		public void UpdateList(){
			StartCoroutine(initializeList ());
		}

		private IEnumerator initializeList(){
			yield return new WaitForEndOfFrame ();
			ContentRecTransform.gameObject.SetActive (true);

			if (NumberOfCellsDelegate.target) {
				ListLength = ((int)NumberOfCellsDelegate.CallDelegate ());
				if(ListLength<0){
					Debug.LogError ("List length can not be less than 0!");
					yield break;
				}
			}

			if (PrefabForCell.target) {
				CellPrefab = ((GameObject)PrefabForCell.CallDelegate ());
			}

			if (DistanceBetweenCells.target) {
				DistanceBetweenElements = ((float)DistanceBetweenCells.CallDelegate ());
			}

			CreateList ();

			refreshLock = true;
		}
			
		void Update () {
			if(userefresh){
				if(!refreshLock && ContentRecTransform.offsetMax.y < 0){
					float percentage = ContentRecTransform.offsetMax.magnitude / TopOffsetForRefreshing;
					if (ContentRecTransform.offsetMax.magnitude < TopOffsetForRefreshing) {
						Color transparentColor = refreshImage.color;
						transparentColor.a = percentage;
						refreshImage.color = transparentColor;
						if(RotateRefreshImage){
							refreshImage.transform.rotation = new Quaternion(0,0,0,0);
							refreshImage.transform.Rotate (0,0,-360*percentage);
						}

					} else {
						Color transparentColor = refreshImage.color;
						transparentColor.a = 1.0f;
						refreshImage.color = transparentColor;
						refreshLock = true;
						OnRefresh.Invoke ();
					}
				}else{
					if(ContentRecTransform.offsetMax.magnitude==0){
						refreshLock = false;
					}
				}
			}
		}

		void CreateList(){
			float currentY = 0;

			if(!CellPrefab){
				Debug.LogWarning ("No Cell Prefab set.");
				return;
			}


			DeactivateNotUsedCells ();

			for(int index = 0; index<ListLength;index++){
				RectTransform item = GetReusableCellOrNew(index).transform as RectTransform;
				item.name = CellPrefab.name;
				item.SetParent (ContentRecTransform,false);
				if (PopulateEachCell.target) {
					PopulateEachCell.CallDelegate (item.gameObject,index);
				}
				SetListItemRectPos (item,currentY);
				currentY += item.rect.height + DistanceBetweenElements;
			}

			SetContentRectHeight (currentY - DistanceBetweenElements);

		}

		GameObject GetReusableCellOrNew(int index){
			if (index >= cellsInList.Count) {
				GameObject newCell = Instantiate (CellPrefab);
				cellsInList.Add (newCell);
				newCell.SetActive (true);
				return newCell;
			} else {
				GameObject reusableCell = cellsInList[index];
				reusableCell.SetActive (true);
				return reusableCell;
			}
		}

		void DeactivateNotUsedCells ()
		{
			//Set which cells are unused
			for (int index = ListLength; index < cellsInList.Count; index++) {
				cellsInList [index].SetActive (false);
			}
		}

		void TestForRectAnchorPositions(RectTransform rt){
			if(rt.anchorMin.x != 0 || rt.anchorMin.y != 1 || rt.anchorMax.x!= 1 || rt.anchorMax.y!=1){
				throw new System.Exception ("All ListItems inside an ListView must have anchor position as Stretch-Top!");
			}
		}

		void SetListItemRectPos(RectTransform listItem, float posY){
			float contentHeight = ContentRecTransform.rect.height;
			Vector2 initialSize = listItem.sizeDelta;
			listItem.offsetMax = - new Vector2 (1,posY);
			listItem.offsetMin = - new Vector2 (0,(posY+initialSize.y));
			listItem.anchorMin = new Vector2 (0,1);
			listItem.anchorMax = new Vector2 (1, 1);

		}

		void SetContentRectHeight(float posY){

			ContentRecTransform.anchorMax = new Vector2(1,1);
			ContentRecTransform.anchorMin = new Vector2(0,1);
			ContentRecTransform.offsetMax = new Vector2(0,0);
			if(posY < (gameObject.transform as RectTransform).rect.height){
				ContentRecTransform.offsetMin = new Vector2(0,- (gameObject.transform as RectTransform).rect.height);
			}else{
				ContentRecTransform.offsetMin = new Vector2(0,- posY);
			}
			ScrollRect scrollRect = gameObject.GetComponent<ScrollRect> (); 
			if (ContentRecTransform.sizeDelta.y <= (scrollRect.transform as RectTransform).rect.height && !AlwaysScroll) {
				scrollRect.vertical = false;
			}
		}


	}

	public interface IListViewDelegate{
		int GetNumOfCells ();

		GameObject GetCellPrefab();

		void PopulateEachCell(GameObject cell, int index);

		float GetCellDeistance();
	}
}
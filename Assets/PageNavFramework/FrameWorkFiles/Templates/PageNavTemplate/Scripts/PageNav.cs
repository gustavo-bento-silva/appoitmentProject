
using UnityEngine;
using System.Collections;
using System.Threading;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

namespace PageNavFrameWork
{
	public class PageNav : MonoBehaviour
	{

		private static PageNav _instance = null;

		public PageNavSettings settings;
		public Transform Container;
		public Transform CachingContainer;
		public PagesEnum InitialPage = PagesEnum.None;
		public PagesEnum LoadingPage = PagesEnum.None;
		public PagesEnum ErrorPopup = PagesEnum.None;
		public PagesEnum SuccessPopup = PagesEnum.None;
		public PageTransition DefaultPageTransition = null;
		public PageTransition DefaultModalTransition = null;
		[HideInInspector]
		private PageController _CurrentPage = null;
		public bool UseAndroidBackButton = true;

		public int PageStackLength {
			get {
				return PagesStack.Count;
			}
		}

		private const int positionOffSet = 300;
		private const int loadingCountDownTime = 10;
		private float timeLeft = 0;
		private bool loadingCountDownEnabled;
		private Vector3 sideMenuInitialPosition;
		private RectTransform _loadingPage = null;
		private RectTransform _successPopup = null;
		private RectTransform _errorPopup = null;
		private GameObject _PopUp = null;
		private PageController _PopUpController = null;
		private bool mutex = false;
		private bool transitionMutex = false;
		private Dictionary<PagesEnum,GameObject> CachedPages = new Dictionary<PagesEnum, GameObject> ();
		private bool deactivateBehindPage = true;

		[SerializeField]
		private Stack<PageController> PagesStack = new Stack <PageController> ();

		/// <summary>
		/// Gets the menu instance. The return may be NULL.
		/// </summary>
		public static PageNav GetPageNavInstance ()
		{
			return _instance;
		}

		void Awake ()
		{
			if (_instance == null) {
				_instance = this;
			}
		}

		
		void Start ()
		{
			if (Container == null) {
				Container = transform.Find ("Container").transform;
			}

			//Load Cached pages
			LoadCachedPages ();

			InstatiateSuccessAndErrorPopupAndHide ();

			//instantiate loading page, and hide it
			if (LoadingPage != PagesEnum.None) {
				if (settings.PagesCacheSettings [((int)LoadingPage) - 1]) {
					_loadingPage = CachedPages [LoadingPage].transform as RectTransform;
				} else {
					_loadingPage = Instantiate (GetPagePrefabByEnum (LoadingPage)).transform as RectTransform;
				}

				_loadingPage.SetParent (this.transform, false);
				ArrangePageTransform (_loadingPage.gameObject);
				SetLoadingVisibility (false);
			} else {
				Debug.LogWarning ("The PageNav has no LoadingPage");
			}

			//set initial page
			if (InitialPage != PagesEnum.None) {
				PresentFirstPage (InitialPage);
			} else {
				Debug.LogWarning ("The PageNav has no InitialPage");
			}
		}

		void InstatiateSuccessAndErrorPopupAndHide ()
		{
			if (ErrorPopup != PagesEnum.None) {
				if (settings.PagesCacheSettings [((int)ErrorPopup) - 1]) {
					_errorPopup = CachedPages [ErrorPopup].transform as RectTransform;
				} else {
					_errorPopup = Instantiate (GetPagePrefabByEnum (ErrorPopup)).transform as RectTransform;
				}

				_errorPopup.SetParent (this.transform, false);
				ArrangePageTransform (_errorPopup.gameObject);
				SetLoadingVisibility (false);
			} else {
				Debug.LogWarning ("The PageNav has no ErrorPopup");
			}
			if (SuccessPopup != PagesEnum.None) {
				if (settings.PagesCacheSettings [((int)SuccessPopup) - 1]) {
					_successPopup = CachedPages [SuccessPopup].transform as RectTransform;
				} else {
					_successPopup = Instantiate (GetPagePrefabByEnum (SuccessPopup)).transform as RectTransform;
				}

				_successPopup.SetParent (this.transform, false);
				ArrangePageTransform (_successPopup.gameObject);
				SetLoadingVisibility (false);
			} else {
				Debug.LogWarning ("The PageNav has no SuccessPopup");
			}
		}

		void Update ()
		{
			if (Input.GetKeyDown (KeyCode.Escape) && UseAndroidBackButton) {
				if (_PopUp) {
					if (_PopUpController.usesBackButton) {
						CloseModal ();
					}
				} else {
					if (_CurrentPage.usesBackButton) {
						PopPageFromStack ();
					}
				}
			}
			if (loadingCountDownEnabled) {
				timeLeft -= Time.deltaTime;

				if (timeLeft <= 0) {
					EndTimer ();
					loadingCountDownEnabled = false;
				}
			}
		}

		void PresentFirstPage (PagesEnum page)
		{
			SetLoadingVisibility (true);
			StartCoroutine (AsyncShowFirstPage (page));
		}


		private IEnumerator AsyncShowFirstPage (PagesEnum page)
		{

			var pageInstance = CreatePageInstance (page);
			PageController pageController = pageInstance.GetComponent<PageController> ();
			pageInstance.GetComponent<Canvas> ().renderMode = RenderMode.WorldSpace;

			pageInstance.transform.SetParent (this.Container);
			if (!pageController.isCached) {
				SetComponentsForPages (pageInstance.gameObject);
			}

			ArrangePageTransform (pageInstance);


			PagesStack.Push (pageController);

			_CurrentPage = pageController;

			pageController.EnablePage ();

			SetLoadingVisibility (false);
			yield return null;
		}

		void ArrangeNewPageTransform (RectTransform newPage)
		{
			newPage.anchorMin = Vector2.zero;
			newPage.anchorMax = new Vector2 (1, 1);
			newPage.offsetMax = new Vector2 ((Container.transform as RectTransform).rect.width, 0);
			newPage.offsetMin = new Vector2 ((Container.transform as RectTransform).rect.width, 0);
			newPage.localScale = new Vector3 (1, 1, 1);
		}

		/// <summary>
		/// Sets the transform of the <c>pageInstance</c> so it fits nicely in the menu.
		/// </summary>
		/// <param name="pageInstance">Page instance has to have a Rect Transform Component.</param>
		public void ArrangePageTransform (GameObject pageInstance)
		{
			(pageInstance.transform as RectTransform).anchorMin = Vector2.zero;
			(pageInstance.transform as RectTransform).anchorMax = new Vector2 (1, 1);
			(pageInstance.transform as RectTransform).offsetMax = Vector2.zero;
			(pageInstance.transform as RectTransform).offsetMin = Vector2.zero;
			(pageInstance.transform as RectTransform).localScale = new Vector3 (1, 1, 1);
		}

		public void SetSuccessVisibility (bool state)
		{
			if (!_successPopup) {
				return;
			}
			_successPopup.SetAsLastSibling ();
			_successPopup.gameObject.SetActive (state);
		}

		public void SetErrorVisibility (bool state, string msg = "")
		{
			if (!_errorPopup) {
				return;
			}
			_errorPopup.SetAsLastSibling ();
			if (!string.IsNullOrEmpty (msg)) {
				Helper.FindComponentInChildWithTag <Text> (_errorPopup.gameObject, "ErrorMessage").text = msg;
			}
			_errorPopup.gameObject.SetActive (state);
		}

		public void SetLoadingVisibility (bool state, string msg = "")
		{
			if (!_loadingPage) {
				return;
			}
			_loadingPage.SetAsLastSibling ();
			if (!string.IsNullOrEmpty (msg)) {
				Helper.FindComponentInChildWithTag <Text> (_successPopup.gameObject, "SuccessMessage").text = msg;
			}
			_loadingPage.gameObject.SetActive (state);
			if (state) {
				StartTimer ();
			}
		}

		void StartTimer ()
		{
			timeLeft = loadingCountDownTime;
			loadingCountDownEnabled = true;
		}

		void EndTimer ()
		{
			SetLoadingVisibility (false);
			SetErrorVisibility (true);
		}

		public PageController GetCurrentPage ()
		{
			return _CurrentPage;
		}

		public void CloseModal ()
		{
			if (_PopUp && !transitionMutex) {
				transitionMutex = true;
				PageController pageController = _PopUp.GetComponent<PageController> ();
				ArrangePageTransform (_CurrentPage.gameObject);
				PageTransition pageTransition = null;
				if (!pageController.pageTransition) {
					pageTransition = DefaultModalTransition;
				} else {
					pageTransition = pageController.pageTransition;
				}
				pageTransition.NewPage = pageController;
				pageTransition.OldPage = _CurrentPage;
				pageTransition.initTransitionFrom (FinishedModalTransitionFrom, this.transform as RectTransform);
			}
		}

		void RemoveCanvasRayCastOfContent (GameObject go)
		{
			GraphicRaycaster gr = go.GetComponent<GraphicRaycaster> ();
			if (gr) {
				GameObject.Destroy (gr);
			}
		}

		static void SetComponentsForPages (GameObject pageInstance)
		{
			GameObject.Destroy (pageInstance.gameObject.GetComponent<GraphicRaycaster> ());
			GameObject.Destroy (pageInstance.gameObject.GetComponent<CanvasScaler> ());
			GameObject.Destroy (pageInstance.gameObject.GetComponent<Canvas> ());
		}

		public GameObject GetPagePrefabByEnum (PagesEnum pageEnum)
		{
			if (pageEnum == PagesEnum.None) {
				return null;
			}
			GameObject prefab = GetPagePrefabOrCachedPage (pageEnum);
			if (!prefab) {
				Debug.LogWarning ("Trying to get prefab from enum " + pageEnum.ToString () + ", but prefab is empty.");
			}
			return prefab;
		}

		public PageArgs GetDefaultPageArgs (PagesEnum pageEnum)
		{
			if (pageEnum == PagesEnum.None) {
				return null;
			}
			int index = (int)pageEnum - 1;
			return settings.PagesCustomArguments [index];
		}

		public void FinishedPageTransitionTo (PageController oldPage, PageController newPage)
		{
			if (deactivateBehindPage) {
				oldPage.gameObject.SetActive (false);
				oldPage.DisablePage ();
			}
			transitionMutex = false;
		}

		public void FinishedPageTransitionFrom (PageController oldPage, PageController newPage)
		{
			Destroy (newPage.gameObject);
			transitionMutex = false;
		}

		/// <summary>
		/// Pushs the page to stack. If Page's transition is NULL, then default transition will occur.
		/// </summary>
		/// <param name="PageEnum">Page enum.</param>
		public void PushPageToStack (PagesEnum pageEnum, bool deactivateBehindPage = true)
		{
			if ((int)pageEnum - 1 >= settings.PagesPrefabs.Count) {
				Debug.LogWarning ("The pageEnum you are trying to use does not exist!");
				return;
			}
			if (!transitionMutex) {
				this.deactivateBehindPage = deactivateBehindPage;
				transitionMutex = true;
				var pageInstance = CreatePageInstance (pageEnum);
				pageInstance.transform.SetParent (this.Container);
				PageController pageController = pageInstance.GetComponent<PageController> ();
				SetComponentsForPages (pageInstance.gameObject);
				ArrangePageTransform (pageInstance);
				PagesStack.Push (pageController);
				PageTransition pageTransition;
				if (!pageController.pageTransition) {
					pageTransition = DefaultPageTransition;
				} else {
					pageTransition = pageController.pageTransition;
				}
				pageTransition.OldPage = _CurrentPage;
				_CurrentPage = pageController;
				pageTransition.NewPage = _CurrentPage;
				pageTransition.initTransitionTo (FinishedPageTransitionTo, Container as RectTransform);

			}
		}

		/// <summary>
		/// Pushs the page to stack with arguments. These arguments are passed to the PageController as arguments for the method InstantiatedWithArgs, which the page must override.
		/// </summary>
		/// <param name="pageEnum">Page enum.</param>
		/// <param name="args">Arguments.</param>
		public void PushPageToStackWithArgs (PagesEnum pageEnum, Dictionary<string,object> args, bool deactivateBehindPage = true)
		{
			if ((int)pageEnum - 1 >= settings.PagesPrefabs.Count) {
				Debug.LogWarning ("The pageEnum you are trying to use does not exist!");
				return;
			}
			if (!transitionMutex) {
				this.deactivateBehindPage = deactivateBehindPage;
				transitionMutex = true;
				var pageInstance = CreatePageInstance (pageEnum);
				pageInstance.transform.SetParent (this.Container);
				PageController pageController = pageInstance.GetComponent<PageController> ();
				SetComponentsForPages (pageInstance.gameObject);
				ArrangePageTransform (pageInstance);
				PagesStack.Push (pageController);
				pageController.InstantiatedWithArgs (args);
				PageTransition pageTransition;
				if (!pageController.pageTransition) {
					pageTransition = DefaultPageTransition;
				} else {
					pageTransition = pageController.pageTransition;
				}
				pageTransition.OldPage = _CurrentPage;
				_CurrentPage = pageController;
				pageTransition.NewPage = _CurrentPage;
				pageTransition.initTransitionTo (FinishedPageTransitionTo, Container as RectTransform);

			}
		}

		/// <summary>
		/// Pushs the page to stack. If Page's transition is NULL, then default transition will occur.
		/// </summary>
		/// <param name="PageEnum">Page enum.</param>
		public void PushPageToStack (GameObject pageInstance, bool deactivateBehindPage = true)
		{
			if (!pageInstance) {
				return;
			}
			if (!transitionMutex) {
				this.deactivateBehindPage = deactivateBehindPage;
				transitionMutex = true;
				pageInstance.SetActive (true);
				pageInstance.transform.SetParent (this.Container);
				PageController pageController = pageInstance.GetComponent<PageController> ();
				if (!pageController.isCached) {
					SetComponentsForPages (pageInstance.gameObject);
				}
				ArrangePageTransform (pageInstance);
				PagesStack.Push (pageController);
				PageTransition pageTransition;
				if (!pageController.pageTransition) {
					pageTransition = DefaultPageTransition;
				} else {
					pageTransition = pageController.pageTransition;
				}
				pageTransition.OldPage = _CurrentPage;
				_CurrentPage = pageController;
				pageTransition.NewPage = _CurrentPage;
				pageTransition.initTransitionTo (FinishedPageTransitionTo, Container as RectTransform);

			}
		}

		/// <summary>
		/// Pushs the page to stack with arguments. These arguments are passed to the PageController as arguments for the method InstantiatedWithArgs, which the page must override.
		/// </summary>
		/// <param name="pageInstance">Page instance.</param>
		/// <param name="args">Arguments.</param>
		public void PushPageToStackWithArgs (GameObject pageInstance, Dictionary<string,object> args, bool deactivateBehindPage = true)
		{
			if (!pageInstance) {
				return;
			}
			if (!transitionMutex) {
				this.deactivateBehindPage = deactivateBehindPage;
				transitionMutex = true;
				pageInstance.SetActive (true);
				pageInstance.transform.SetParent (this.Container);
				PageController pageController = pageInstance.GetComponent<PageController> ();
				pageController.InstantiatedWithArgs (args);
				if (!pageController.isCached) {
					SetComponentsForPages (pageInstance.gameObject);
				}
				ArrangePageTransform (pageInstance);
				PagesStack.Push (pageController);
				PageTransition pageTransition;
				if (!pageController.pageTransition) {
					pageTransition = DefaultPageTransition;
				} else {
					pageTransition = pageController.pageTransition;
				}
				pageTransition.OldPage = _CurrentPage;
				_CurrentPage = pageController;
				pageTransition.NewPage = _CurrentPage;
				pageTransition.initTransitionTo (FinishedPageTransitionTo, Container as RectTransform);

			}
		}


		/// <summary>
		/// Pops the page from stack. If amount > stack.size, then, the stack will drop to the first page.
		/// </summary>
		/// <param name="amount">Amount of pages to pop.</param>
		public void PopPageFromStack ()
		{
			if (PagesStack.Count <= 1) {
				return;
			}
			if (!transitionMutex) {
				transitionMutex = true;
				PageController pageController = PagesStack.Pop ();
				_CurrentPage = PagesStack.Peek ();
				_CurrentPage.gameObject.SetActive (true);
				ArrangePageTransform (_CurrentPage.gameObject);
				PageTransition pageTransition = null;
				if (!pageController.pageTransition) {
					pageTransition = DefaultPageTransition;
				} else {
					pageTransition = pageController.pageTransition;
				}
				pageTransition.NewPage = pageController;
				pageTransition.OldPage = _CurrentPage;
				pageTransition.initTransitionFrom (FinishedPageTransitionFrom, Container as RectTransform);
			}
		}

		/// <summary>
		/// Pops the page from stack. If amount > stack.size, then, the stack will drop to the first page.
		/// </summary>
		/// <param name="amount">Amount of pages to pop.</param>
		public void PopPageFromStack (int amount)
		{
			if (PagesStack.Count <= 1 || amount <= 0) {
				return;
			}
			if ((PagesStack.Count - amount) < 1) {
				amount = PagesStack.Count - 1;
			}
			if (!transitionMutex) {
				transitionMutex = true;
				PageController pageController = PagesStack.Pop ();
				int size = PagesStack.Count;
				for (int i = 0; i < amount - 1; i++) {
					GameObject go = PagesStack.Pop ().gameObject;	
					Destroy (go);
				}
				_CurrentPage = PagesStack.Peek ();
				_CurrentPage.gameObject.SetActive (true);
				ArrangePageTransform (_CurrentPage.gameObject);
				PageTransition pageTransition = null;
				if (!pageController.pageTransition) {
					pageTransition = DefaultPageTransition;
				} else {
					pageTransition = pageController.pageTransition;
				}
				pageTransition.NewPage = pageController;
				pageTransition.OldPage = _CurrentPage;
				pageTransition.initTransitionFrom (FinishedPageTransitionFrom, Container as RectTransform);
			}
		}

		/// <summary>
		/// Return to first page of the stack.
		/// </summary>
		public void DropAllPagesFromStack ()
		{
			if (PagesStack.Count <= 1) {
				return;
			}
			if (!transitionMutex) {
				transitionMutex = true;
				PageController pageController = PagesStack.Pop ();
				int size = PagesStack.Count;
				for (int i = 0; i < size - 1; i++) {
					GameObject go = PagesStack.Pop ().gameObject;	
					Destroy (go);
				}
				_CurrentPage = PagesStack.Peek ();
				_CurrentPage.gameObject.SetActive (true);
				ArrangePageTransform (_CurrentPage.gameObject);
				PageTransition pageTransition = null;
				if (!pageController.pageTransition) {
					pageTransition = DefaultPageTransition;
				} else {
					pageTransition = pageController.pageTransition;
				}
				pageTransition.NewPage = pageController;
				pageTransition.OldPage = _CurrentPage;
				pageTransition.initTransitionFrom (FinishedPageTransitionFrom, Container as RectTransform);
			}
		}

		/// <summary>
		/// Caches the page, if the page is not already cached.
		/// </summary>
		/// <param name="pageEnum">Page enum.</param>
		public void CachePage (PagesEnum pageEnum)
		{
			GameObject prefab = GetPagePrefabByEnum (pageEnum);
			if (prefab.GetComponent<PageController> ().isCached) {
				return;
			}
			GameObject page = Instantiate (prefab);
			page.SetActive (false);
			page.name = "[Cached]" + prefab.name;
			page.transform.SetParent (CachingContainer);
			PageController pageController = page.GetComponent<PageController> ();
			if (pageController == null) {
				pageController = page.AddComponent<PageController> ();
			}
			pageController.isCached = true;
			CachedPages.Add (pageEnum, page);
		}

		private void LoadCachedPages ()
		{
			for (int index = 0; index < settings.PagesCacheSettings.Count; index++) {
				if (settings.PagesCacheSettings [index]) {
					PagesEnum pageEnum = (PagesEnum)(index + 1);
					GameObject prefab = settings.PagesPrefabs [index];
					GameObject page = Instantiate (prefab);
					page.SetActive (false);
					page.name = "[Cached]" + prefab.name;
					page.transform.SetParent (CachingContainer);
					PageController pageController = page.GetComponent<PageController> ();
					if (pageController == null) {
						pageController = page.AddComponent<PageController> ();
					}
					pageController.isCached = true;
					CachedPages.Add (pageEnum, page);
				}
			}
		}

		GameObject GetPagePrefabOrCachedPage (PagesEnum pageEnum)
		{
			int index = ((int)pageEnum) - 1;
			GameObject prefab = null;
			if (settings.PagesCacheSettings [index]) {
				prefab = CachedPages [pageEnum];
			} else {
				prefab = this.settings.PagesPrefabs [index];
			}
			return prefab;
		}

		GameObject CreatePageInstance (PagesEnum page)
		{
			GameObject pagePrefab = GetPagePrefabByEnum (page);
			GameObject pageInstance = Instantiate (pagePrefab);
			pageInstance.SetActive (true);
			return pageInstance;
		}

		/// <summary>
		/// Opens modal page.
		/// </summary>
		/// <param name="pagePrefab0">Page Enum.</param>
		public void OpenModal (GameObject modalPrefab)
		{
			if (transitionMutex) {
				return;
			}
			CloseModal ();
			transitionMutex = true;
			GameObject pageInstance = Instantiate (modalPrefab);
			pageInstance.SetActive (true);
			SetLoadingVisibility (false);
			RectTransform rectTransform = pageInstance.transform as RectTransform;
			rectTransform.SetParent (this.transform);
			rectTransform.SetAsLastSibling ();
			ArrangePageTransform (rectTransform.gameObject);
			_PopUp = rectTransform.gameObject;
			_PopUpController = _PopUp.GetComponent<PageController> ();
			PageController pageController = pageInstance.GetComponent<PageController> ();
			PageTransition pageTransition = null;
			if (!pageController.pageTransition) {
				pageTransition = DefaultModalTransition;
			} else {
				pageTransition = pageController.pageTransition;
			}
			pageTransition.NewPage = pageController;
			pageTransition.OldPage = _CurrentPage;
			pageTransition.initTransitionTo (FinishedModalTransitionTo, this.transform as RectTransform);
		}

		public void FinishedModalTransitionTo (PageController oldPage, PageController newPage)
		{
			transitionMutex = false;
		}

		public void FinishedModalTransitionFrom (PageController oldPage, PageController newPage)
		{
			Destroy (_PopUp);
			_PopUp = null;
			_PopUpController = null;
			transitionMutex = false;
		}
	}



}
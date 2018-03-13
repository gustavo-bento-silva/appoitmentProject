using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PageNavFrameWork
{
	public class PageController : MonoBehaviour
	{
		public bool Loading {
			get {
				return _loading;
			}
			set {
				_loading = value;
				PageNav.GetPageNavInstance ().SetLoadingVisibility (value);
			}
		}

		public bool Success {
			get {
				return _success;
			}
			set {
				_success = value;
				PageNav.GetPageNavInstance ().SetSuccessVisibility (value);
			}
		}

		public bool Error {
			get {
				return _error;
			}
			set {
				_error = value;
				PageNav.GetPageNavInstance ().SetErrorVisibility (value);
			}
		}

		public string Title = "";
		public PageTransition pageTransition = null;
		#if UNITY_EDITOR || UNITY_ANDROID
		public bool usesBackButton = true;
		#else
		[HideInInspector]
		public bool usesBackButton = false; 
		#endif

		protected bool _loading;
		protected bool _success;
		protected bool _error;
		protected PageNav PageNavInstance = null;
		protected GameObject PagePrefab = null;

		[HideInInspector]
		public bool isCached = false;

		void LoadPageNavInstanceToProperty ()
		{
			if (!PageNavInstance) {
				PageNavInstance = PageNav.GetPageNavInstance ();
			}
		}

		public void Awake ()
		{
			LoadPageNavInstanceToProperty ();
		}

		public void OpenErrorPopup ()
		{
			_error = true;
			PageNav.GetPageNavInstance ().SetErrorVisibility (true);
		}

		public void OpenSuccessPopup ()
		{
			_success = true;
			PageNav.GetPageNavInstance ().SetSuccessVisibility (true);
		}

		public void CloseErrorPopup ()
		{
			_error = false;
			PageNav.GetPageNavInstance ().SetErrorVisibility (false);
		}

		public void CloseSuccessPopup ()
		{
			_success = false;
			PageNav.GetPageNavInstance ().SetSuccessVisibility (false);
		}

		/// <summary>
		/// Pushs the page to stack.
		/// </summary>
		/// <param name="PageEnum">Page enum.</param>
		public void PushPageToStack (PageEnumHolder pageEnumHolder)
		{
			PageNavInstance.PushPageToStack (pageEnumHolder.page, pageEnumHolder.DeactivateBehindPage);
		}

		/// <summary>
		/// Pushses a page to the stack with arguments.
		/// </summary>
		/// <param name="pageEnum">Page enum.</param>
		/// <param name="args">Arguments.</param>
		public void PushPageToStackWithArgs (PagesEnum pageEnum, Dictionary<string,object> args)
		{
			PageNav.GetPageNavInstance ().PushPageToStackWithArgs (pageEnum, args);
		}

		/// <summary>
		/// Pushses a page to the stack with arguments.
		/// </summary>
		/// <param name="pageArgsHolder">Page arguments holder.</param>
		public void PushPageToStackWithArgs (PageArgs pageArgsHolder)
		{
			PageNav.GetPageNavInstance ().PushPageToStackWithArgs (pageArgsHolder.PageToCreate, pageArgsHolder.DictionaryArgs);
		}

		/// <summary>
		/// Pops the page from stack.
		/// </summary>
		/// <param name="amount">Amount of pages to pop.</param>
		public void PopPageFromStack ()
		{
			PageNavInstance.PopPageFromStack ();
		}

		/// <summary>
		/// Pops 'amount' pages from the stack if it can.
		/// </summary>
		/// <param name="amount">Amount of pages to pop.</param>
		public void PopPageFromStack (int amount)
		{
			PageNavInstance.PopPageFromStack (amount);
		}

		/// <summary>
		/// Return to first page of the stack.
		/// </summary>
		public void DropAllPagesFromStack ()
		{
			PageNavInstance.DropAllPagesFromStack ();
		}

		/// <summary>
		/// Opens modal page.
		/// </summary>
		/// <param name="pagePrefab0">Page Enum.</param>
		public void OpenModal (PageEnumHolder pageEnumHolder)
		{
			GameObject pagePrefab = PageNavInstance.GetPagePrefabByEnum (pageEnumHolder.page);
			if (!pagePrefab) {
				return;
			}
			PageNavInstance.OpenModal (pagePrefab);
		}

		/// <summary>
		/// Closes the modal page.
		/// </summary>
		public void CloseModal ()
		{
			PageNavInstance.CloseModal ();
		}

		/// <summary>
		/// This method will be called when the page is shown on the PageNav.
		/// </summary>
		public virtual void EnablePage ()
		{
		}

		/// <summary>
		/// This method will be called when the page is going to be hidden on the PageNav.
		/// </summary>
		public virtual void DisablePage ()
		{
		}


		/// <summary>
		/// This method will be called after the Awake() but before Start(). This method is called whenever a page is instanciated using the PageNavs PushPageToStackWithArgs() method.
		/// </summary>
		/// <param name="args">Arguments.</param>
		public virtual void InstantiatedWithArgs (Dictionary<string,object> args)
		{
		}

	}
}
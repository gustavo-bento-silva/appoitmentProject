using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PageNavFrameWork{
	public class PageController : MonoBehaviour {
		public bool Loading{
			get{
				return _loading;
			}
			set{
				_loading = value;
				PageNav.GetPageNavInstance ().SetLoadingVisibility (value);
			}
		}

		public PageTransition pageTransition = null;

		protected bool _loading;
		protected PageNav PageNavInstance = null;
		protected GameObject PagePrefab = null;

		[HideInInspector]
		public bool isCached = false;


		public void Awake(){
			if(!PageNavInstance){
				PageNavInstance = PageNav.GetPageNavInstance ();
			}

			//TODO get own gameObject
		}

		/// <summary>
		/// Pushs the page to stack.
		/// </summary>
		/// <param name="PageEnum">Page enum.</param>
		public void PushPageToStack(PageEnumHolder pageEnumHolder){
			PageNavInstance.PushPageToStack (pageEnumHolder.page);
		}

		/// <summary>
		/// Pops the page from stack.
		/// </summary>
		/// <param name="amount">Amount of pages to pop.</param>
		public void PopPageFromStack(){
			PageNavInstance.PopPageFromStack ();
		}

		/// <summary>
		/// Pops 'amount' pages from the stack if it can.
		/// </summary>
		/// <param name="amount">Amount of pages to pop.</param>
		public void PopPageFromStack(int amount){
			PageNavInstance.PopPageFromStack (amount);
		}

		/// <summary>
		/// Return to first page of the stack.
		/// </summary>
		public void DropAllPagesFromStack(){
			PageNavInstance.DropAllPagesFromStack ();
		}

		/// <summary>
		/// Opens modal page.
		/// </summary>
		/// <param name="pagePrefab0">Page Enum.</param>
		public void OpenModal(PageEnumHolder pageEnumHolder){
			GameObject pagePrefab = PageNavInstance.GetPagePrefabByEnum (pageEnumHolder.page);
			if(!pagePrefab){
				return;
			}
			PageNavInstance.OpenModal (pagePrefab);
		}

		/// <summary>
		/// Closes the modal page.
		/// </summary>
		public void CloseModal(){
			PageNavInstance.CloseModal ();
		}

		public void EnablePage(){
			//TODO
		}

		public void DisablePage(){
			//TODO
		}

	}
}
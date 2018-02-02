using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using PageNavFrameWork;

//Exemple of editor scripting:
using System;


public class PageTransition : ScriptableObject {

	[HideInInspector]
	public PageController OldPage = null;
	[HideInInspector]
	public PageController NewPage = null;

	public UnityEvent OnFinish;

	private Action<PageController,PageController> callback;

	public void initTransitionTo(Action<PageController,PageController> callback, RectTransform targetTransform){
		if(!OldPage || !NewPage){
			Debug.LogError ("Transition could not be completed.");
			return;
		}
		this.callback = callback;
		PageNav.GetPageNavInstance ().StartCoroutine (BeginTransitionTo(targetTransform));
	}

	public void initTransitionFrom(Action<PageController,PageController> callback, RectTransform targetTransform){
		if(!OldPage || !NewPage){
			Debug.LogError ("Transition could not be completed.");
			return;
		}
		this.callback = callback;
		PageNav.GetPageNavInstance ().StartCoroutine (BeginTransitionFrom(targetTransform));
	}

	/// <summary>
	/// This function will pe called by the PageNav singleton, to begin the transition to the NewPage.
	/// To create custom transition, override this function. This function MUST call FinishTransition method, 
	/// when transition is over.
	/// Attibutes<code>OldPage</code> and <code>NewPage</code> must be different than <code>NULL</code>.
	/// </summary>
	virtual public IEnumerator BeginTransitionTo(RectTransform targetTransform){
		yield return null;
		FinishTransition ();
	}

	/// <summary>
	/// This function will pe called by the PageNav singleton, to begin the transition back to the OldPage.
	/// To create custom transition, override this function. This function MUST call FinishTransition method, 
	/// when transition is over.
	/// Attibutes<code>OldPage</code> and <code>NewPage</code> must be different than <code>NULL</code>.
	/// </summary>
	virtual public IEnumerator BeginTransitionFrom(RectTransform targetTransform){
		yield return null;
		FinishTransition ();
	}



	public void FinishTransition(){
		callback (OldPage,NewPage);
		OnFinish.Invoke ();
	}
}

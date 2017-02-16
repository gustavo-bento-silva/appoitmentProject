using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class WebImage : MonoBehaviour {

	public string url = "";
	private Image image = null;
	public bool useLoading = true;
	public GameObject Loading;

	void Awake(){
		image = gameObject.GetComponent<Image> ();
		if(!useLoading){
			if(Loading != null)Loading.SetActive (false);
		}
	}

	void Start () {
		if(!string.IsNullOrEmpty(url)){
			GetWebImageAsync ();
		}
	}
	
	public void GetWebImageAsync(){
		StartCoroutine (GetWebImageEnumerator());
	}

	private IEnumerator GetWebImageEnumerator(){
		WWW www = new WWW (url);
		while(!www.isDone){
			yield return new WaitForEndOfFrame ();
		}

		if (www.texture != null) {
			
			if(Loading != null)
			{	
				Loading.SetActive (false);
			}
			this.image.sprite = Sprite.Create ((Texture2D)www.texture, new Rect (0, 0, www.texture.width, www.texture.height), new Vector2 (0.5f, 0.5f));
		} else {
			throw new UnityException ("The URL ("+url+") has not returned an image.");
		}

	}
}

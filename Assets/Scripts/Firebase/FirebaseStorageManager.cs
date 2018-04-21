using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Storage;
using System.Threading.Tasks;
using System;

public class FirebaseStorageManager : MonoBehaviour
{

	static FirebaseStorageManager _instance;
	private StorageReference imagesReference;
	FirebaseStorage storage;
	string bucketReference = "gs://appointmentproject-a7233.appspot.com";
	string format = ".jpg";

	public enum ChildsReferences
	{
		CompanyImages,
		companyImage
	}

	public static FirebaseStorageManager GetFireBaseInstance ()
	{
		return _instance;
	}

	void Awake ()
	{
		storage = FirebaseStorage.DefaultInstance;
//		Firebase.Storage.StorageReference storage_ref =
//			storage.GetReferenceFromUrl ("gs://appointmentproject-a7233.appspot.com");
//		imagesReference = storage_ref.Child (ChildsReferences.CompanyImages.ToString ());
		if (_instance == null) {
			_instance = this;
		}
	}
	//	gs://appointmentproject-a7233.appspot.com/CompanyImages/z0iJvJUBK2aK2BP2OAuACDrNMSn1/companyImage.jpg
	public void LoadImage (string companyID, Delegates.OnSpriteSuccess success)
	{
		var filepath = string.Format ("{0}/{1}/{2}/{3}{4}", bucketReference, ChildsReferences.CompanyImages.ToString (), companyID, ChildsReferences.companyImage.ToString (), format);
		Firebase.Storage.StorageReference gs_reference = storage.GetReferenceFromUrl (filepath);
		gs_reference.GetDownloadUrlAsync ().ContinueWith ((Task<Uri> task) => {
			if (!task.IsFaulted && !task.IsCanceled) {
				Debug.Log ("Download URL: " + task.Result);
				StartCoroutine (LoadImageInternet (task.Result.ToString (), success));
			} else {

				Debug.Log (task.Exception.ToString ());
			}
		});
	}

	IEnumerator LoadImageInternet (string url, Delegates.OnSpriteSuccess success)
	{
		var tex = new Texture2D (4, 4, TextureFormat.DXT1, false);
		var Link = new WWW (url);
		yield return Link;
		Link.LoadImageIntoTexture (tex);
		var mSprite = Sprite.Create (tex, new Rect (0, 0, tex.width, tex.height), new Vector2 (0, 0)); 
		success (mSprite);
	}
}

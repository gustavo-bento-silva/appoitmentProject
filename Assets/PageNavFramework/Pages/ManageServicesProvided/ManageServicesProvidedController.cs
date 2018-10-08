using UnityEngine;
using System.Collections;
using PageNavFrameWork;
using System.Collections.Generic;

public class ManageServicesProvidedController : PageController
{

	public Transform cellPrefab;
	public RectTransform scrollContentList;
	public GameObject nullListMessage;

	List<GameObject> userCell = new List<GameObject>();
	List<ServicesProvidedModel> services = new List<ServicesProvidedModel>();

	void Start()
	{
		Loading = true;
		if ((DataManager.currentUser as CompanyModel).servicesProvided != null)
		{
			if ((DataManager.currentUser as CompanyModel).servicesProvided.Count != 0)
			{
				foreach (var serviceKey in (DataManager.currentUser as CompanyModel).servicesProvided.Keys)
				{
					services.Add((ServicesProvidedModel)(DataManager.currentUser as CompanyModel).servicesProvided[serviceKey]);
				}
				nullListMessage.SetActive(false);
				FillList();
			}
			else
			{
				Loading = false;
			}
		}
		else
		{
			Loading = false;
		}
	}

	void OnDeleteUserClicked()
	{
		//		PageNav.GetPageNavInstance().PushPageToStackWithArgs(PagesEnum.ConfirmDeleteUserPopUp)
	}

	void FillList()
	{
		services.ForEach(x => userCell.Add(ServiceManagerCell.Instantiate(cellPrefab, x)));
		StartCoroutine(OnFillList());
	}

	IEnumerator OnFillList()
	{
		yield return new WaitForSeconds(1f);
		userCell.ForEach(x => x.transform.SetParent(scrollContentList, false));
		ReadjustScrollSize(userCell.Count);
		Loading = false;
	}

	void ReadjustScrollSize(int size)
	{
		scrollContentList.anchorMax = new Vector2(1, 1);
		scrollContentList.anchorMin = new Vector2(0, 1);

		scrollContentList.offsetMax = new Vector2(0, 0);
		var number = (((RectTransform)cellPrefab).rect.height * (size + 1));

		if (number < scrollContentList.offsetMin.y)
		{
			scrollContentList.offsetMin = new Vector2(0, -number);
		}
	}
}

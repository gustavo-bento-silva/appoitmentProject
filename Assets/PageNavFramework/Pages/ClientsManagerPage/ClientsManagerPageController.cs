using UnityEngine;
using System.Collections;
using PageNavFrameWork;
using System.Collections.Generic;

public class ClientsManagerPageController : PageController
{

	public Transform cellPrefab;
	public RectTransform scrollContentList;
	public GameObject nullListMessage;

	List<GameObject> userCell = new List<GameObject>();
	List<UserModel> clientsList = new List<UserModel>();

	void Start()
	{
		Loading = true;
		if (DataManager.currentUser.userType == Constants.UserType.Company.ToString())
		{
			CaseUserAsCompany((DataManager.currentUser as CompanyModel));
		}
		else
		{
			CaseUserAsResponsible();
		}
	}

	void CaseUserAsCompany(CompanyModel user)
	{
		if (user.clients != null)
		{
			if (user.clients.Count != 0)
			{
				foreach (var clientKey in user.clients.Keys)
				{
					clientsList.Add((UserModel)user.clients[clientKey]);
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

	void CaseUserAsResponsible()
	{
		DataManager.GetUserById((DataManager.currentUser as ResponsibleModel).companyID, delegate (UserModel user)
		{
			CompanyModel company = new CompanyModel(user);
			DataManager.GetAllClientsFromCompany(user.userID, delegate (List<UserModel> users)
			{
				users.ForEach(x => company.clients.Add(x.userID, (object)x));
				CaseUserAsCompany(company);
			});
		});
	}

	void FillList()
	{
		clientsList.ForEach(x => userCell.Add(ClientCellController.Instantiate(cellPrefab, x, delegate (UserModel user)
		{

		})));
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

		scrollContentList.offsetMin = new Vector2(0, -number);
	}

	public void CreateNewClientPageWithArgs()
	{
		var dict = new Dictionary<string, object>();
		dict.Add("0", true);
		PageNav.GetPageNavInstance().PushPageToStackWithArgs(PagesEnum.CreateNewClientPage, dict);
	}
}

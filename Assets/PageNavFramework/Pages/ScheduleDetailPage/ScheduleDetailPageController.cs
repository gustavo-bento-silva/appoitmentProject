using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PageNavFrameWork;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.UI;
using System.Globalization;

public class ScheduleDetailPageController : PageController
{
	public RectTransform scrollViewContent;
	public Transform cellPrefabTransform;
	public GameObject blockDayButton;
	public GameObject backDayButton;
	public GameObject unblockDayButton;
	public GameObject errorpopup;

	public Text appointmentData;

	int appointmentDay;
	int appointmentMonth;
	int appointmentYear;

	int limit;
	int begin;

	int appointmentIndex = 0;

	bool isFromSchedulePage = false;
	bool isBlockDay = false;

	List<GameObject> cellList = new List<GameObject>();

	DateTime dt;

	public override void InstantiatedWithArgs(Dictionary<string, object> args)
	{
		isFromSchedulePage = (bool)args["isFromScheduleAppointment"];
	}

	void Start()
	{
		Loading = true;
		GetAppointmentList();
	}

	void GetAppointmentList()
	{
		CultureInfo provider = new CultureInfo("pt-BR");
		DateTime date;
		DataManager.GetResponsibleAppointments(delegate ()
		{
			DataManager.GetBlockDayByResponsible(DataManager.currentResponsible.userID, delegate (List<BlockDay> blockDayList)
			{
				blockDayList.ForEach(x =>
				{
					date = DateTime.ParseExact(x.data, Constants.dateformat, provider);
					if (date.Year == DataManager.dateNewAppointment.Year && date.Month == DataManager.dateNewAppointment.Month && date.Day == DataManager.dateNewAppointment.Day)
					{
						isBlockDay = true;
					}
				});
				Initilize();
			}, delegate (string error)
			{
				Loading = false;
				Error = true;
			});
		}, delegate (string error)
		{
			Loading = false;
			Error = true;
		});
	}

	void Initilize()
	{
		UpdateTextData();
		var dtNow = DateTime.Now;
		var appointmentDate = new DateTime(appointmentYear, appointmentMonth, appointmentDay);
		var resp = DataManager.currentResponsible;
		var temp = DataManager.currentResponsible.timeToBeginWork[(int)dtNow.DayOfWeek];
		var hour = Mathf.FloorToInt(DataManager.currentResponsible.timeToBeginWork[(int)dtNow.DayOfWeek]);
		var minute = (int)((DataManager.currentResponsible.timeToBeginWork[(int)appointmentDate.DayOfWeek] - hour) * 60);
		dt = new DateTime(appointmentYear, appointmentMonth, appointmentDay, hour, minute, 0);
		InitializeScheduleTime();
	}

	public void SetErrorVisibility(bool status)
	{
		errorpopup.SetActive(status);
	}

	public void BlockDay()
	{
		CultureInfo provider = new CultureInfo("pt-BR");
		bool thereIsAppointment = false;
		var day = DataManager.dateNewAppointment.Day;
		var month = DataManager.dateNewAppointment.Month;
		var year = DataManager.dateNewAppointment.Year;
		DateTime mAppointmentDate = new DateTime(year, month, day);
		DataManager.responsibleAppointmentList.ForEach(x =>
		{
			if (DateTime.ParseExact(x.data, Constants.dateformat, provider) == mAppointmentDate)
			{
				thereIsAppointment = true;
			}
		});
		if (thereIsAppointment)
		{
			SetErrorVisibility(true);
		}
		else
		{
			Loading = true;
			DataManager.AddBlockDayForResponsible(DataManager.currentResponsible, DataManager.dateNewAppointment, delegate ()
			{
				Loading = false;
				Constants.LoadHomePage();
			}, delegate (string error)
			{
				Loading = false;
				Error = true;
			});
		}
	}

	public void UnblockDay()
	{
		Loading = true;
		DataManager.RemoveBlockDayForResponsible(DataManager.currentResponsible, DataManager.dateNewAppointment, delegate ()
		{
			Loading = false;
			Constants.LoadHomePage();
		}, delegate (string error)
		{
			Loading = false;
			Error = true;
		});
	}

	void UpdateTextData()
	{
		appointmentDay = DataManager.dateNewAppointment.Day;
		appointmentMonth = DataManager.dateNewAppointment.Month;
		appointmentYear = DataManager.dateNewAppointment.Year;

		if (!isFromSchedulePage)
		{
			appointmentData.text = string.Format("Dia {0}/{1}/{2} \nProfissional: {3} \nServiço: {4}", appointmentDay, appointmentMonth,
				appointmentYear, DataManager.currentResponsible.name, DataManager.currentservice.name);
		}
		else
		{
			backDayButton.SetActive(false);
			if (isBlockDay)
			{
				blockDayButton.SetActive(false);
				unblockDayButton.SetActive(true);
			}
			else
			{
				blockDayButton.SetActive(true);
				unblockDayButton.SetActive(false);
			}
			appointmentData.text = string.Format("Dia {0}/{1}/{2} \nProfissional: {3} ", appointmentDay, appointmentMonth,
				appointmentYear, DataManager.currentResponsible.name);
		}
	}

	List<AppointmentModel> CreateApoointmentList()
	{
		var appointmentList = new List<AppointmentModel>();
		appointmentList.Add(new AppointmentModel(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day,
			(int)DataManager.currentResponsible.timeToBeginWork[(int)DateTime.Now.DayOfWeek], 30, 0), "teste", "Teste", "Teste", "Ocupado"));
		return appointmentList;
	}

	IEnumerator OnButtonClick()
	{
		yield return new WaitForSeconds(0.6f);
		cellList.ForEach(x => x.transform.SetParent(scrollViewContent, false));
		ReadjustScrollSize(limit);
		Loading = false;
	}

	void ReadjustScrollSize(int size)
	{
		scrollViewContent.anchorMax = new Vector2(1, 1);
		scrollViewContent.anchorMin = new Vector2(0, 1);

		scrollViewContent.offsetMax = new Vector2(0, 0);
		var number = (((RectTransform)cellPrefabTransform).rect.height * (size + 1));

		scrollViewContent.offsetMin = new Vector2(0, -number);
	}

	bool isThereAppointmentBeginningAtTime(DateTime time, List<AppointmentModel> appointmentList)
	{
		bool isThere = false;
		int index = 0;
		appointmentList.ForEach(x =>
		{
			if (x.hour == time.Hour && x.minute == time.Minute)
			{
				appointmentIndex = index;
				isThere = true;
			}
			index++;
		});
		return isThere;
	}

	void InitializeScheduleTime()
	{
		var initLunchTime = DataManager.currentResponsible.lunchTime.initTime;
		var endLunchTime = DataManager.currentResponsible.lunchTime.endTime;

		var hour = Mathf.FloorToInt(DataManager.currentResponsible.lunchTime.endTime);
		var minute = (int)((DataManager.currentResponsible.lunchTime.endTime - hour) * 60);

		DateTime lunchTime = new DateTime(appointmentYear, appointmentMonth, appointmentDay, hour, minute, 0);
		var dayOfWeek = (int)DataManager.dateNewAppointment.DayOfWeek;
		limit = Mathf.FloorToInt(DataManager.currentResponsible.timeToFinishWork[dayOfWeek]);
		begin = Mathf.FloorToInt(DataManager.currentResponsible.timeToBeginWork[dayOfWeek]);
		List<AppointmentModel> appointmentList = new List<AppointmentModel>();
		var isOneInOneHour = PlayerPreferences.oneInOneHour;
		int index = 0;
		bool isResponsible = false;
		var newAppointmentDate = new DateTime(appointmentYear, appointmentMonth, appointmentDay);
		CultureInfo provider = new CultureInfo("pt-BR");

		if (DataManager.currentResponsible.userID == DataManager.currentUser.userID || (DataManager.currentUser.userType == Constants.UserType.Company.ToString()))
		{
			isResponsible = true;
		}

		var responsibles = DataManager.responsibleAppointmentList;
		DataManager.responsibleAppointmentList.ForEach(x =>
		{
			if (DateTime.ParseExact(x.data, Constants.dateformat, provider) == newAppointmentDate)
			{
				appointmentList.Add(x);
			}
		});

		if (appointmentList != null)
			appointmentList.Sort((first, second) => ((new DateTime(appointmentYear, appointmentMonth, appointmentDay, first.hour, first.minute, 0)).CompareTo((new DateTime(appointmentYear, appointmentMonth, appointmentDay, second.hour, second.minute, 0)))));

		DataManager.todayResponsibleAppointmentList.Clear();
		DataManager.todayResponsibleAppointmentList = appointmentList;

		if (!isOneInOneHour)
		{
			limit = (limit - begin) * 2;
		}
		var into = false;

		for (var i = 0; i < limit; i++)
		{
			if (isBlockDay || dt.CompareTo(DateTime.Now) < 0 || (dt.Hour >= initLunchTime && (dt.CompareTo(lunchTime) <= 0)))
			{
				var cell = DayController.Instantiate(cellPrefabTransform, dt.Hour.ToString() + ":" + dt.Minute.ToString("00"), "Bloqueado", false);
				if (isFromSchedulePage)
					cell.GetComponent<Button>().interactable = false;
				cellList.Add(cell);
				dt = dt.AddMinutes(isOneInOneHour ? 60 : 30);
			}
			else if (appointmentList != null && index < appointmentList.Count)
			{
				var initAppointmentDateTime = (new DateTime(appointmentYear, appointmentMonth, appointmentDay, appointmentList[appointmentIndex].hour, appointmentList[appointmentIndex].minute, 0));
				var finishAppointmentDateTime = (new DateTime(appointmentYear, appointmentMonth, appointmentDay, appointmentList[appointmentIndex].hour, appointmentList[appointmentIndex].minute, 0)).AddMinutes(appointmentList[index].durationInMinutes);

				if (into)
				{
					if (finishAppointmentDateTime.CompareTo(dt) > 0)
					{
						var description = "";
						if (isResponsible)
						{
							description = appointmentList[appointmentIndex].userName;
						}
						else
						{
							description = "Ocupado";
						}
						var cell = DayController.Instantiate(cellPrefabTransform, dt.Hour.ToString() + ":" + dt.Minute.ToString("00"), description, false);
						cell.GetComponent<Button>().interactable = false;
						cellList.Add(cell);
						into = true;
						dt = dt.AddMinutes(isOneInOneHour ? 60 : 30);
					}
					else
					{
						into = false;
						limit++;
						index++;
					}
				}
				else
				{
					if (isThereAppointmentBeginningAtTime(dt, appointmentList))
					{
						var description = "";
						if (isResponsible)
						{
							description = appointmentList[appointmentIndex].userName;
						}
						else
						{
							description = "Ocupado";
						}
						var cell = DayController.Instantiate(cellPrefabTransform, dt.Hour.ToString() + ":" + dt.Minute.ToString("00"), description, false);
						cell.GetComponent<Button>().interactable = false;
						cellList.Add(cell);
						into = true;
					}
					else
					{
						var cell = DayController.Instantiate(cellPrefabTransform, dt.Hour.ToString() + ":" + dt.Minute.ToString("00"), "Livre");
						if (isFromSchedulePage)
							cell.GetComponent<Button>().interactable = false;
						cellList.Add(cell);
					}
					dt = dt.AddMinutes(isOneInOneHour ? 60 : 30);
				}
			}
			else
			{
				var cell = DayController.Instantiate(cellPrefabTransform, dt.Hour.ToString() + ":" + dt.Minute.ToString("00"), "Livre");
				if (isFromSchedulePage)
					cell.GetComponent<Button>().interactable = false;
				cellList.Add(cell);
				dt = dt.AddMinutes(isOneInOneHour ? 60 : 30);
			}
		}
		StartCoroutine(OnButtonClick());
	}
}
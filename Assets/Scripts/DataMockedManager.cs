using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataMockedManager : MonoBehaviour
{

	public static List<AppointmentModel> appointmentList;
	public static CompanyModel companyData;
	public static List<ResponsableModel> employers = new List<ResponsableModel>();

	void Awake()
	{
		appointmentList = CreateApoointmentList();
		companyData = CreateCompanyData();
	}

	List<AppointmentModel> CreateApoointmentList()
	{
		var appointmentList = new List<AppointmentModel>();
		appointmentList.Add(new AppointmentModel(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, PlayerPreferences.initialTime, 30, 0),
			"teste", "Teste", "Ocupado"));
		return appointmentList;
	}

	CompanyModel CreateCompanyData()
	{
		var company = new CompanyModel("12345", "Empresa");
		var employee1 = new ResponsableModel("1234", "Funcionario 1");
		var employee2 = new ResponsableModel("1235", "Funcionario 2");
		var employee3 = new ResponsableModel("1236", "Funcionario 3");
		var employee4 = new ResponsableModel("1237", "Funcionario 4");
		var employee5 = new ResponsableModel("1238", "Funcionario 5");
		
		employers.Add(employee1);
		employers.Add(employee2);
		employers.Add(employee3);
		employers.Add(employee4);
		employers.Add(employee5);
		
		company.employees.Add("1", employee1);
		company.employees.Add("2", employee2);
		company.employees.Add("3", employee3);
		company.employees.Add("4", employee4);
		company.employees.Add("5", employee5);
		
		return company;
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IMSWEB.Service;
using IMSWEB.Model;
using System.Data;
using Microsoft.Reporting.WebForms;
using IMSWEB.Report.DataSets;

namespace IMSWEB.Report
{
    public class BasicReport : IBasicReport
    {
        DataSet _dataSet = null;
        ReportParameter _reportParameter = null;
        List<ReportParameter> _reportParameters = null;
        IEmployeeService _employeeService;
        ISystemInformationService _systemInformationService;
        ICustomerService _customerService;
        ISupplierService _SupplierService;
        public BasicReport(IEmployeeService employeeService, ICustomerService customerService, ISupplierService supplierService, ISystemInformationService systemInformationService)
        {
            _SupplierService = supplierService;
            _customerService = customerService;
            _employeeService = employeeService;
            _systemInformationService = systemInformationService;
        }

        public async Task<byte[]> EmployeeInformationReport(string userName, int concernID)
        {
            try
            {
                var empInfos = await _employeeService.GetAllEmployeeAsync();
                DataRow row = null;
                BasicDataSet.dtEmployeesInfoDataTable dtEmployeesInfo = new BasicDataSet.dtEmployeesInfoDataTable();

                foreach (var item in empInfos)
                {
                    row = dtEmployeesInfo.NewRow();
                    row["EmpCode"] = item.Item2;
                    row["Name"] = item.Item3;
                    row["Designation"] = item.Item7;
                    row["ContactNo"] = item.Item4;
                    row["JoiningDate"] = item.Item6;

                    dtEmployeesInfo.Rows.Add(row);
                }

                dtEmployeesInfo.TableName = "dtEmployeesInfo";
                _dataSet = new DataSet();
                _dataSet.Tables.Add(dtEmployeesInfo);
                GetCommonParameters(userName, concernID);

                return ReportBase.GenerateBasicReport(_dataSet, _reportParameters, "EmployeeInformation.rdlc");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public byte[] CustomerCategoryWiseDueRpt(string userName, int concernID, int CustomerId, int reportType, int DueType)
        {
            try
            {
                var customerDueInfo = _customerService.CustomerCategoryWiseDueRpt(concernID, CustomerId, reportType, DueType);

                BasicDataSet.dtCustomerDataTable dt = new BasicDataSet.dtCustomerDataTable();
                _dataSet = new DataSet();

                foreach (var grd in customerDueInfo)
                {
                    dt.Rows.Add(grd.Item1, grd.Item2, grd.Item3, grd.Item4, grd.Item5, grd.Item6, grd.Item7,grd.Rest.Item1);
                }

                dt.TableName = "dtCustomer";
                _dataSet.Tables.Add(dt);

                GetCommonParameters(userName, concernID);
                return ReportBase.GenerateBasicReport(_dataSet, _reportParameters, "rptCustomer.rdlc");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public byte[] ConcernWiseSupplierDueRpt(string userName, int concernID, int SupplierId, int reportType)
        {
            try
            {
                IEnumerable<Tuple<string, string, string, string, string, decimal>> supplierDueInfo = _SupplierService.ConcernWiseSupplierDueRpt(concernID, SupplierId, reportType);

                BasicDataSet.dtSupplierDataTable dt = new BasicDataSet.dtSupplierDataTable();
                _dataSet = new DataSet();

                foreach (var grd in supplierDueInfo)
                {
                    dt.Rows.Add(grd.Item1, grd.Item2, grd.Item3, grd.Item4, grd.Item5, grd.Item6);
                }

                dt.TableName = "dtSupplier";
                _dataSet.Tables.Add(dt);

                GetCommonParameters(userName, concernID);
                return ReportBase.GenerateBasicReport(_dataSet, _reportParameters, "rptSupplier.rdlc");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void GetCommonParameters(string userName, int concernID)
        {
            string logoPath = string.Empty;
            SystemInformation currentSystemInfo = _systemInformationService.GetSystemInformationByConcernId(concernID);
            _reportParameters = new List<ReportParameter>();

            _reportParameter = new ReportParameter("Logo", logoPath);
            _reportParameters.Add(_reportParameter);
            _reportParameter = new ReportParameter("CompanyName", currentSystemInfo.Name);
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("Phone", currentSystemInfo.TelephoneNo);
            _reportParameters.Add(_reportParameter);


            _reportParameter = new ReportParameter("Address", currentSystemInfo.Address);
            _reportParameters.Add(_reportParameter);
            _reportParameter = new ReportParameter("PrintedBy", userName);
            _reportParameters.Add(_reportParameter);
        }

        public byte[] GetCustomerDetails(string userName, int concernID, int CustomerID)
        {
            try
            {
                var Customer = _customerService.GetCustomerById(CustomerID);
                DataRow row = null;

                BasicDataSet.dtCustomerDetailsDataTable dt = new BasicDataSet.dtCustomerDetailsDataTable();
                _dataSet = new DataSet();

                row = dt.NewRow();
                row["Code"] = Customer.Code;
                row["Name"] = Customer.Name;
                row["Address"] = Customer.Address;
                row["CompanyName"] = Customer.CompanyName;
                row["ContactNo"] = Customer.ContactNo;
                row["CreditDue"] = 0m; //Customer.CreditDue;
                row["TotalDue"] = Customer.TotalDue;
                row["CustomerType"] = Customer.CustomerType.ToString();
                row["EmailID"] = Customer.EmailID;
                row["FName"] = Customer.FName;
                row["NID"] = Customer.NID;
                row["OpeningDue"] = Customer.OpeningDue;
                row["RefAddress"] = Customer.RefAddress;
                row["RefContactNo"] = Customer.RefContact;
                row["RefFName"] = Customer.RefFName;
                row["RefName"] = Customer.RefName;
                row["Remarks"] = Customer.Remarks;
                dt.Rows.Add(row);

                dt.TableName = "dtCustomerDetails";
                _dataSet.Tables.Add(dt);

                GetCommonParameters(userName, concernID);
                return ReportBase.GenerateBasicReport(_dataSet, _reportParameters, "rptCustomerDetails.rdlc");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}

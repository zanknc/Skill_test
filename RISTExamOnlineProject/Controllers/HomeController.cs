using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using RISTExamOnlineProject.Models.db;
using RISTExamOnlineProject.Models.TSQL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace RISTExamOnlineProject.Controllers
{
    public class HomeController : Controller
    {
        public const string SessionID = "";
        private readonly IConfiguration _configuration;
        private readonly SPTODbContext _sptoDbContext;
        private readonly IHttpContextAccessor httpContextAccessor;
        public HomeController(SPTODbContext context, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        { 
            _sptoDbContext = context;
            _configuration = configuration;
            this.httpContextAccessor = httpContextAccessor;
        }
        [Authorize]
        public IActionResult Index()
        {

            var UserName = User.Identity.Name;
            vewOperatorAlls dataOperator = new vewOperatorAlls();
            dataOperator = _sptoDbContext.vewOperatorAll.FirstOrDefault(x => x.OperatorID == UserName);
            ViewBag.NameEng = dataOperator.NameEng;
            ViewBag.JobTitle = dataOperator.JobTitle;
            var varsd = "http://10.29.1.12/RAJPTrainingControlSystem/PIC/"+ UserName+".jpg";

            string GetSession = HttpContext.Session.GetString(SessionID);

            ViewBag.imgProfile = varsd;
            if (!ModelState.IsValid)
            {
                return RedirectToPage("./Account/Logout");
            }
            return View();
        }
        public JsonResult getDataLicense()
        {

            DataTable dt = new DataTable();
            string Strsql = "SELECT * from vew_CountLicenseTypeGroup";
            var ObjRun = new mgrSQLConnect(_configuration);
            dt = ObjRun.GetDatatables(Strsql);

            return Json(data : dt);
        }

        public JsonResult getDdlDivision()
        {

            DataTable dt = new DataTable();
            string Strsql = "SELECT * FROM [SPTOSystem].[dbo].[vewDivisionMaster] where DivisionID <> 'Z'";
            var ObjRun = new mgrSQLConnect(_configuration);
            dt = ObjRun.GetDatatables(Strsql);

            return Json(data: dt);
        }


        public JsonResult getDataDropdownlist(string type, string value_key)
        {
            DataTable dt = new DataTable();
            if (type == "Division")
            {
                string Strsql = "SELECT * FROM vewDepartmentMaster where row_num ='" + value_key + "'" ;
                var ObjRun = new mgrSQLConnect(_configuration);
                dt = ObjRun.GetDatatables(Strsql);
            }
            
            if(type == "Department")
            {
                string Strsql = "SELECT * FROM vewSectionMaster where row_dept_id ='" + value_key + "'";
                var ObjRun = new mgrSQLConnect(_configuration);
                dt = ObjRun.GetDatatables(Strsql);
            }

            if (type == "Section")
            {
                string Strsql = "SELECT * FROM zImport_License_SectionCode where SectionCode ='" + value_key + "'";
                var ObjRun = new mgrSQLConnect(_configuration);
                dt = ObjRun.GetDatatables(Strsql);
            }
         
            if (type == "License")
            {
                string Strsql = "SELECT * FROM ItemCategory ";
                var ObjRun = new mgrSQLConnect(_configuration);
                dt = ObjRun.GetDatatables(Strsql);
            }

            if (type == "ItemCateg")
            {
                string Strsql = "SELECT * FROM ItemCateg where ItemCateg ='" + value_key + "'";
                var ObjRun = new mgrSQLConnect(_configuration);
                dt = ObjRun.GetDatatables(Strsql);
                

            }

            if (type == "ItemCode")
            {
                string Strsql = "SELECT * FROM vewItemCodeList where ItemCateg ='" + value_key + "'";
                var ObjRun = new mgrSQLConnect(_configuration);
                dt = ObjRun.GetDatatables(Strsql);


            }
            if(type == "ItemCodeName")
            {
                string Strsql = "SELECT * FROM vewItemCategPlanMode where ItemCode ='" + value_key + "'";
                var ObjRun = new mgrSQLConnect(_configuration);
                dt = ObjRun.GetDatatables(Strsql);
            }
            if (type == "ItemCategName")
            {
                string Strsql = "SELECT * FROM ItemCategory where ItemCateg ='" + value_key + "'";
                var ObjRun = new mgrSQLConnect(_configuration);
                dt = ObjRun.GetDatatables(Strsql);
            }
            






            return Json(data: dt);
        }

        public JsonResult Get_SummaryLicenseDiv()
        {
            List<vewLicenseMaster> listItems = new List<vewLicenseMaster>();

            var ObjRun = new mgrSQLConnect(_configuration);
            DataTable data_licenGroup = new DataTable();
            string strLicense_group = "select License_Type from RTM.dbo.Plan_MasterDetails group by License_Type";



            data_licenGroup = ObjRun.GetDatatables(strLicense_group);
            string[] licenGroup = new string[data_licenGroup.Rows.Count];
            int l = 0;
            foreach (DataRow dr in data_licenGroup.Rows)
            {
                licenGroup[l++] = dr[0].ToString();
            }

            //listItems.Add(tempList);
            string[] Division_name = new string[] { "TR/di", "MCR", "OPM", "LAPIS", "LSI" };
            string[] taaa = new string[] { };
          
            var data_list = new ArrayList();
            var arry_license = new ArrayList();
            List<vewLicenseMaster> Ls = new List<vewLicenseMaster>();
            DataTable dt = new DataTable();
            var arlist2 = new ArrayList();
            for (int n = 0; n < licenGroup.Length; n++)
            {
                var data_list2 = new ArrayList();

                for (int i = 0; i < Division_name.Length; i++)
                {
                    
                    string sql_Tr = "select count(License_Type) as TRDI , License_Type from vewSummaryLicenseDivision where division in" +
                                    " (select division from vewOperatorAll where division like '%" + Division_name[i] +"%' group by division) " +
                                       "and License_Type = '" + licenGroup[n] +"' group by License_Type";
                    dt = ObjRun.GetDatatables(sql_Tr);
                    arry_license = new ArrayList() { dt.Rows.OfType<DataRow>().Select(k => k[0].ToString()).ToArray(), Division_name[i], licenGroup[n] };
                    data_list2.Add(arry_license);
                }
               

                data_list.Add(data_list2);

            }

            var jsonResult = Json(new { 
                data = data_list,
                licenGroup = licenGroup
              
            });
            return Json(jsonResult);
        }
        public JsonResult Count_Operator_License()
        {
            DataTable dt = new DataTable();
            string Strsql = "SELECT * FROM vew_countLicense";
            var ObjRun = new mgrSQLConnect(_configuration);
            dt = ObjRun.GetDatatables(Strsql);
            return Json(data: dt);
        }
        public JsonResult Search_DashboardDetail(string DivisionValue, string sectionValue , string LicenseValue, string ItemCodeValue, string itemNameValue, string ItemCategNameValue , string ItemCategValue , string DepartmentsValue)
        {


            return Json("");
        }
        public JsonResult Get_licenseReview()
        {
            var ObjRun = new mgrSQLConnect(_configuration);
            DataTable dt_UsedLicense = new DataTable();
            DataTable dt_NeverUseLicense = new DataTable();
            string sql_UsedLicense = "SELECT * FROM vewCountLicenseAllTest";
            string sql_NeverUseLicense = "SELECT * FROM vewCountLicenseNotTest";

            dt_UsedLicense = ObjRun.GetDatatables(sql_UsedLicense);
            dt_NeverUseLicense = ObjRun.GetDatatables(sql_NeverUseLicense);


            var JsonResults = Json(new { License_Used = dt_UsedLicense , License_NotUsed = dt_NeverUseLicense });
            return JsonResults;

        }

        public JsonResult Get_Datatable_Exam()
        {
            DataTable dt = new DataTable();
            string Strsql = "SELECT * FROM vew_ExamResultFull";
            var ObjRun = new mgrSQLConnect(_configuration);
            dt = ObjRun.GetDatatables(Strsql);
            return Json(data : dt);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return RedirectToPage("./Account/Logout");
            }

            //_context.Customer.Add(Customer);
            //await _context.SaveChangesAsync();
            //Message = $"Customer {Customer.Name} added";

            return RedirectToPage("./Home/Dashboard_Detail");
        }
        public IActionResult Dashboard_Detail()
        {
           
            return View();
        }

        public JsonResult Profile(string OPID)
        {
            var UserName = OPID;
             
            vewOperatorAlls dataOperator = new vewOperatorAlls();
            dataOperator = _sptoDbContext.vewOperatorAll.FirstOrDefault(x => x.OperatorID == UserName);
            var Name = dataOperator.NameEng;
            var JobTitle = dataOperator.JobTitle;
            var varsd = "http://10.29.1.12/RAJPTrainingControlSystem/PIC/" + UserName + ".jpg";

            var jsonResult = Json(new{ data=varsd ,strName = Name, strJobTitle = JobTitle } );

            return jsonResult;
        }
    }
}
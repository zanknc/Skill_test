using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using RISTExamOnlineProject.Models.db;
using RISTExamOnlineProject.Models.TSQL;
using System;
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
            var ObjRun = new mgrSQLConnect(_configuration);
            //------------------- TRDI--------------------------------------
            DataTable dt_trdi = new DataTable();
            string sql_Tr = "select count(License_Type) as TRDI , License_Type from vewSummaryLicenseDivision where division in " +
                            "(select division from vewOperatorAll where division like '%TR/Di%' group by division) group by License_Type";
            dt_trdi = ObjRun.GetDatatables(sql_Tr);

            //------------------- MCR--------------------------------------
            DataTable dt_MCR = new DataTable();
            string sql_MCR = "select count(License_Type) as TRDI , License_Type from vewSummaryLicenseDivision where division in " +
                            "(select division from vewOperatorAll where division like '%MCR%' group by division) group by License_Type";
            dt_MCR = ObjRun.GetDatatables(sql_MCR);

            //------------------- OPM--------------------------------------
            DataTable dt_OPM = new DataTable();
            string sql_OPM = "select count(License_Type) as TRDI , License_Type from vewSummaryLicenseDivision where division in " +
                            "(select division from vewOperatorAll where division like '%OPM%' group by division) group by License_Type";
            dt_OPM = ObjRun.GetDatatables(sql_OPM);


            //------------------- LAPIS--------------------------------------
            DataTable dt_LAPIS = new DataTable();
            string sql_LAPIS = "select count(License_Type) as TRDI , License_Type from vewSummaryLicenseDivision where division in " +
                            "(select division from vewOperatorAll where division like '%LAPIS%' group by division) group by License_Type";
            dt_LAPIS = ObjRun.GetDatatables(sql_LAPIS);

            //------------------- LSI--------------------------------------
            DataTable dt_LSI = new DataTable();
            string sql_LSI = "select count(License_Type) as TRDI , License_Type from vewSummaryLicenseDivision where division in " +
                            "(select division from vewOperatorAll where division like '%LSI%' group by division) group by License_Type";
            dt_LSI = ObjRun.GetDatatables(sql_LSI);

            var jsonResult = Json(new { TR =  dt_trdi , MCR = dt_MCR, OPM = dt_OPM , LAPIS =  dt_LAPIS , LSI = dt_LSI });
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
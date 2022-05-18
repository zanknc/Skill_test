using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RISTExamOnlineProject.Models.db;
using RISTExamOnlineProject.Models.TSQL;

namespace RISTExamOnlineProject.Controllers
{
    public class AccountController : Controller
    {
        public const string SessionID = "";
        public const string SessionDivision = "";
        public const string SessionName = "";
        private readonly SPTODbContext _sptoDbContext;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor httpContextAccessor;
       
        public AccountController(SPTODbContext context, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _sptoDbContext = context;
            _configuration = configuration;
            this.httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]

        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }
        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            return RedirectToAction(nameof(HomeController.Index), "Home");

        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Login(UserLoginModel userModel, string returnUrl = null)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(userModel);
        //    }

        //    var result = await _signInManager.PasswordSignInAsync(userModel.Email, userModel.Password, userModel.RememberMe, false);
        //    if (result.Succeeded)
        //    {
        //        return RedirectToLocal(returnUrl);
        //    }
        //    else
        //    {
        //        ModelState.AddModelError("", "Invalid UserName or Password");
        //        return View();
        //    }
        //}

        [HttpPost]
        public async Task<IActionResult> Login(string OperatorID, string Password, vewOperatorAlls model, string returnUrl = null)
        {
            if (!ModelState.IsValid) return View();

            if (!string.IsNullOrEmpty(OperatorID) && string.IsNullOrEmpty(Password)) return RedirectToAction("Login");

            //Check the user name and password
            //Here can be implemented checking logic from the database
            ClaimsIdentity identity = null;
            var isAuthenticated = false;
            string authority = null;
            var active = false;
            var querylogin = await _sptoDbContext.vewOperatorAll
                .Where(x => x.OperatorID == OperatorID && x.Password == Password)
                .Select(c => new {c.OperatorID, c.Active, c.Authority}).FirstOrDefaultAsync();


            if (querylogin != null)
            {
                //foreach (var item in querylogin)
                //{
                //    authority = item.Authority;
                //    active = item.Active;
                //}

                authority = querylogin.Authority;
                active = querylogin.Active;

                if (active == false) return RedirectToAction("Login");

                switch (authority)
                {
                    case "9":
                        //Create the identity for the Admin
                        identity = new ClaimsIdentity(new[]
                        {
                            new Claim(ClaimTypes.Name, model.OperatorID),

                            new Claim(ClaimTypes.Role, "Admin"),
                        }, CookieAuthenticationDefaults.AuthenticationScheme);

                        isAuthenticated = true;
                        break;
                    case "7":
                        //Create the identity for the Training
                        identity = new ClaimsIdentity(new[]
                        {
                            new Claim(ClaimTypes.Name, model.OperatorID),           
                            new Claim(ClaimTypes.Role, "Training")
                        }, CookieAuthenticationDefaults.AuthenticationScheme);

                        isAuthenticated = true;
                        break;
                    case "0":
                        //Create the identity for the user
                        identity = new ClaimsIdentity(new[]
                        {
                            new Claim(ClaimTypes.Name, model.OperatorID),                   
                            new Claim(ClaimTypes.Role, "User")
                        }, CookieAuthenticationDefaults.AuthenticationScheme);

                        isAuthenticated = true;
                        break;
                }
                string status_sprInsert = "1";
                DataTable dt = new DataTable();
                string Strsql = "EXEC sprInsertOperatorOnline " + "'" + OperatorID + "', '" + status_sprInsert + "'";
                var ObjRun = new mgrSQLConnect(_configuration);
                dt = ObjRun.GetDatatables(Strsql);
               
                HttpContext.Session.SetString(SessionID, OperatorID);
            }
            else
            {
                ModelState.AddModelError("", "Invalid Operator No. or Password");
                return View();
            }
           
            


             if (!isAuthenticated) return View();
            var principal = new ClaimsPrincipal(identity);

            var login = HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            return RedirectToLocal(returnUrl);
            //return RedirectToAction("Index", "Home");
        
        }
        public string InsertOperatorOnline()
        {
            string statusInsert = "";
            string Get_sessionID = HttpContext.Session.GetString(SessionID);
            if (Get_sessionID != "")
            {
                
                string status_sprInsert = "1";
                DataTable dt = new DataTable();
                string Strsql = "EXEC sprInsertOperatorOnline " + "'" + Get_sessionID + "', '" + status_sprInsert + "'";
                var ObjRun = new mgrSQLConnect(_configuration);
                dt = ObjRun.GetDatatables(Strsql);
            }
            return statusInsert;
        }
        public string DeleteOperatorOnline()
        {
            string statusInsert = "";
            string Get_sessionID = HttpContext.Session.GetString(SessionID);
            if (Get_sessionID != "")
            {
                
                string status_sprInsert = "2";
                DataTable dt = new DataTable();
                string Strsql = "EXEC sprInsertOperatorOnline " + "'" + Get_sessionID + "', '" + status_sprInsert + "'";
                var ObjRun = new mgrSQLConnect(_configuration);
                dt = ObjRun.GetDatatables(Strsql);
            }

            return statusInsert;
        }

        public IActionResult Logout()
        {
            string Get_sessionID = HttpContext.Session.GetString(SessionID);
            if (Get_sessionID != "")
            {
                string status_sprInsert = "2";
                DataTable dt = new DataTable();
                string Strsql = "EXEC sprInsertOperatorOnline " + "'" + Get_sessionID + "', '" + status_sprInsert + "'";
                var ObjRun = new mgrSQLConnect(_configuration);
                dt = ObjRun.GetDatatables(Strsql);
            }
            var login = HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}
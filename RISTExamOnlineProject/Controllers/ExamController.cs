﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;

using System.Xml;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RISTExamOnlineProject.Models.db;
using RISTExamOnlineProject.Models.TSQL;

using Microsoft.AspNetCore.Hosting.Server;

namespace RISTExamOnlineProject.Controllers
{
    public class ExamController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly SPTODbContext _sptoDbContext;
        private readonly IHttpContextAccessor httpContextAccessor;


        public ExamController(SPTODbContext context, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {

            _sptoDbContext = context;
            _configuration = configuration;
            this.httpContextAccessor = httpContextAccessor;

        }





        public IActionResult New_Exam()
        {
            return View();
        }
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }


        [Authorize]
        public IActionResult Exam_maintenance(string Itemcode)
        {

            ViewBag.Itemcode = Itemcode;
            return View();

        }


        [Authorize]
        public IActionResult Examination(string Itemcode)
        {


            ViewBag.Itemcode = Itemcode;

            return View();

        }

        [Authorize]
        public IActionResult GetExamDetail(string Itemcode)
        {
            DataTable dt = new DataTable();
            string ValueCodeQuestion = "";
            string ValueCodeAnswer = "";
            int QuestionCount = 0;

            string ItemName;
            int Max_Seq;
            int Rewrite_ValueList = 0;
            int Rewrite_Master = 0;
            string UpdDate = "";
            mgrSQLcommand_Exam ObjRun = new mgrSQLcommand_Exam(_configuration);


            //------------------ Get Rewrite Action  -------------------------
            dt = ObjRun.InputItem_Detail(Itemcode);
            if (dt.Rows.Count != 0)
            {
                Rewrite_Master = Convert.ToInt16(dt.Rows[0]["Rewrite"].ToString());
                ValueCodeQuestion = dt.Rows[0]["ValueCodeQuestion"].ToString();
                ValueCodeAnswer = dt.Rows[0]["ValueCodeAnswer"].ToString();
                UpdDate = dt.Rows[0]["UpdDate"].ToString();
            }
            //else { 

            // }

            //----------------------------------------------------------------


            dt = ObjRun.Get_ExamDetail(Itemcode);

            if (dt.Rows.Count != 0)
            {
                Max_Seq = Convert.ToInt16(dt.Rows[0]["Max_Seq"].ToString());
                //  Rewrite_ValueList = Convert.ToInt16(dt.Rows[0]["Rewrite_ValueList"].ToString());
                //   Rewrite_Master = Convert.ToInt16(dt.Rows[0]["Rewrite_Master"].ToString());
                UpdDate = dt.Rows[0]["UpdDate"].ToString();
                List<Exam_QuestionDetail> Detail = new List<Exam_QuestionDetail>();

                if (Max_Seq != 0)
                {

                    foreach (DataRow row in dt.Rows)
                    {
                        Detail.Add(new Exam_QuestionDetail()
                        {
                            ItemCode = row["ItemCode"].ToString(),
                            ItemCategName = row["ItemCategName"].ToString(),
                            ValueCodeQuestion = row["ValueCodeQuestion"].ToString(),
                            ValueCodeAnswer = row["ValueCodeAnswer"].ToString(),
                            Seq = Convert.ToInt16(row["Seq"].ToString()),
                            Question = row["Question"].ToString(),
                            Ans_Count = row["Ans_Count"].ToString(),
                            Max_Seq = row["Max_Seq"].ToString(),
                            ValueStatus = row["ValueStatus"].ToString(),

                        });

                    }
                }



                //  ValueCodeQuestion = dt.Rows[0]["ValueCodeQuestion"].ToString();
                // ValueCodeAnswer = dt.Rows[0]["ValueCodeAnswer"].ToString();
                dt = ObjRun.Get_ValueCount(ValueCodeQuestion);

                //   QuestionCount = Convert.ToInt32(dt.Rows.Count);         



                ItemName = dt.Rows[0]["ItemName"].ToString();
                ItemName = Itemcode + "-" + ItemName;

                if (Max_Seq == 0)
                {
                    QuestionCount = 0;

                }
                else
                {
                    QuestionCount = Convert.ToInt32(dt.Rows.Count);

                }


                return Json(new
                {
                    success = true,
                    ValueCodeQuestion = ValueCodeQuestion,
                    ValueCodeAnswer = ValueCodeAnswer,
                    QuestionCount = QuestionCount,
                    Max_Seq = Max_Seq,
                    ItemName = ItemName,
                    Detail = Detail,
                    Rewrite_ValueList = Rewrite_ValueList,
                    Rewrite_Master = Rewrite_Master,
                    UpdDate = UpdDate
                });



            }
            else
            {
                return Json(new { success = false, Rewrite_Master = Rewrite_Master, ValueCodeQuestion = ValueCodeQuestion, ValueCodeAnswer = ValueCodeAnswer, UpdDate = UpdDate });
            }
        }

        [Authorize]
        public IActionResult GetHTML(string ItemCateg, string ItemCode)
        {
            mgrSQLcommand_Exam ObjRun = new mgrSQLcommand_Exam(_configuration);
            string HTMLTEXT = ObjRun.GetExamHTML(ItemCateg, ItemCode);
            return Json(new { success = true, HTMLTEXT = HTMLTEXT });
        }


        [Authorize]
        public IActionResult GetCategType()
        {
            mgrSQLcommand_Exam ObjRun = new mgrSQLcommand_Exam(_configuration);
            List<SelectListItem> listItems = new List<SelectListItem>();

            string Strsql = " SELECT ItemCategType as TextValues ,ItemCategType as Values_  from [SPTOSystem].[dbo].[ItemCategory]  group by ItemCategType order by ItemCategType asc";
            listItems = ObjRun.GetItemDropDownList(Strsql, "Category Type");
            return Json(new SelectList(listItems, "Value", "Text"));

        }


        [Authorize]
        public IActionResult GetCategory(string CategoryType)
        {

            mgrSQLcommand_Exam ObjRun = new mgrSQLcommand_Exam(_configuration);
            List<SelectListItem> listItems = new List<SelectListItem>();
            string Strsql = "  SELECT  ItemCateg +' - '+[ItemCategName],[ItemCateg] FROM[SPTOSystem].[dbo].[vewQuestionCateg]  where [ItemCategType] ='" + CategoryType.Trim() + "' group by[ItemCateg],[ItemCategName] order by ItemCateg asc";
            listItems = ObjRun.GetItemDropDownList(Strsql, "Category");
            return Json(new SelectList(listItems, "Value", "Text"));

        }

        [Authorize]
        public IActionResult GetExamname(string Category)
        {
            mgrSQLcommand_Exam ObjRun = new mgrSQLcommand_Exam(_configuration);
            List<SelectListItem> listItems = new List<SelectListItem>();
            string Strsql = "SELECT    [ItemCode] +' - '+ [ItemName] ,ItemCode  FROM [SPTOSystem].[dbo].[vewQuestionCateg] where[ItemCateg] = '" + Category + "' group by[ItemName],[ItemCode]";
            listItems = ObjRun.GetItemDropDownList(Strsql, "Exam");
            return Json(new SelectList(listItems, "Value", "Text"));
        }



        public IActionResult Test()
        {


            return View();

        }



        public string CheckBase64FileType(string PictrueText)
        {
            // var data = PictrueText.Substring(23, 5);

            string data = PictrueText.Substring(0, 20);

            data = PictrueText.Substring(11, PictrueText.IndexOf(";") - 11);

            switch (data)
            {

                case "gif":
                    return data;
                case "png":
                    return data;
                case "jpeg":
                    return data;
                case "jpg":
                    return data;




                //case "0LGOD":
                //    return "gif";
                //case "VBORW":
                //    return "png";
                //case "/9J/4":
                //    return "jpeg";
                //case "AAAAF":
                //    return "mp4";
                //case "JVBER":
                //    return "pdf";
                //case "AAABA":
                //    return "ico";
                //case "UMFYI":
                //    return "rar";
                //case "E1XYD":
                //    return "rtf";
                //case "U1PKC":
                //    return "txt";
                //case "MQOWM":
                //case "77U/M":
                //    return "srt";


                default:
                    return "";
            }

        }



        public string SavePictrue(string TextHTML, string PictrueText)
        {

            if (PictrueText.Substring(0, 4) == "data")
            {


                string FileType = CheckBase64FileType(PictrueText);
                if (FileType != "")
                {
                    try
                    {
                        //  string Root = @"wwwroot\lib\IMG_For_Exam"; // Path
                        string Root = @"\\10.29.1.116\G$\TEC_Train_Dept\Program\TestSkill_Image";
                        string fileName = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + "." + FileType;  // Make File Name Random
                        TextHTML = TextHTML.Replace(PictrueText, @"http://10.29.1.116/SkillTestPicture/TestSkill_Image/" + fileName);   //---------- Replase
                        PictrueText = PictrueText.Replace("data:image/" + FileType + ";base64,", "");                    //------- Set file for convert                               

                        //--------- Convert File ----------------
                        var base64string = PictrueText;
                        var base64array = Convert.FromBase64String(base64string);
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), Root, fileName);
                        // Save
                        System.IO.File.WriteAllBytes(filePath, base64array);
                    }
                    catch (Exception ex)
                    {
                        return ex.Message.ToString();
                    }
                }
            }

            return TextHTML;
        }





        [Authorize]
        public IActionResult Valueslist(int Max_Seq, int QuestionCount, string ValueCodeQuestion, string ValueCodeAnswer, string[] Ans_TextDisplay, string[] Ans_Text_HTML_Display
          , string[] Ans_Value, string Need_value, string Text_Question, string TextHTML_Question, string Job, string OP_UPD, int DisplayOrder, int Rewrite_Master,
           string[][] Ans_Picture, string[] Question_Picture)
        {

            mgrSQLcommand_Exam ObjRun = new mgrSQLcommand_Exam(_configuration);


            string IP = Request.HttpContext.Connection.RemoteIpAddress.ToString();
            string MS;



            try
            {
                //----------------------------  Save File  Question ---------------------------
                if (Question_Picture.Length > 0)
                {
                    for (int i = 0; i <= Question_Picture.Length - 1; i++)
                    {
                        TextHTML_Question = SavePictrue(TextHTML_Question, Question_Picture[i]);
                    }


                }

                for (int x = 0; x <= Ans_Picture.Length - 1; x++)
                {

                    int Count = Convert.ToInt32(Ans_Picture[x][0]);
                    string CodeBase64 = Ans_Picture[x][1].ToString();
                    Ans_Text_HTML_Display[Count] = SavePictrue(Ans_Text_HTML_Display[Count], CodeBase64);



                }

                //  }





                //   string myuuidAsString = myuuid.ToString();

                //----------------------------






                if (Job == "DEL" || Job == "RES" || Job == "REJ")
                {
                    ObjRun.Valueslist_Management(Job, "", DisplayOrder, "", "", "0", "", IP, OP_UPD, ValueCodeQuestion.Trim(), ValueCodeAnswer.Trim(), Rewrite_Master);
                }
                else
                {
                    if (Job == "UPD")
                    {
                        ObjRun.Valueslist_Management("BK", "", DisplayOrder, "", "", "0", "", IP, OP_UPD, ValueCodeQuestion.Trim(), ValueCodeAnswer.Trim(), Rewrite_Master);
                        Max_Seq = DisplayOrder;
                    }
                    else
                    {
                        Max_Seq = Max_Seq + 1;
                    }

                    //----------- inseart Qeustion ----  
                    MS = ObjRun.Valueslist_Management(Job, ValueCodeQuestion, Max_Seq, TextHTML_Question, Text_Question, "0", Need_value, IP, OP_UPD, "", "", Rewrite_Master);
                    if (MS != "OK")
                    {
                        return Json(new { success = false, responseText = MS });
                    }
                    //----------- inseart Anser ----

                    for (int i = 0; i < Ans_TextDisplay.Length; i++)
                    {
                        string TextDisplay;
                        if (Ans_TextDisplay[i] == null)
                        {
                            TextDisplay = "";
                        }
                        else
                        {
                            TextDisplay = Ans_TextDisplay[i].Trim();
                        }

                        MS = ObjRun.Valueslist_Management(Job, ValueCodeAnswer, Max_Seq, Ans_Text_HTML_Display[i].Trim(), TextDisplay, Ans_Value[i].Trim(), "0", IP, OP_UPD, "", "", Rewrite_Master);


                        if (MS != "OK")
                        {
                            return Json(new { success = false, responseText = MS });
                        }
                    }
                }



            }
            catch (Exception ex)
            {
                return Json(new { success = false, responseText = ex.Message.ToString() });
                throw;
            }



            return Json(new { success = true });

        }
        public JsonResult Get_HTML_Question_Detail(string ValueCodeAnswer, string ValueCodeQuestion, int Seq, string Job)
        {
            mgrSQLcommand_Exam ObjRun = new mgrSQLcommand_Exam(_configuration);
            string HTML_Text = ObjRun.HTML_Question_Detail(ValueCodeQuestion, ValueCodeAnswer, Seq, Job);
            return Json(new { success = true, HTML = HTML_Text });
        }
















        //----------------------------------------------- Exam Approved -------------------------------------



        [Authorize]
        public ActionResult Exam_Approved()
        {

            return View();
        }




        [HttpPost]


        [Authorize]
        public IActionResult GetCategoryType_Approved()
        {
            mgrSQLcommand_Exam ObjRun = new mgrSQLcommand_Exam(_configuration);
            List<SelectListItem> listItems = new List<SelectListItem>();
            string Strsql = "select DISTINCT [ItemCategType] as Text    ,[ItemCategType]    as Values_   FROM [SPTOSystem].[dbo].[vewExamApproved_New]";
            listItems = ObjRun.GetItemDropDownList(Strsql, "Category ");
            return Json(new SelectList(listItems, "Value", "Text"));

        }

        [Authorize]
        public IActionResult GetCategory_Approved(string ItemCategType)
        {
            mgrSQLcommand_Exam ObjRun = new mgrSQLcommand_Exam(_configuration);
            List<SelectListItem> listItems = new List<SelectListItem>();
            string Strsql = "select DISTINCT [ItemCateg] + ' - ' + [ItemCategName] as [ItemCategName]      ,[ItemCateg]        FROM [SPTOSystem].[dbo].[vewExamApproved_New] where [ItemCategType] = '" + ItemCategType.Trim() + "'";
            listItems = ObjRun.GetItemDropDownList(Strsql, "Category ");
            return Json(new SelectList(listItems, "Value", "Text"));

        }

        [Authorize]
        public IActionResult GetExamname_Approved(string Category)
        {
            mgrSQLcommand_Exam ObjRun = new mgrSQLcommand_Exam(_configuration);
            List<SelectListItem> listItems = new List<SelectListItem>();
            string Strsql = " select DISTINCT  [ItemName],[ValueCodeQuestion]+'-' +[ValueCodeAnswer] FROM [SPTOSystem].[dbo].[vewExamApproved_New] where[ItemCateg] = '" + Category + "' ";
            listItems = ObjRun.GetItemDropDownList(Strsql, "Exam ");
            return Json(new SelectList(listItems, "Value", "Text"));

        }
        [Authorize]
        [HttpPost]
        public JsonResult Approved_Detail(string ValueCodeQuestion)
        {

            DataTable dt = new DataTable();
            mgrSQLcommand_Exam ObjRun = new mgrSQLcommand_Exam(_configuration);
            List<ExamApproved_Detail> Detail = new List<ExamApproved_Detail>();
            var draw = HttpContext.Request.Form["draw"].FirstOrDefault();
            var start = Request.Form["start"].FirstOrDefault();
            var length = Request.Form["length"].FirstOrDefault();
            var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"]
                .FirstOrDefault();
            var sortColumnDir = Request.Form["order[0][dir]"].FirstOrDefault();
            var searchValue = Request.Form["search[value]"].FirstOrDefault();
            var pageSize = length != null ? Convert.ToInt32(length) : 10;
            var skip = start != null ? Convert.ToInt32(start) : 0;
            var recordsTotal = 0;

            Detail = ObjRun.Get_ExamDetail_Approved(ValueCodeQuestion);
            var data = Detail.ToList();
            recordsTotal = data.Count();
            //  string Rewrite_Master = Detail[0].Rewrite_Master.ToString();

            // return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data ,Rewrite_Master = Rewrite_Master });
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });




        }
        [Authorize]

        public JsonResult View_QuestionDetail(int seq, string ValueCodeQuestion, string ValueCodeAnswer, string ValueStatus)
        {
            string StrHTML = "";
            mgrSQLcommand_Exam ObjRun = new mgrSQLcommand_Exam(_configuration);
            StrHTML = ObjRun.View_Question(seq, ValueCodeQuestion, ValueCodeAnswer, ValueStatus);

            if (StrHTML != "")
            {

                return Json(new { success = true, responseText = StrHTML });

            }
            else
            {
                return Json(new { success = false });
            }


        }



        [Authorize]
        public JsonResult Job_Reject_And_Approved(string Job, string valueStatus_Array, string seq_Array, string valueCodeQuestion, int Rewrite_Master)
        {




            mgrSQLcommand_Exam ObjRun = new mgrSQLcommand_Exam(_configuration);

            string ms;
            string TextAlert;

            if (Job == "APP") TextAlert = "Approved"; else TextAlert = "Reject";



            ms = ObjRun.Approved_Reject_Question_(Job, valueStatus_Array, seq_Array, valueCodeQuestion, Rewrite_Master);


            if (ms == "OK")
            {
                return Json(new { success = true, textresponse = "The question was successfully " + TextAlert + " " });
            }
            else
            {
                return Json(new { success = false, textresponse = TextAlert + "Error" });
            }

            //--------------  if 






        }

    }
}
﻿using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using RISTExamOnlineProject.Models.db;

namespace RISTExamOnlineProject.Models.TSQL
{
    public class mgrSQLcommand
    {
        private readonly IConfiguration _configuration;
        private DataSet ds = new DataSet();
        private DataTable dt = new DataTable();
        private string strSQL = "";

        public mgrSQLcommand(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public DataTable GetSectionCode(string strDivision, string strDepartment)
        {
            var ObjRun = new mgrSQLConnect(_configuration);
            dt = new DataTable();
            strSQL = "";
            int Chk = 0;
            strSQL += "SELECT [SectionCode],[Section],[Department],[Division]";
            strSQL += "FROM [SPTOSystem].[dbo].[vewT_Section_Master] ";
            if (strDivision != "" && strDivision != null)
            {
                strSQL += Chk == 0 ? " Where  " : " and  ";

                strSQL += "Substring(sectionCode,1,1) = '" + strDivision + "'";
                Chk++;
            }
            if (strDepartment != "" && strDepartment != null)
            {
                strSQL += Chk == 0 ? " Where  " : " and  ";

                strSQL += "Substring(sectionCode,1,2) = '" + strDepartment + "'";
                Chk++;
            }
            strSQL += "group by  [SectionCode],[Section],[Department],[Division]  ";
            dt = ObjRun.GetDatatables(strSQL);
            return dt;
        }

        public DataTable GetDepartment(string strDivision)
        {
            mgrSQLConnect ObjRun = new mgrSQLConnect(_configuration);
            dt = new DataTable();
            strSQL = "";
            int Chk = 0;
            strSQL += "SELECT Substring(sectionCode,1,2) as sectionCode , [Department]" +
                "FROM [SPTOSystem].[dbo].[vewT_Section_Master] ";

            if (strDivision != "" && strDivision != null)
            {
                strSQL += Chk == 0 ? " Where  " : " and  ";
                strSQL += "Substring(sectionCode,1,1) = '" + strDivision + "'";
                Chk++;
            }
            strSQL += "group by Substring(sectionCode,1,2),[Department] ";

            dt = ObjRun.GetDatatables(strSQL);

            return dt;
        }
        public DataTable GetDivision()
        {
            mgrSQLConnect ObjRun = new mgrSQLConnect(_configuration);
            dt = new DataTable();
            strSQL = "";

            strSQL += "SELECT Substring(sectionCode,1,1) as sectionCode  ,[Division] " +
                "FROM [SPTOSystem].[dbo].[vewT_Section_Master] ";
            strSQL += " group by Substring(sectionCode,1,1) ,[Division]  ";

            dt = ObjRun.GetDatatables(strSQL);

            return dt;
        }

        public DataTable GetGroupName()
        {

            mgrSQLConnect ObjRun = new mgrSQLConnect(_configuration);
            dt = new DataTable();
            strSQL = "";

            strSQL += "SELECT [OperatorGroup] ,[GroupName] FROM [SPTOSystem].[dbo].[OperatorGroup]";

            dt = ObjRun.GetDatatables(strSQL);

            return dt;
        }


        public List<vewOperatorLicense> GetUserLicense(string Opid)
        {

            mgrSQLConnect ObjRun = new mgrSQLConnect(_configuration);
            List<vewOperatorLicense> dataList = new List<vewOperatorLicense>();
            dt = new DataTable();
            strSQL = "";
            strSQL += "SELECT * FROM [SPTOSystem].[dbo].vewOperatorLicense  where OperatorID ='" + Opid + "'";
            dt = ObjRun.GetDatatables(strSQL);
            if (dt.Rows.Count != 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    dataList.Add(new vewOperatorLicense()
                    {
                        OperatorID = row["OperatorID"].ToString().Trim(),
                        License = row["License"].ToString().Trim(),
                    });
                }
            }
            return dataList;

        }

        public string[] GetUpdUserdetail(vewOperatorAlls _Data, List<vewOperatorLicense> _DataLicense, string OpNo, string strIpAddress)
        {
            mgrSQLConnect ObjRun = new mgrSQLConnect(_configuration);
            dt = new DataTable();
            string DataMgs, strFlag;
            var results = true;
            strSQL = "";
            string[] Result;
            strFlag = "UPD";
            try
            {
                string DataLicense = "";
                foreach (vewOperatorLicense i in _DataLicense)
                {
                    DataLicense += ";" + i.License;

                }


                strSQL += "Exec [sprOperator]";
                strSQL += "'" + strFlag + "',";                                              //flag
                strSQL += "'" + _Data.OperatorID + "',";
                strSQL += "'" + _Data.Password + "',";
                strSQL += "'" + _Data.NameEng + "',";
                strSQL += "N'" + _Data.NameThai + "',";
                strSQL += "'" + _Data.SectionCode + "',";
                strSQL += "'" + _Data.OperatorGroup + "',";
                strSQL += "'" + _Data.Position + "',";
                strSQL += "'" + _Data.JobTitle + "',";
                strSQL += "'" + _Data.Email1 + "',";
                strSQL += "'" + _Data.Email2 + "',";
                strSQL += "'" + _Data.RFID + "',";
                strSQL += "'" + _Data.Authority + "',";
                strSQL += "'" + _Data.Active + "',";
                strSQL += "'" + OpNo + "',";
                strSQL += "'" + strIpAddress + "';";





                strSQL += "   Exec [sprOperatorLicense]";
                strSQL += "'" + _Data.OperatorID + "'";
                strSQL += ",'" + DataLicense.Substring(1) + "'";
                strSQL += ",'" + OpNo + "'";
                strSQL += ",'" + strIpAddress + "';";



                dt = ObjRun.GetDatatables(strSQL);
                if (dt.Rows.Count > 0)
                {
                    Result = new[] { dt.Rows[0][0].ToString(), dt.Rows[0][1].ToString() };
                }
                else
                {
                    results = false;
                    Result = new[] { results.ToString(), "Error " };
                }
            }
            catch (Exception e)
            {
                DataMgs = e.Message + ":" + strSQL;
                results = false;
                Result = new[] { results.ToString(), DataMgs };
            }
            return Result;
        }



        public ResultItemCateg GetOperatorItemCateg(string Opid)
        {
            dt = new DataTable();
            mgrSQLConnect ObjRun = new mgrSQLConnect(_configuration);
            ResultItemCateg ResultOPcateg = new ResultItemCateg();
            List<_OperatorItemCateg> dataList = new List<_OperatorItemCateg>();

            try
            {


                strSQL = "";
                //strSQL += "SELECT * FROM [SPTOSystem].[dbo].[vewItemCategPlan]";
                //    strSQL += "  where OperatorID ='" + Opid + "'";



                strSQL += "sprGetExamMode";
                strSQL += "''";
                strSQL += ",'" + Opid + "'";
                strSQL += ",'Licence'";
                dt = ObjRun.GetDatatables(strSQL);
                if (dt.Rows.Count != 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        dataList.Add(new _OperatorItemCateg()
                        {
                            ItemCateg = row["ItemCateg"].ToString().Trim(),
                            ItemCategName = row["ItemCategName"].ToString().Trim(),
                            cntItemCateg = row["cntItemCateg"].ToString().Trim(),
                        });
                    }

                    ResultOPcateg._listOpCateg = dataList;
                    ResultOPcateg.strResult = "OK";
                }
                else
                {
                    ResultOPcateg.strResult = "No exam found <br/> ไม่พบข้อสอบ";
                }
            }
            catch (Exception e)
            {
                ResultOPcateg.strResult = e.Message + " :: " + strSQL;
            }

            return ResultOPcateg;
        }


        public DataTable GetItemCateg(string strItemCateg)
        {
            dt = new DataTable();
            mgrSQLConnect ObjRun = new mgrSQLConnect(_configuration);
            ResultItemCateg ResultOPcateg = new ResultItemCateg();
            List<_OperatorItemCateg> dataList = new List<_OperatorItemCateg>();

            try
            {
                strSQL = "";
                strSQL += "SELECT * FROM [SPTOSystem].[dbo].[ItemCategory]";
                strSQL += "  where ItemCateg ='" + strItemCateg + "'";
                dt = ObjRun.GetDatatables(strSQL);
                if (dt.Rows.Count != 0)
                {
                    strSQL = "XX";
                }
                else
                {
                    strSQL = "aaa";
                }
            }
            catch (Exception )
            {
                dt = null;
            }
            return dt;
        }


        public ResultItemCateg GetInputItemList(string strItemCateg, string OPID)
        {
            dt = new DataTable();
            mgrSQLConnect ObjRun = new mgrSQLConnect(_configuration);
            ResultItemCateg ResultOPcateg = new ResultItemCateg();
            List<_OperatorItemCateg> dataList = new List<_OperatorItemCateg>();

            try
            {


                strSQL = "";
                strSQL += "sprGetExamMode";
                strSQL += "'" + strItemCateg + "'";
                strSQL += ",'" + OPID + "'";
                strSQL += ",'Mode'";



                dt = ObjRun.GetDatatables(strSQL);
                if (dt.Rows.Count != 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        dataList.Add(new _OperatorItemCateg()
                        {
                            ItemCateg = row["ItemCode"].ToString().Trim(),
                            ItemCategName = row["ItemName"].ToString().Trim(),
                            cntItemCateg = row["TimeLimit"].ToString().Trim(),
                        });
                    }

                    ResultOPcateg._listOpCateg = dataList;
                    ResultOPcateg.strResult = "OK";
                }
                else
                {
                    ResultOPcateg.strResult = "Err :" + strSQL;
                }
            }
            catch (Exception e)
            {
                ResultOPcateg.strResult = e.Message;
            }

            return ResultOPcateg;
        }

        public DataTable GetInputItems(string strItemCode)
        {
            dt = new DataTable();
            mgrSQLConnect ObjRun = new mgrSQLConnect(_configuration);
            ResultItemCateg ResultOPcateg = new ResultItemCateg();
            List<_OperatorItemCateg> dataList = new List<_OperatorItemCateg>();

            try
            {
                strSQL = "";
                strSQL += "SELECT * FROM [SPTOSystem].[dbo].[InputItemList]";
                strSQL += "  where ItemCode ='" + strItemCode + "'";
                dt = ObjRun.GetDatatables(strSQL);
            }
            catch (Exception )
            {
                dt = null;
            }
            return dt;
        }


        public string MakingExam(string strItemCateg, string strItemCode)
        {
            dt = new DataTable();
            mgrSQLConnect ObjRun = new mgrSQLConnect(_configuration);
            ResultItemCateg ResultOPcateg = new ResultItemCateg();
            List<_OperatorItemCateg> dataList = new List<_OperatorItemCateg>();
            string Result = "";
            try
            {
                strSQL = "";
                strSQL += "sprMakeHTMLExam";
                strSQL += " N'" + strItemCateg + "',";
                strSQL += " N'" + strItemCode + "'";
                dt = ObjRun.GetDatatables(strSQL);
                if (dt.Rows.Count != 0)
                {
                    Result = dt.Rows[0][0].ToString();
                }
                else
                {
                    Result = "Error";
                }
            }
            catch (Exception e)
            {
                Result = e.Message;
            }
            return Result;
        }

        public _ExamCommitResult CommitExam(string Opid, string strItemCateg, string strItemInput, string strStartTime, string strEndTime, List<_ExamQuestionAnswer> ArrAns, string IpAddress)
        {
            mgrSQLConnect ObjRun = new mgrSQLConnect(_configuration);
            _ExamCommitResult EXresult = new _ExamCommitResult();
            DataTable dt = new DataTable();
            double Correct = 0;
            double TatalExam = 0;
            double Average = 0;
            double Wrong = 0;
            string strPlan = null;
            string strLevel = null;
            double Standard = 0;
            Boolean Result = true;
            try
            {
                foreach (_ExamQuestionAnswer rows in ArrAns)
                {
                    strSQL = "";
                    strSQL += "[sprExamAnswerCheck] '" + rows.strQuestion + "','" + rows.strAnswer + "','CheckAns'";
                    dt = ObjRun.GetDatatables(strSQL);
                    if (dt.Rows.Count > 0)
                    {
                        Correct += Convert.ToDouble(dt.Rows[0][0].ToString());
                    }
                }
                strSQL = "";
                strSQL += "[sprExamAnswerCheck] '" + strItemCateg + "','" + strItemInput + "','TatalExam'";
                dt = ObjRun.GetDatatables(strSQL);
                if (dt.Rows.Count > 0)
                {
                    TatalExam += Convert.ToDouble(dt.Rows[0][0].ToString());
                }
                Wrong = TatalExam - Correct;
                Average = (Correct / TatalExam) * 100;
                strSQL = "";
                strSQL += "select * from vewItemCategPlan";
                strSQL += " where OperatorID = '" + Opid + "' and ItemCateg = '" + strItemCateg + "'";

                dt = ObjRun.GetDatatables(strSQL);
                if (dt.Rows.Count > 0)
                {
                    strPlan = dt.Rows[0]["Plan"].ToString();
                    strLevel = "";
                    Standard = Convert.ToDouble(dt.Rows[0]["Standard"].ToString());
                }
                else
                {

                    EXresult.BoolResult = false;
                    EXresult.strMgs = "vewItemCategPlan";
                    EXresult.strResult = "Error";
                }

                if(Average >= Standard)
                {
                    Result = true;
                }
                else
                {
                    Result = false;
                }


               // Result = (Average > Standard) ? true : false;

                strSQL = "";
                strSQL += "[sprExamResults] ";
                strSQL += " '" + strPlan + "' ";
                strSQL += ",'" + Opid + "' ";
                strSQL += ",'" + strItemCateg + "' ";
                strSQL += ",'" + strItemInput + "' ";
                strSQL += ",'" + strStartTime + "' ";
                strSQL += ",'" + strEndTime + "' ";
                strSQL += ",'" + strLevel + "' ";
                strSQL += ",'" + Correct + "' ";
                strSQL += ",'" + Wrong + "' ";
                strSQL += ",'" + TatalExam + "' ";
                strSQL += ",'" + Result + "' ";
                strSQL += ",'" + Opid + "' ";
                strSQL += ",'" + IpAddress + "' ";
                dt = ObjRun.GetDatatables(strSQL);


                if (dt.Rows.Count > 0)
                {
                    if (dt.Rows[0][1].ToString() == "OK")
                    {
                        string ExNbr = dt.Rows[0]["ExNbr"].ToString();

                        if(ArrAns.Count != 0) { 

                        foreach (_ExamQuestionAnswer rows in ArrAns)
                        {
                            strSQL = "";
                            strSQL += "[sprExamResultsAttribute] '" + ExNbr + "','" + rows.strQuestion + "','" + rows.strAnswer + "' ,'" + Opid + "','" + IpAddress + "' ";
                            dt = ObjRun.GetDatatables(strSQL);
                            if (dt.Rows.Count > 0)
                            {
                                EXresult.BoolResult = Convert.ToBoolean(dt.Rows[0][0].ToString());
                                EXresult.strMgs = Convert.ToString(Result); //dt.Rows[0][1].ToString();
                                EXresult.strResult = dt.Rows[0][1].ToString();
                            }
                        }
                        }
                        else
                        {
                            EXresult.BoolResult = false;
                            EXresult.strMgs = "คุณไม่เลือกคำตอบ";
                            EXresult.strResult = "คุณทำข้อสอบไม่ผ่าน";
                        }
                    }
                    else
                    {
                        EXresult.BoolResult = false;
                        EXresult.strMgs = dt.Rows[0][1].ToString();
                        EXresult.strResult = "Error";
                    }


                }
                else
                {
                    EXresult.BoolResult = false;
                    EXresult.strMgs = "ExamResult";
                    EXresult.strResult = "Error";
                }


            }
            catch (Exception e)
            {
                EXresult.BoolResult = false;
                EXresult.strMgs = e.Message.ToString();
                EXresult.strResult = "Error";
            }








            return EXresult;
        }
        public ListSelectList_ GetPlaning(string strCriteria)
        {
            ListSelectList_ resultList = new ListSelectList_(); 
            mgrSQLConnect ObjRun = new mgrSQLConnect(_configuration);
            List<SelectListItem> listItems = new List<SelectListItem>();
            dt = new DataTable();
            strSQL = "";
            string strItemText = "";
            string strItemVal = "";
            try
            {
                if(strCriteria == "ItemCateg")
                {
                    strCriteria = "ItemCateg,ItemCategName ";
                    strItemText = "ItemCategName";
                    strItemVal = "ItemCateg";
                }
                else if(strCriteria == "ItemCode")
                {
                    strCriteria = "ItemCode,ItemName ";
                    strItemText = "ItemName";
                    strItemVal = "ItemCode";
                }
                else if (strCriteria == "OperatorID")
                {
                    strCriteria = "OperatorID,OperatorName ";
                    strItemText = "OperatorName";
                    strItemVal = "OperatorID";
                }
                else
                {
                   
                    strItemText = strCriteria;
                    strItemVal = strCriteria;
                }







                strSQL = "select "; 
                strSQL += " " + strCriteria + " ";
                strSQL += " from vewExamResults ";
                strSQL += " group by " + strCriteria + " ";
                strSQL += " order by " + strCriteria + " ";

                dt = ObjRun.GetDatatables(strSQL);
                if (dt.Rows.Count != 0)
                {
                    listItems.Add(new SelectListItem()
                    {
                        Text = "- ALL -",
                        Value = "",
                    });
                    foreach (DataRow row in dt.Rows)
                    {


                        string strTxt = (strItemVal == strItemText ? row[strItemText].ToString().Trim() : row[strItemVal].ToString().Trim() + " : " + row[strItemText].ToString().Trim());


                        listItems.Add(new SelectListItem()
                        {
                            Text = strTxt,
                            Value = row[strItemVal].ToString().Trim(),
                        });
                    }
                    resultList._ListSelectList = listItems;
                    resultList.strResult = "OK";
                }
                else
                {
                    resultList.strResult = "Data "+ strCriteria + " not found";
                }

               
            }
            catch (Exception e)
            {
                resultList.strResult = e.Message;
            }

            return resultList;


        }

        public _ExamResultList GetDataExamResultList(_excamResultCtrl strCtrl)
        {
            _ExamResultList resultList = new _ExamResultList();
            mgrSQLConnect ObjRun = new mgrSQLConnect(_configuration);
            List<_ExamResultDetail> listItems = new List<_ExamResultDetail>();
            dt = new DataTable();
            strSQL = ""; 
            try
            {
                  strSQL = "sprExamResultInquiry ";
                strSQL += " '" + strCtrl.PlanRefID + "',";
                strSQL += " '" + strCtrl.ItemCateg + "',";
                strSQL += " '" + strCtrl.ItemCode + "',";
                strSQL += " '" + strCtrl.OperatorID + "',";
                strSQL += " '" + strCtrl.StartTime + "',";
                strSQL += " '" + strCtrl.EndTime + "'";
                

                dt = ObjRun.GetDatatables(strSQL);
                if (dt.Rows.Count != 0)
                {
                     
                    foreach (DataRow row in dt.Rows)
                    {


                       

                        listItems.Add(new _ExamResultDetail()
                        {
                            PlanRefID = row["PlanRefID"].ToString().Trim(),
                            ItemCateg = row["ItemCateg"].ToString().Trim(),
                            ItemCategName = row["ItemCategName"].ToString().Trim(),
                            ItemCode = row["ItemCode"].ToString().Trim(),
                            ItemName = row["ItemName"].ToString().Trim(),
                            OperatorID = row["OperatorID"].ToString().Trim(),
                            OperatorName = row["OperatorName"].ToString().Trim(),
                            Minutes = row["Minutes"].ToString().Trim(),
                            StartTime = row["StartTime"].ToString().Trim(),
                            EndTime = row["EndTime"].ToString().Trim(),
                            Level = row["Level"].ToString().Trim(),
                            Correct = row["Correct"].ToString().Trim(),
                            Wrong = row["Wrong"].ToString().Trim(),
                            Total = row["Total"].ToString().Trim(),
                            Results = row["Results"].ToString().Trim(),
                            AddDate = row["AddDate"].ToString().Trim(),

                        });
                    }
                    resultList.DataExamReultList = listItems;
                    resultList.strResult = "OK";
                }
                else
                {
                    resultList.strResult = "Data not found";
                }


            }
            catch (Exception e)
            {
                resultList.strResult = "Error : "+e.Message;
            }

            return resultList;


        }



    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using jrs.DBContexts;
using jrs.Models;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.AspNetCore.Authorization;
using jrs.Classes;
using System.Security.Claims;
using Microsoft.Data.SqlClient;
using System.Data;



namespace jrs.DA
{
    public class DalCommon
    {
        
        private readonly GeneralContext _context;
        private readonly IMSLogContext _logContext;

        private string _userId = "";
        private string _ip = "";


        public DalCommon(GeneralContext context, IMSLogContext logContext,string UserId,string IP)
        {
            _context = context;
            _logContext = logContext;
            _userId=UserId;
            _ip=IP;
        }



        public  List<ImsList> GetGenericList(string ListName)
        {
            ImsUtility utility = new ImsUtility();


            List<ImsList> genericList = new List<ImsList>();
            string spName=DatabaseConstants.spIMSSelGetGenericList;

            SqlDataReader dr = null;

            try
            {

                using (SqlConnection conn = _context.Database.GetDbConnection() as SqlConnection)
                {
                    if (conn.State != ConnectionState.Open) conn.Open();
                    SqlCommand sqlcmd = new SqlCommand();
                    sqlcmd.Connection = conn;
                    sqlcmd.CommandType = CommandType.StoredProcedure;
                    sqlcmd.Parameters.AddWithValue("@LISTNAME",ListName);
                    sqlcmd.CommandText = spName;

                    dr = sqlcmd.ExecuteReader();
                    while (dr.Read())
                    {
                        ImsList listElement = new ImsList();

                        listElement.text = (dr["TEXT"] + "");
                        listElement.value = (dr["VALUE"] + "");
                        genericList.Add(listElement);

                    }
                }
            }
            catch (Exception ex)
            {
                ImsLogerrors evt = utility.GetLogError("DalCommon", "GetGenericList", ex, _userId,ListName, _ip);
                _logContext.ImsLogerrors.Add(evt);
                _logContext.SaveChanges();
            }
            return genericList;
        }



        public  List<ImsList> GetGenericListByCompany(string ListName,string Company)
        {
            ImsUtility utility = new ImsUtility();


            List<ImsList> genericList = new List<ImsList>();
            string spName=DatabaseConstants.spIMSSelGetGenericListByCompany;

            SqlDataReader dr = null;

            try
            {

                using (SqlConnection conn = _context.Database.GetDbConnection() as SqlConnection)
                {
                    if (conn.State != ConnectionState.Open) conn.Open();
                    SqlCommand sqlcmd = new SqlCommand();
                    sqlcmd.Connection = conn;
                    sqlcmd.CommandType = CommandType.StoredProcedure;
                    sqlcmd.Parameters.AddWithValue("@LISTNAME",ListName);
                    sqlcmd.Parameters.AddWithValue("@COMPANY",Company);
                    sqlcmd.CommandText = spName;

                    dr = sqlcmd.ExecuteReader();
                    while (dr.Read())
                    {
                        ImsList listElement = new ImsList();

                        listElement.text = (dr["TEXT"] + "");
                        listElement.value = (dr["VALUE"] + "");
                        genericList.Add(listElement);

                    }
                }
            }
            catch (Exception ex)
            {
                ImsLogerrors evt = utility.GetLogError("DalCommon", "GetGenericList", ex, _userId,ListName, _ip);
                _logContext.ImsLogerrors.Add(evt);
                _logContext.SaveChanges();
            }
            return genericList;
        }
    }
}
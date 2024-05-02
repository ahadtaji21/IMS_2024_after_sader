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
    public class DalDOC
    {
        
        private readonly GeneralContext _context;
        private readonly IMSLogContext _logContext;

        private string _userId = "";
        private string _ip = "";


        public DalDOC(GeneralContext context, IMSLogContext logContext,string UserId,string IP)
        {
            _context = context;
            _logContext = logContext;
            _userId=UserId;
            _ip=IP;
        }




       public byte[] GetDOCTemplate(string type)
        {
            ImsUtility utility = new ImsUtility();


            byte[]  template = null;
            string viewName = "SELECT  TOP 1 TEMPLATE FROM IMS_DOC_TEMPLATE WHERE TYPE=@TYPE ORDER by UPLOAD_DATE DESC";
            
            SqlDataReader dr = null;

            try
            {
            
                using (SqlConnection conn = _context.Database.GetDbConnection() as SqlConnection)
                {
                    if (conn.State != ConnectionState.Open) conn.Open();
                    SqlCommand sqlcmd = new SqlCommand();
                    sqlcmd.Connection = conn;
                    sqlcmd.CommandType = CommandType.Text;

                    sqlcmd.CommandText = viewName;
                    sqlcmd.Parameters.AddWithValue("@TYPE", type);
                    dr = sqlcmd.ExecuteReader();
                    while (dr.Read())
                    {

                        template = (byte[])(dr["TEMPLATE"]);
                       
                      
                        


                    }
                }
            }
            catch (Exception ex)
            {

                ImsLogerrors evt = utility.GetLogError("DalDOC", "GetDOCTemplate", ex, _userId, type, _ip);
                _logContext.ImsLogerrors.Add(evt);
                _logContext.SaveChanges();


            }

            return template;

        }
    }
}
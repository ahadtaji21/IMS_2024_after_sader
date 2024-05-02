using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using jrs.Models;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace jrs.Classes
{
    public class ImsUtility
    {

        public string GetIsoCompany(string company)
        {

            if(company != null && company.Length>3)
            return company.Substring(0,3);
            else return company;
        }
        public string GetNavUrlPrefix()
        {
            return "http://nav.jrs.net:7048/NAV02/ODataV4/Company('";
        }
       public  NetworkCredential GetNavCredential()
        {

            return new NetworkCredential("NAV.INTEGRATION", "yBZgObvaGJk/RSL5swufffZjnLMx13qdzXRGDqSq3MY=");
        }
        private string FormatStackTrace(Exception ex)
        {
            StringBuilder stacktrace = new StringBuilder();
            stacktrace.Append("EXCEPTION:"); stacktrace.Append(ex.Message); stacktrace.Append(Environment.NewLine);
            stacktrace.Append("STACK TRACE:"); stacktrace.Append(Environment.NewLine);
            stacktrace.Append(ex.StackTrace); stacktrace.Append(Environment.NewLine);
            stacktrace.Append(ex.StackTrace); stacktrace.Append(Environment.NewLine);
            while (ex.InnerException != null)
            {

                ex = ex.InnerException;
                stacktrace.Append("INNER EXCEPTION:");
                stacktrace.Append(ex.Message); stacktrace.Append(Environment.NewLine);
                stacktrace.Append("INNER STACK TRACE:"); stacktrace.Append(Environment.NewLine);
                stacktrace.Append(ex.StackTrace); stacktrace.Append(Environment.NewLine);
            }
            return stacktrace.ToString();
        }

        public ImsLogevents GetLogEvent(string Class, string Procedure, string Message, string User, Object ToBeLogged, string IP)
        {
            ImsLogevents evt = new ImsLogevents();
            evt.InsertDate = DateTime.Now;
            evt.Class = Class;
            evt.Procedure = Procedure;
            evt.Message = Message;
            evt.Ip = IP;
            evt.UserId = User;
            if (ToBeLogged != null)
            {
                StringBuilder sb = new StringBuilder();
                foreach (PropertyInfo propertyInfo in ToBeLogged.GetType().GetProperties())
                {
                    sb.Append(propertyInfo.Name);
                    sb.Append(":");
                    sb.Append(propertyInfo.GetValue(ToBeLogged, null));
                    sb.Append(";");
                }
                evt.Parameters = sb.ToString();
            }


            return evt;

        }
        public ImsLogevents GetLogEvent(string Class, string Procedure, string Message, string User, string Params, string IP)
        {
            ImsLogevents evt = new ImsLogevents();
            evt.InsertDate = DateTime.Now;
            evt.Class = Class;
            evt.Procedure = Procedure;
            evt.Message = Message;
            evt.Ip = IP;
            evt.UserId = User;
            evt.Parameters = Params;
            return evt;

        }

        public ImsLogerrors GetLogError(string Class, string Procedure, Exception ex, string User, Object ToBeLogged, string IP)
        {
            ImsLogerrors evt = new ImsLogerrors();
            evt.InsertDate = DateTime.Now;
            evt.Class = Class;
            evt.Procedure = Procedure;
            evt.Ip = IP;
            evt.StackTrace = FormatStackTrace(ex);
            evt.Source = ex.Source;
            evt.UserId = User;

            if (ToBeLogged != null)
            {
                StringBuilder sb = new StringBuilder();
                foreach (PropertyInfo propertyInfo in ToBeLogged.GetType().GetProperties())
                {
                    sb.Append(propertyInfo.Name);
                    sb.Append(":");
                    sb.Append(propertyInfo.GetValue(ToBeLogged, null));
                    sb.Append(";");
                }
                evt.Parameters = sb.ToString();
            }
            else
            {
                evt.Parameters = "inputobj=null";
            }

            return evt;

        }
        public ImsLogerrors GetLogError(string Class, string Procedure, Exception ex, string User, string Params, string IP)
        {
            ImsLogerrors evt = new ImsLogerrors();
            evt.InsertDate = DateTime.Now;
            evt.Class = Class;
            evt.Procedure = Procedure;
            evt.StackTrace = FormatStackTrace(ex);
            evt.UserId = User;
            evt.Source = ex.Source;
            evt.Ip = IP;
            evt.Parameters = Params;
            return evt;

        }
        public string GetIP(HttpResponse Response)
        {
            string ip = Response.HttpContext.Connection.RemoteIpAddress.ToString();

            if (ip == "::1")
            {
                try
                {
                    if (Dns.GetHostEntry(Dns.GetHostName()).AddressList != null &&
                    Dns.GetHostEntry(Dns.GetHostName()).AddressList.Length > 0)
                    {
                        if (Dns.GetHostEntry(Dns.GetHostName()).AddressList.Length > 1)
                            ip = Dns.GetHostEntry(Dns.GetHostName()).AddressList[1].ToString();
                        else
                            ip = Dns.GetHostEntry(Dns.GetHostName()).AddressList[0].ToString();

                    }
                }
                catch (Exception ex)
                {

                }
            }
            return ip.ToString();
        }


    }
    internal class DatabaseConstants
    {
        internal const string spPMSSelGetProjectWorkflowStatus = "PMS_SP_SEL_GET_PROJECT_WORKFLOW_STATUS";
        internal const string spPMSSelGetAnnualPlanList = "PMS_SP_SEL_GET_ANNUAL_PLAN_LIST";
        internal const string spPMSUpdAnnualPlanDetails = "PMS_SP_UPD_ANNUAL_PLAN_DETAILS";
        internal const string spIMSSelGetGenericList = "IMS_SP_SEL_GET_GENERIC_LIST";
        internal const string spIMSSelGetGenericListByCompany = "IMS_SP_SEL_GET_GENERIC_LIST_BY_COMPANY";

        internal const string spIMSSelGetJRSCOA = "IMS_SP_SEL_GET_JRS_COA";

        internal const string spPMSSelGetProgressiveId = "PMS_SP_SEL_GET_PROGRESSIVE_ID";


         internal const string spPMSSelGetEntryNbr = "PMS_SP_SEL_GET_ENTRY_NMB";
    }
}

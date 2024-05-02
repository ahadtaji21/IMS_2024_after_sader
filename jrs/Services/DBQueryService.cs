using System;
using System.Threading.Tasks;
using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using jrs.Models;

namespace jrs.Services
{
    public interface IDBQueryService
    {
        Task<Object> GenericQueryTableStructure(DbContext context, GenericSqlSelectPayload selectPayload);
        Task<Object> GenericQueryTableStructureAndData(DbContext context, GenericSqlSelectPayload selectPayload);
    }

    public class DBQueryService : IDBQueryService
    {
        /// <summary>Retrieve Jrs Table structure.</summary>
        /// <param name="context">DB context to execute query (GeneralContext)</param>
        /// <param name="selectPayload">Query conditions</param>
        /// <returns>Json string describing the Jrs Table structure.</returns>
        public async Task<Object> GenericQueryTableStructure(DbContext context, GenericSqlSelectPayload selectPayload)
        {
            var retJson = new SqlParameter("RET_JSON", "") { Direction = ParameterDirection.Output, DbType = DbType.String, Size = -1 };
            try
            {
                await context.Database.ExecuteSqlInterpolatedAsync($"dbo.UTIL_SP_GENERIC_QUERY_DATA_STRUCTURE @VIEW_NAME={selectPayload.viewName}, @COLUMN_LIST={selectPayload.colList}, @WHERE_CONDITION={selectPayload.whereCond}, @ORDER_STATEMENT={selectPayload.orderStmt}, @RET_JSON={retJson} output");
            }
            catch (Exception e)
            {
                throw e;
            }
            return retJson.Value;
        }

        /// <summary>Retrieve Jrs Table structure and data.</summary>
        /// <param name="context">DB context to execute query (GeneralContext)</param>
        /// <param name="selectPayload">Query conditions</param>
        /// <returns>Json string describing the Jrs Table structure and the requested data.</returns>
        public async Task<Object> GenericQueryTableStructureAndData(DbContext context, GenericSqlSelectPayload selectPayload)
        {
            var retJson = new SqlParameter("RET_JSON", "") { Direction = ParameterDirection.Output, DbType = DbType.String, Size = -1 };
            try
            {
                // Setup prameters
                string execViewName = selectPayload.viewName;
                string columnList = selectPayload.colList;
                string whereCondition = selectPayload.whereCond;
                string orderStatement = selectPayload.orderStmt;
                int? itemNumber = selectPayload.itemNumber;
                int? skipNumber = selectPayload.skipNumber;
                int? officeId = selectPayload.officeId;
                string currentLocale = selectPayload.currentLocale;

                // execute query
                await context.Database.ExecuteSqlInterpolatedAsync($"dbo.UTIL_SP_GENERIC_QUERY_DATA @VIEW_NAME={execViewName}, @COLUMN_LIST={columnList}, @WHERE_CONDITION={whereCondition}, @ORDER_STATEMENT={orderStatement}, @FETCH_NBR={itemNumber}, @OFFSET_NBR={skipNumber}, @OFFICE_ID={officeId}, @LOCALE={currentLocale}, @RET_JSON={retJson} output");

            }
            catch (Exception e)
            {
                throw e;
            }
            return retJson.Value;
        }


        public async Task<Object> GenericQuestionnaireStructureAndData(DbContext context, int questionnaireId, GenericConditionRule[]? conditionRules)
        {
            var retJson = new SqlParameter("RET_JSON", "") { Direction = ParameterDirection.Output, DbType = DbType.String, Size = -1 };
            try
            {
                string conditions = null;

                //Manage condition rules
                if(conditionRules != null && conditionRules.Length > 0){
                    string[] conditionStrings = Array.ConvertAll<GenericConditionRule, string>(conditionRules, item => item.ToString());
                    conditions = string.Join(",", conditionStrings);
                    conditions = $"[{conditions}]";
                }

                // execute query
                await context.Database.ExecuteSqlInterpolatedAsync($"dbo.UTIL_SP_QUESTIONNAIRE_DATA_AND_STRUCTURE @QUESTIONNAIRE_ID={questionnaireId}, @CONDITION_RULES={conditions}, @RET_JSON={retJson} output");
            
                return retJson.Value;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}

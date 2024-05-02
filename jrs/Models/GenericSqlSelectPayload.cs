using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace jrs.Models
{

	/// <summary>
	/// Class defining the required payload for generic sql select calls.
	/// </summary>
	public class GenericSqlSelectPayload
	{
		///<summary>Name of db view to query.</summary>
		public string viewName { get; set; }
        public string viewSchema { get; set; }

        ///<summary>List of names of specific columns to query. If not defined, all columns are returned.</summary>
        public string colList { get; set; }

		///<summary>Where condition to be applied to query.</summary>
		public string whereCond { get; set; }

		///<summary>Ordering criteria of query.</summary>
		public string orderStmt { get; set; }

		///<summary>Number of items to recover</summary>
		public int? itemNumber { get; set; }

		///<summary>Number of items to skip</summary>
		public int? skipNumber { get; set; }

		///<summary>Office to filter dataset</summary>
		public int? officeId { get; set; }

		/// <summary>The users current locale</summary>
		public string currentLocale { get; set; }

		/// <summary>Enables the possibility to override the office filter when querying the data</summary>
		public bool? ignoreOfficeFilter { get; set; }
		public GenericConditionRule[]? conditionRules { get; set; }
	}

	// public enum RuleOperator{
	//     [EnumMember(Value = "<")]
	//     lt,
	//     [EnumMember(Value = "<=")]
	//     lte,
	//     [EnumMember(Value = "=")]
	//     eq,
	//     [EnumMember(Value = ">=")]
	//     gte,
	//     [EnumMember(Value = ">")]
	//     gt,
	//     [EnumMember(Value = "!=")]
	//     ne,
	// }
	// // /// <summary>
	// // /// Class defining available rule operators.
	// // /// </summary>
	// // public class RuleOperator
	// // {
	// //     public static RuleOperator lt = new RuleOperator("<");
	// //     public static RuleOperator lte = new RuleOperator("<=");
	// //     public static RuleOperator eq = new RuleOperator("=");
	// //     public static RuleOperator gte = new RuleOperator(">=");
	// //     public static RuleOperator gt = new RuleOperator(">");
	// //     public static RuleOperator ne = new RuleOperator("!=");
	// //     public string sqlOperator { get; set; }
	// //     private RuleOperator(string op) { this.sqlOperator = op; }

	// //     /// <summary>
	// //     /// Generates a string representation of the RuleOperator instance.
	// //     /// </summary>
	// //     /// <returns>
	// //     /// The string representation of the object.
	// //     /// </returns>
	// //     public override string ToString()
	// //     {
	// //         return this.sqlOperator;
	// //     }

	// //     ///<summary>
	// //     /// Explicit conversion from {string}operatorCode to RuleOperato
	// //     ///</summary>
	// //     ///<param name="operatorCode">operator code used to create instance of RuleOperator.</param>
	// //     public static explicit operator RuleOperator(string operatorCode)
	// //     {
	// //         switch (operatorCode)
	// //         {
	// //             case "lt":
	// //                 return lt;
	// //             case "lte":
	// //                 return lte;
	// //             case "eq":
	// //                 return eq;
	// //             case "gt":
	// //                 return gt;
	// //             case "gte":
	// //                 return gte;
	// //             case "ne":
	// //                 return ne;
	// //             default:
	// //                 throw new ArgumentOutOfRangeException("operatorCode", "Invalid operator");
	// //         }
	// //     }

	// //     ///<summary>
	// //     /// Verify if the porvided operatorCode is a valid code
	// //     ///</summary>
	// //     ///<param name="operatorCode">Operator code to check.</param>
	// //     public static bool isValidOperator(string operatorCode)
	// //     {
	// //         string[] validOperators = {
	// //             "lt",
	// //             "lte",
	// //             "eq",
	// //             "gte",
	// //             "gt",
	// //             "ne"
	// //         };
	// //         bool foundCorrectOperator = false;

	// //         foreach (string opCode in validOperators)
	// //         {
	// //             if (String.Equals(operatorCode, opCode)){
	// //                 foundCorrectOperator = true;
	// //                 break;
	// //             }
	// //         }
	// //         return foundCorrectOperator;
	// //     }
	// // }

	/// <summary>
	/// Class defining a rule that is part of a more complex query "where" condition.
	/// </summary>
	public class GenericConditionRule
	{

		public enum RuleOperator
		{
			// [EnumMember(Value = "<")]
			lt,
			[EnumMember(Value = "<=")]
			lte,
			[EnumMember(Value = "=")]
			eq,
			[EnumMember(Value = ">=")]
			gte,
			[EnumMember(Value = ">")]
			gt,
			[EnumMember(Value = "!=")]
			ne
		}


		public string operandA { get; set; }
		public string operandB { get; set; }
		public RuleOperator Operator { get; set; }

		public GenericConditionRule(string a, string b, RuleOperator operatorCode)
		{
			try
			{
				// if(RuleOperator.isValidOperator(operatorCode)){
				// RuleOperator op = operatorCode; //(RuleOperator)operatorCode;

				this.operandA = a;
				this.operandB = b;
				this.Operator = operatorCode;
				// }
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

        public static string parseSqlOperator(RuleOperator operatorCode){

            switch (operatorCode)
	        {
	            case RuleOperator.lt:
	                return "<";
	            case RuleOperator.lte:
	                return "<=";
	            case RuleOperator.eq:
	                return "=";
	            case RuleOperator.gte:
	                return ">=";
	            case RuleOperator.gt:
	                return ">";
	            case RuleOperator.ne:
	                return "=";
	            default:
	                throw new ArgumentOutOfRangeException("operatorCode", "Invalid operator");
	        }
        }

		/// <summary>
		/// Generates a string representation of the GenericConditionRule instance.
		/// </summary>
		/// <returns>
		/// The string representation of the object.
		/// </returns>        
		public override string ToString()
		{
            string sqlOperator = GenericConditionRule.parseSqlOperator(this.Operator);
			string ret = $"{{\"operandA\":\"{this.operandA}\", \"operandB\":\"{this.operandB}\", \"Operator\":\"{sqlOperator}\"}}";
			return ret;
		}
	}

}
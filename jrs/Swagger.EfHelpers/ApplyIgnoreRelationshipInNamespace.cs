using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.OpenApi.Models;

namespace Core.Api.Infra.Swagger.Filters{
    /// <summary>
    /// Apply this filter to ignore all related objects that could cause  a self referencing loop
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ApllyIgnoreRelationshipInNamespace<T> : ISchemaFilter{

        /// <summary>
        /// Required by interface
        /// </summary>
        /// <param name="schema"></param>
        /// <param name="context"></param>
        public void Apply(OpenApiSchema schema, SchemaFilterContext context){

            List<string> exclusionModels = new List<string>(){
                "jrs.Models.Menu"
            };
            List<string> exclusionProperties = new List<string>(){
                "InverseParentMenu"
            };

            if(schema.Properties == null || !context.Type.Namespace.Equals(typeof(T).Namespace)){
                return;
            }

            if(exclusionModels.Contains(context.Type.ToString())){
                context.Type.GetProperties()
                    .Where( prop => 
                        prop.PropertyType.Namespace == typeof(T).Namespace
                        && exclusionProperties.Contains(prop.Name))
                    .Select( prop => cleanPropName(prop.Name))
                    .ToList()
                    .ForEach( prop => {
                        if(schema.Properties.ContainsKey(prop)){
                            schema.Properties.Remove(prop);
                        }
                    });
            }

            // context.Type.GetProperties()
            //     .Where( prop => prop.PropertyType.Namespace == typeof(T).Namespace)
            //     .Select( prop => cleanPropName(prop.Name))
            //     .ToList()
            //     .ForEach( prop => {
            //         if(schema.Properties.ContainsKey(prop)){
            //             schema.Properties.Remove(prop);
            //         }
            //     });
        }

        // private static string cleanPropName(string str) => string.IsNullOrEmpty(str) || str.Length < 2 ? str : char.ToLowerInvariant(str[0]) + str.Substring(1);
        private static string cleanPropName(string str){
            if(string.IsNullOrEmpty(str) || str.Length < 2){
                return str;
            }
            return char.ToLowerInvariant(str[0]) + str.Substring(1);
        }
    }
}

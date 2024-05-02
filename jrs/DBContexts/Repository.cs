using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace jrs.DBContexts
{
    public class Repository : Interfaces.IRepository
    {
        private JRSDBContext _db;
        public Repository(JRSDBContext db)
        {
            this._db = db;
        }

        public void Add(Test t)
        {
            _db.Add(t);
            _db.SaveChanges();
        }

        public IEnumerable<Test> GetTests()
        {
            return (from t in _db.Tests
                   select t);
        }

        public IEnumerable<TestDepartment> GetTestDepartments(int? id = null){
            return this._db.TestDepartments.Where( dep => id.HasValue == false || dep.TestDepartmentId == id );
        }

        // [Obsolete]
        // public IEnumerable<Test> GetTestsFromSp(int id)
        // {
        //     var entities = _db.Set<Test>();
        //     var idx = new SqlParameter("id", id);
        //     return entities.FromSql("exec sp_SEL_GetDataFromTest  @id ", idx);
           

        // }
    }
}
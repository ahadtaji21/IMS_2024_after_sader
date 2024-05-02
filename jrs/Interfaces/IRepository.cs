using jrs.DBContexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace jrs.Interfaces
{
    public interface IRepository
    {
        IEnumerable<Test> GetTests();

       // IEnumerable<Test> GetTestsFromSp(int id);
        void Add(Test t);

        IEnumerable<TestDepartment> GetTestDepartments(int? id = null);
    }
}

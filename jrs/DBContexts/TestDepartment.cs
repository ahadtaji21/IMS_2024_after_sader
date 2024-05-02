using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace jrs.DBContexts
{
    [Table("TEST_DEPARTMENT")]
    public class TestDepartment{
        [Column("TEST_DEPARTMENT_ID")]
        public int TestDepartmentId { get; set; }

        [Column("TEST_DEPARTMENT_DESCR")]
        public string TestDepartmentDescr { get; set; }
    }
}



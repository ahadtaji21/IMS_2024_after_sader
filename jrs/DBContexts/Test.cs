using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace jrs.DBContexts
{
    [Table("TEST")]
    public class Test
    {

        public int Id { get; set; }

        public string Descr { get; set; }
    }
}
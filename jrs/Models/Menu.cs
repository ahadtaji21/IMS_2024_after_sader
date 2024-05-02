using System;
using System.Collections.Generic;

namespace jrs.Models
{
    public partial class Menu
    {
        public Menu()
        {
            InverseParentMenu = new HashSet<Menu>();
        }
        public int Id { get; set; }
        public string Descr { get; set; }
        public string Url { get; set; }
        public int? ParentMenuId { get; set; }
        public string LabelKey { get; set; }
        public string IconCode { get; set; }
        public virtual ICollection<Menu> InverseParentMenu { get; set; }
        public int OrdinalPosition { get; set; }

        ///<summary>Reference to module</summary>
        public int? ModuleId { get; set; }
        
        ///<summary>Name of the module</summary>
        public string ModuleName { get; set; }

        ///<summary>Code of the module</summary>
        public string ModuleCode { get; set; }

        ///<summary>
        /// Defines a hidden menu item.
        /// The menu will not be accessible through the navigation tree.
        ///</summary>
        public Boolean? isHidden { get; set; }
    }
}

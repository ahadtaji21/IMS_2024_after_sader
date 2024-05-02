namespace jrs.Models
{
    public partial class PmsProject
    {
        
    }
    public partial class PmsProjectWorkFlow
    {
        public int previousStatus {get;set;} 
         public int nextStatus {get;set;} 
    }

      public partial class PmsProjectRights
    {
        public bool editRight {get;set;} 
         public bool viewRight {get;set;} 
    }
}
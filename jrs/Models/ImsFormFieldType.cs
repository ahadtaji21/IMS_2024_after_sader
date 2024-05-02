namespace jrs.Models
{
    public partial class ImsFormFieldType
    {
        public int id { get; set; }
        public string code { get; set; }
        public string descr { get; set; }
        public bool isAvailableForQuestionnaire { get; set; }
    }
}
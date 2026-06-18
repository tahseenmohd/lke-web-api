namespace LKE_DAL.Models
{
    public class EntityInformation
    {
       public int ID { get; set; }
       public int BAC { get; set; }
       public int CaseID { get; set; }
       public int CaseID2 { get; set; }
       public int EntityID { get; set; }
       public int Domicile { get; set; }
       public int USLegalType { get; set; }
       public int USTaxType { get; set; }
       public int LocalLegalType { get; set; }
       public int LocalTaxType {get; set;}
       public int IsConsolidated { get; set; }
       public string ShortName { get; set; }
       public string LongName { get; set; }
       public int FunctionalCurrencyID { get; set; }
       public string WTPRepName { get; set; }
       public string WTPRepNum { get; set; }
       public string WTPRepEmail { get; set; }
        public string EntityName { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressCity { get; set; }
        public string AddressState { get; set; }
        public string AddressCountryID { get; set; }
        public string AddressZipCode { get; set; }
    }
}
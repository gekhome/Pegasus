//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Pegasus.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class EXP_FREELANCE
    {
        public int EXP_ID { get; set; }
        public int AITISI_ID { get; set; }
        public Nullable<System.DateTime> DATE_FROM { get; set; }
        public Nullable<System.DateTime> DATE_TO { get; set; }
        public Nullable<float> DAYS_AUTO { get; set; }
        public Nullable<float> DAYS_MANUAL { get; set; }
        public Nullable<float> INCOME { get; set; }
        public Nullable<int> INCOME_YEAR { get; set; }
        public Nullable<float> INCOME_TAXFREE { get; set; }
        public string INCOME_NOMISMA { get; set; }
        public Nullable<float> MORIA { get; set; }
        public string DOC_PROTOCOL { get; set; }
        public string DOC_ORIGIN { get; set; }
        public string DOC_COMMENT { get; set; }
        public Nullable<bool> DOC_VALID { get; set; }
        public string ERROR_TEXT { get; set; }
        public Nullable<bool> DUPLICATE { get; set; }
    
        public virtual AITISIS AITISIS { get; set; }
    }
}

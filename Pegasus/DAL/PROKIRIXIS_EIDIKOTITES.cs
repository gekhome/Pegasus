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
    
    public partial class PROKIRIXIS_EIDIKOTITES
    {
        public int PSE_ID { get; set; }
        public int PROKIRIXI_ID { get; set; }
        public int SCHOOL_ID { get; set; }
        public int EIDIKOTITA_ID { get; set; }
    
        public virtual SYS_EIDIKOTITES SYS_EIDIKOTITES { get; set; }
        public virtual PROKIRIXIS PROKIRIXIS { get; set; }
    }
}

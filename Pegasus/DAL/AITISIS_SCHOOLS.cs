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
    
    public partial class AITISIS_SCHOOLS
    {
        public int ID { get; set; }
        public int AITISI_ID { get; set; }
        public int SCHOOL_TYPE { get; set; }
        public int SCHOOL { get; set; }
        public int PERIFERIA_ID { get; set; }
        public Nullable<int> PROKIRIXI_ID { get; set; }
    
        public virtual AITISIS AITISIS { get; set; }
    }
}

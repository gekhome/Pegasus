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
    
    public partial class UploadVocation
    {
        public UploadVocation()
        {
            this.UploadVocationFiles = new HashSet<UploadVocationFiles>();
        }
    
        public int UploadID { get; set; }
        public Nullable<int> ProkirixiID { get; set; }
        public Nullable<int> AitisiID { get; set; }
        public string TeacherAFM { get; set; }
        public Nullable<int> SchoolID { get; set; }
        public Nullable<System.DateTime> UploadDate { get; set; }
        public string UploadSummary { get; set; }
    
        public virtual ICollection<UploadVocationFiles> UploadVocationFiles { get; set; }
    }
}

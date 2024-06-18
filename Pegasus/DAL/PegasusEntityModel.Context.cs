﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class PegasusDBEntities : DbContext
    {
        public PegasusDBEntities()
            : base("name=PegasusDBEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<AITISIS_SCHOOLS> AITISIS_SCHOOLS { get; set; }
        public virtual DbSet<AITISIS_STATUS> AITISIS_STATUS { get; set; }
        public virtual DbSet<ROLES> ROLES { get; set; }
        public virtual DbSet<SYS_ANERGIA> SYS_ANERGIA { get; set; }
        public virtual DbSet<SYS_APOKLEISMOI> SYS_APOKLEISMOI { get; set; }
        public virtual DbSet<SYS_ARMY> SYS_ARMY { get; set; }
        public virtual DbSet<SYS_BASICEDUCATION> SYS_BASICEDUCATION { get; set; }
        public virtual DbSet<SYS_COMPUTERASEP> SYS_COMPUTERASEP { get; set; }
        public virtual DbSet<SYS_DIMOS> SYS_DIMOS { get; set; }
        public virtual DbSet<SYS_EIDIKOTITES> SYS_EIDIKOTITES { get; set; }
        public virtual DbSet<SYS_GENDERS> SYS_GENDERS { get; set; }
        public virtual DbSet<SYS_KLADOS> SYS_KLADOS { get; set; }
        public virtual DbSet<SYS_LANGUAGE> SYS_LANGUAGE { get; set; }
        public virtual DbSet<SYS_MSCPERIODS> SYS_MSCPERIODS { get; set; }
        public virtual DbSet<SYS_NOMISMA> SYS_NOMISMA { get; set; }
        public virtual DbSet<SYS_PEDAGOGICPERIOD> SYS_PEDAGOGICPERIOD { get; set; }
        public virtual DbSet<SYS_PERIFERIAKES> SYS_PERIFERIAKES { get; set; }
        public virtual DbSet<SYS_PERIFERIES> SYS_PERIFERIES { get; set; }
        public virtual DbSet<SYS_SCHOOLS> SYS_SCHOOLS { get; set; }
        public virtual DbSet<SYS_SCHOOLTYPES> SYS_SCHOOLTYPES { get; set; }
        public virtual DbSet<SYS_SCHOOLYEARS> SYS_SCHOOLYEARS { get; set; }
        public virtual DbSet<SYS_SOCIALGROUPS> SYS_SOCIALGROUPS { get; set; }
        public virtual DbSet<SYS_SPOUDES_TYPES> SYS_SPOUDES_TYPES { get; set; }
        public virtual DbSet<SYS_TEACH1_TYPES> SYS_TEACH1_TYPES { get; set; }
        public virtual DbSet<SYS_TEACH2_TYPES> SYS_TEACH2_TYPES { get; set; }
        public virtual DbSet<SYS_TEACHER_TYPES> SYS_TEACHER_TYPES { get; set; }
        public virtual DbSet<SYS_YESNO> SYS_YESNO { get; set; }
        public virtual DbSet<TEACHERS> TEACHERS { get; set; }
        public virtual DbSet<USER_ADMINS> USER_ADMINS { get; set; }
        public virtual DbSet<USER_TEACHERS> USER_TEACHERS { get; set; }
        public virtual DbSet<VD_EIDIKOTITES> VD_EIDIKOTITES { get; set; }
        public virtual DbSet<sqlTEACHER_AITISEIS> sqlTEACHER_AITISEIS { get; set; }
        public virtual DbSet<SYS_EPAGELMA> SYS_EPAGELMA { get; set; }
        public virtual DbSet<SYS_TAXFREE> SYS_TAXFREE { get; set; }
        public virtual DbSet<PROKIRIXIS_SCHOOLS> PROKIRIXIS_SCHOOLS { get; set; }
        public virtual DbSet<sqlPERIFERIES_SCHOOLS> sqlPERIFERIES_SCHOOLS { get; set; }
        public virtual DbSet<sqlUSER_SCHOOL> sqlUSER_SCHOOL { get; set; }
        public virtual DbSet<PROKIRIXIS_EIDIKOTITES> PROKIRIXIS_EIDIKOTITES { get; set; }
        public virtual DbSet<sqlEXP_FREELANCE_1> sqlEXP_FREELANCE_1 { get; set; }
        public virtual DbSet<sqlEXP_FREELANCE_FINAL> sqlEXP_FREELANCE_FINAL { get; set; }
        public virtual DbSet<sqlEXP_TEACHING_2> sqlEXP_TEACHING_2 { get; set; }
        public virtual DbSet<sqlEXP_VOCATION_1> sqlEXP_VOCATION_1 { get; set; }
        public virtual DbSet<sqlEXP_VOCATION_FINAL> sqlEXP_VOCATION_FINAL { get; set; }
        public virtual DbSet<SYS_EIDIKOTITES_GROUPS> SYS_EIDIKOTITES_GROUPS { get; set; }
        public virtual DbSet<PROKIRIXIS> PROKIRIXIS { get; set; }
        public virtual DbSet<SYS_PROKIRIXI_STATUS> SYS_PROKIRIXI_STATUS { get; set; }
        public virtual DbSet<sqlTEACHERS_IN_AITISEIS> sqlTEACHERS_IN_AITISEIS { get; set; }
        public virtual DbSet<USER_SCHOOLS> USER_SCHOOLS { get; set; }
        public virtual DbSet<SYS_EDUCLASSES> SYS_EDUCLASSES { get; set; }
        public virtual DbSet<SYS_ANERGIA_OLD> SYS_ANERGIA_OLD { get; set; }
        public virtual DbSet<sqlEXP_FREELANCE_1_OLD> sqlEXP_FREELANCE_1_OLD { get; set; }
        public virtual DbSet<sqlEXP_FREELANCE_FINAL_OLD> sqlEXP_FREELANCE_FINAL_OLD { get; set; }
        public virtual DbSet<sqlEXP_TEACHING_2_OLD> sqlEXP_TEACHING_2_OLD { get; set; }
        public virtual DbSet<sqlEXP_TEACHING_FINAL_OLD> sqlEXP_TEACHING_FINAL_OLD { get; set; }
        public virtual DbSet<sqlEXP_VOCATION_1_OLD> sqlEXP_VOCATION_1_OLD { get; set; }
        public virtual DbSet<sqlEXP_VOCATION_FINAL_OLD> sqlEXP_VOCATION_FINAL_OLD { get; set; }
        public virtual DbSet<sqlEXP_WORK_FINAL> sqlEXP_WORK_FINAL { get; set; }
        public virtual DbSet<sqlEXP_TEACHING_FINAL> sqlEXP_TEACHING_FINAL { get; set; }
        public virtual DbSet<sqlTEACHERS_WITH_AITISEIS_UNIQUE> sqlTEACHERS_WITH_AITISEIS_UNIQUE { get; set; }
        public virtual DbSet<sqlTEACHERS_WITH_AITISEIS> sqlTEACHERS_WITH_AITISEIS { get; set; }
        public virtual DbSet<SYS_EIDIKOTITES_OLD> SYS_EIDIKOTITES_OLD { get; set; }
        public virtual DbSet<SYS_KLADOS_ENIAIOS> SYS_KLADOS_ENIAIOS { get; set; }
        public virtual DbSet<SYS_LANGUAGELEVEL> SYS_LANGUAGELEVEL { get; set; }
        public virtual DbSet<sqlTEACHER_ACCOUNTS_INFO> sqlTEACHER_ACCOUNTS_INFO { get; set; }
        public virtual DbSet<sqlEIDIKOTITES_SELECTOR> sqlEIDIKOTITES_SELECTOR { get; set; }
        public virtual DbSet<APP_STATUS> APP_STATUS { get; set; }
        public virtual DbSet<REEDUCATION> REEDUCATION { get; set; }
        public virtual DbSet<sqlEIDIKOTITES_KU> sqlEIDIKOTITES_KU { get; set; }
        public virtual DbSet<sqlAITISEIS_WITH_EXPERIENCE> sqlAITISEIS_WITH_EXPERIENCE { get; set; }
        public virtual DbSet<sqlAITISEIS_WITHOUT_EXPERIENCE> sqlAITISEIS_WITHOUT_EXPERIENCE { get; set; }
        public virtual DbSet<sqlEIDIKOTITES_IN_SCHOOLS> sqlEIDIKOTITES_IN_SCHOOLS { get; set; }
        public virtual DbSet<sqlUSER_TEACHER_SELECT> sqlUSER_TEACHER_SELECT { get; set; }
        public virtual DbSet<TAXISNET> TAXISNET { get; set; }
        public virtual DbSet<UploadEnstaseis> UploadEnstaseis { get; set; }
        public virtual DbSet<UploadEnstaseisFiles> UploadEnstaseisFiles { get; set; }
        public virtual DbSet<UploadGeneralFiles> UploadGeneralFiles { get; set; }
        public virtual DbSet<UploadVocationFiles> UploadVocationFiles { get; set; }
        public virtual DbSet<UploadGeneral> UploadGeneral { get; set; }
        public virtual DbSet<UploadVocation> UploadVocation { get; set; }
        public virtual DbSet<UploadTeaching> UploadTeaching { get; set; }
        public virtual DbSet<UploadTeachingFiles> UploadTeachingFiles { get; set; }
        public virtual DbSet<xUploadedGeneralFiles> xUploadedGeneralFiles { get; set; }
        public virtual DbSet<xUploadedTeachingFiles> xUploadedTeachingFiles { get; set; }
        public virtual DbSet<xUploadedVocationFiles> xUploadedVocationFiles { get; set; }
        public virtual DbSet<AITISIS> AITISIS { get; set; }
        public virtual DbSet<AITISIS_ORIGINAL> AITISIS_ORIGINAL { get; set; }
        public virtual DbSet<SYS_PTYXIA_TYPES> SYS_PTYXIA_TYPES { get; set; }
        public virtual DbSet<SUPPORT> SUPPORT { get; set; }
        public virtual DbSet<EXP_FREELANCE> EXP_FREELANCE { get; set; }
        public virtual DbSet<EXP_TEACHING> EXP_TEACHING { get; set; }
        public virtual DbSet<EXP_VOCATIONAL> EXP_VOCATIONAL { get; set; }
    }
}

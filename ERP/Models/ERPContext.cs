using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using ERP.Models.Mapping;

namespace ERP.Models
{
    public partial class ERPContext : DbContext
    {
        static ERPContext()
        {
            Database.SetInitializer<ERPContext>(null);
        }

        public ERPContext()
            : base("Name=ERPContext")
        {
            this.Configuration.LazyLoadingEnabled = false;
        }

        public DbSet<EmployeeMaster> EmployeeMasters { get; set; }
        public DbSet<InOutEntry> InOutEntries { get; set; }
        public DbSet<tblABContact> tblABContacts { get; set; }
        public DbSet<tblABGroup> tblABGroups { get; set; }
        public DbSet<tblABGrp_Contact> tblABGrp_Contact { get; set; }
        public DbSet<tblABLanguage> tblABLanguages { get; set; }
        public DbSet<tblABTemplate> tblABTemplates { get; set; }
        public DbSet<tblApplyLeave> tblApplyLeaves { get; set; }
        public DbSet<tblARModule> tblARModules { get; set; }
        public DbSet<tblARPermissionAssigned> tblARPermissionAssigneds { get; set; }
        public DbSet<tblARSubModule> tblARSubModules { get; set; }
        public DbSet<tblBloodGroup> tblBloodGroups { get; set; }
        public DbSet<tblBrand> tblBrands { get; set; }
        public DbSet<tblCategory> tblCategories { get; set; }
        public DbSet<tblCountry> tblCountries { get; set; }
        public DbSet<tblCurrency> tblCurrencies { get; set; }
        public DbSet<tblDesignation> tblDesignations { get; set; }
        public DbSet<tblDesignationGroup> tblDesignationGroups { get; set; }
        public DbSet<tblDesignationParent> tblDesignationParents { get; set; }
        public DbSet<tblDocument> tblDocuments { get; set; }
        public DbSet<tblEmpAcedamicStatu> tblEmpAcedamicStatus { get; set; }
        public DbSet<tblEmpAttendance> tblEmpAttendances { get; set; }
        public DbSet<tblEmpCompanyInformation> tblEmpCompanyInformations { get; set; }
        public DbSet<tblEmpCredentialInformation> tblEmpCredentialInformations { get; set; }
        public DbSet<tblEmpDailyInOut> tblEmpDailyInOuts { get; set; }
        public DbSet<tblEmpDegree> tblEmpDegrees { get; set; }
        public DbSet<tblEmpDiscipline> tblEmpDisciplines { get; set; }
        public DbSet<tblEmpDocument> tblEmpDocuments { get; set; }
        public DbSet<tblEmpInstitute> tblEmpInstitutes { get; set; }
        public DbSet<tblEmpLoginInformation> tblEmpLoginInformations { get; set; }
        public DbSet<tblEmpPayRollInformation> tblEmpPayRollInformations { get; set; }
        public DbSet<tblEmpPersonalInformation> tblEmpPersonalInformations { get; set; }
        public DbSet<tblEmpQualificationInformation> tblEmpQualificationInformations { get; set; }
        public DbSet<tblEmpRelativeInformation> tblEmpRelativeInformations { get; set; }
        public DbSet<tblEmpRelativeRelation> tblEmpRelativeRelations { get; set; }
        public DbSet<tblEmpSource> tblEmpSources { get; set; }
        public DbSet<tblEmpUniversity> tblEmpUniversities { get; set; }
        public DbSet<tblEmpWorkExperience> tblEmpWorkExperiences { get; set; }
        public DbSet<tblEMSClient> tblEMSClients { get; set; }
        public DbSet<tblEMSGroup> tblEMSGroups { get; set; }
        public DbSet<tblEMSGroupClient> tblEMSGroupClients { get; set; }
        public DbSet<tblEMSMailSendPrepare> tblEMSMailSendPrepares { get; set; }
        public DbSet<tblEMSMailSendTemp> tblEMSMailSendTemps { get; set; }
        public DbSet<tblEMSMailSent> tblEMSMailSents { get; set; }
        public DbSet<tblEMSNewsletter> tblEMSNewsletters { get; set; }
        public DbSet<tblFestival> tblFestivals { get; set; }
        public DbSet<tblFestivalType> tblFestivalTypes { get; set; }
        public DbSet<tblHRDAccidentRecord> tblHRDAccidentRecords { get; set; }
        public DbSet<tblHRDBoilPlantCleaningRecord> tblHRDBoilPlantCleaningRecords { get; set; }
        public DbSet<tblHRDChemicalStorageInspectionLog> tblHRDChemicalStorageInspectionLogs { get; set; }
        public DbSet<tblHRDDDInPressMedia> tblHRDDDInPressMedias { get; set; }
        public DbSet<tblHRDFinancialAssisToDeathEmployee> tblHRDFinancialAssisToDeathEmployees { get; set; }
        public DbSet<tblHRDFireExtinguiserLogBook> tblHRDFireExtinguiserLogBooks { get; set; }
        public DbSet<tblHRDFireHydrantandSprinklerSystem> tblHRDFireHydrantandSprinklerSystems { get; set; }
        public DbSet<tblHRDFirstAIdLogBook> tblHRDFirstAIdLogBooks { get; set; }
        public DbSet<tblHRDIssuedDocument> tblHRDIssuedDocuments { get; set; }
        public DbSet<tblHRDMedicalHelp> tblHRDMedicalHelps { get; set; }
        public DbSet<tblHRDPPEIssueRegister> tblHRDPPEIssueRegisters { get; set; }
        public DbSet<tblHRDPressMediaExpens> tblHRDPressMediaExpenses { get; set; }
        public DbSet<tblHRDQuarterlyManagementMeeting> tblHRDQuarterlyManagementMeetings { get; set; }
        public DbSet<tblHRDSafetyTrainingRecord> tblHRDSafetyTrainingRecords { get; set; }
        public DbSet<tblHRDSocialWelfareExpense> tblHRDSocialWelfareExpenses { get; set; }
        public DbSet<tblHRDTrainingsAndMeeting> tblHRDTrainingsAndMeetings { get; set; }
        public DbSet<tblINVClient> tblINVClients { get; set; }
        public DbSet<tblINVClientPerson> tblINVClientPersons { get; set; }
        public DbSet<tblINVClientSource> tblINVClientSources { get; set; }
        public DbSet<tblINVConversation> tblINVConversations { get; set; }
        public DbSet<tblINVDocument> tblINVDocuments { get; set; }
        public DbSet<tblInvetory> tblInvetories { get; set; }
        public DbSet<tblInvetoryDetail> tblInvetoryDetails { get; set; }
        public DbSet<tblINVInquiry> tblINVInquiries { get; set; }
        public DbSet<tblINVInvoice> tblINVInvoices { get; set; }
        public DbSet<tblINVInvoiceTax> tblINVInvoiceTaxes { get; set; }
        public DbSet<tblINVLogInquiry> tblINVLogInquiries { get; set; }
        public DbSet<tblINVMilestone> tblINVMilestones { get; set; }
        public DbSet<tblINVPayment> tblINVPayments { get; set; }
        public DbSet<tblINVProject> tblINVProjects { get; set; }
        public DbSet<tblINVProposal> tblINVProposals { get; set; }
        public DbSet<tblINVTaxMaster> tblINVTaxMasters { get; set; }
        public DbSet<tblLocation> tblLocations { get; set; }
        public DbSet<tblLOGPageActivity> tblLOGPageActivities { get; set; }
        public DbSet<tblPMSActivityLogProject> tblPMSActivityLogProjects { get; set; }
        public DbSet<tblPMSActivityLogProjectUser> tblPMSActivityLogProjectUsers { get; set; }
        public DbSet<tblPMSComment> tblPMSComments { get; set; }
        public DbSet<tblPMSCommentFile> tblPMSCommentFiles { get; set; }
        public DbSet<tblPMSModule> tblPMSModules { get; set; }
        public DbSet<tblPMSProject> tblPMSProjects { get; set; }
        public DbSet<tblPMSProjectUser> tblPMSProjectUsers { get; set; }
        public DbSet<tblPMSToDo> tblPMSToDoes { get; set; }
        public DbSet<tblRole> tblRoles { get; set; }
        public DbSet<tblSRAMC> tblSRAMCs { get; set; }
        public DbSet<tblSRExtra> tblSRExtras { get; set; }
        public DbSet<tblSRFloor> tblSRFloors { get; set; }
        public DbSet<tblSRMachine> tblSRMachines { get; set; }
        public DbSet<tblSRParameter> tblSRParameters { get; set; }
        public DbSet<tblSRPart> tblSRParts { get; set; }
        public DbSet<tblSRPartIssue> tblSRPartIssues { get; set; }
        public DbSet<tblSRPurchase> tblSRPurchases { get; set; }
        public DbSet<tblSRRepair> tblSRRepairs { get; set; }
        public DbSet<tblSRSubType> tblSRSubTypes { get; set; }
        public DbSet<tblSRType> tblSRTypes { get; set; }
        public DbSet<tblState> tblStates { get; set; }
        public DbSet<tblSubCategory> tblSubCategories { get; set; }
        public DbSet<tblTechnology> tblTechnologies { get; set; }
        public DbSet<tblTechnologiesGroup> tblTechnologiesGroups { get; set; }
        public DbSet<tblVendor> tblVendors { get; set; }
        public DbSet<teledata> teledatas { get; set; }
        public DbSet<VisitorInOut> VisitorInOuts { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new EmployeeMasterMap());
            modelBuilder.Configurations.Add(new InOutEntryMap());
            modelBuilder.Configurations.Add(new tblABContactMap());
            modelBuilder.Configurations.Add(new tblABGroupMap());
            modelBuilder.Configurations.Add(new tblABGrp_ContactMap());
            modelBuilder.Configurations.Add(new tblABLanguageMap());
            modelBuilder.Configurations.Add(new tblABTemplateMap());
            modelBuilder.Configurations.Add(new tblApplyLeaveMap());
            modelBuilder.Configurations.Add(new tblARModuleMap());
            modelBuilder.Configurations.Add(new tblARPermissionAssignedMap());
            modelBuilder.Configurations.Add(new tblARSubModuleMap());
            modelBuilder.Configurations.Add(new tblBloodGroupMap());
            modelBuilder.Configurations.Add(new tblBrandMap());
            modelBuilder.Configurations.Add(new tblCategoryMap());
            modelBuilder.Configurations.Add(new tblCountryMap());
            modelBuilder.Configurations.Add(new tblCurrencyMap());
            modelBuilder.Configurations.Add(new tblDesignationMap());
            modelBuilder.Configurations.Add(new tblDesignationGroupMap());
            modelBuilder.Configurations.Add(new tblDesignationParentMap());
            modelBuilder.Configurations.Add(new tblDocumentMap());
            modelBuilder.Configurations.Add(new tblEmpAcedamicStatuMap());
            modelBuilder.Configurations.Add(new tblEmpAttendanceMap());
            modelBuilder.Configurations.Add(new tblEmpCompanyInformationMap());
            modelBuilder.Configurations.Add(new tblEmpCredentialInformationMap());
            modelBuilder.Configurations.Add(new tblEmpDailyInOutMap());
            modelBuilder.Configurations.Add(new tblEmpDegreeMap());
            modelBuilder.Configurations.Add(new tblEmpDisciplineMap());
            modelBuilder.Configurations.Add(new tblEmpDocumentMap());
            modelBuilder.Configurations.Add(new tblEmpInstituteMap());
            modelBuilder.Configurations.Add(new tblEmpLoginInformationMap());
            modelBuilder.Configurations.Add(new tblEmpPayRollInformationMap());
            modelBuilder.Configurations.Add(new tblEmpPersonalInformationMap());
            modelBuilder.Configurations.Add(new tblEmpQualificationInformationMap());
            modelBuilder.Configurations.Add(new tblEmpRelativeInformationMap());
            modelBuilder.Configurations.Add(new tblEmpRelativeRelationMap());
            modelBuilder.Configurations.Add(new tblEmpSourceMap());
            modelBuilder.Configurations.Add(new tblEmpUniversityMap());
            modelBuilder.Configurations.Add(new tblEmpWorkExperienceMap());
            modelBuilder.Configurations.Add(new tblEMSClientMap());
            modelBuilder.Configurations.Add(new tblEMSGroupMap());
            modelBuilder.Configurations.Add(new tblEMSGroupClientMap());
            modelBuilder.Configurations.Add(new tblEMSMailSendPrepareMap());
            modelBuilder.Configurations.Add(new tblEMSMailSendTempMap());
            modelBuilder.Configurations.Add(new tblEMSMailSentMap());
            modelBuilder.Configurations.Add(new tblEMSNewsletterMap());
            modelBuilder.Configurations.Add(new tblFestivalMap());
            modelBuilder.Configurations.Add(new tblFestivalTypeMap());
            modelBuilder.Configurations.Add(new tblHRDAccidentRecordMap());
            modelBuilder.Configurations.Add(new tblHRDBoilPlantCleaningRecordMap());
            modelBuilder.Configurations.Add(new tblHRDChemicalStorageInspectionLogMap());
            modelBuilder.Configurations.Add(new tblHRDDDInPressMediaMap());
            modelBuilder.Configurations.Add(new tblHRDFinancialAssisToDeathEmployeeMap());
            modelBuilder.Configurations.Add(new tblHRDFireExtinguiserLogBookMap());
            modelBuilder.Configurations.Add(new tblHRDFireHydrantandSprinklerSystemMap());
            modelBuilder.Configurations.Add(new tblHRDFirstAIdLogBookMap());
            modelBuilder.Configurations.Add(new tblHRDIssuedDocumentMap());
            modelBuilder.Configurations.Add(new tblHRDMedicalHelpMap());
            modelBuilder.Configurations.Add(new tblHRDPPEIssueRegisterMap());
            modelBuilder.Configurations.Add(new tblHRDPressMediaExpensMap());
            modelBuilder.Configurations.Add(new tblHRDQuarterlyManagementMeetingMap());
            modelBuilder.Configurations.Add(new tblHRDSafetyTrainingRecordMap());
            modelBuilder.Configurations.Add(new tblHRDSocialWelfareExpenseMap());
            modelBuilder.Configurations.Add(new tblHRDTrainingsAndMeetingMap());
            modelBuilder.Configurations.Add(new tblINVClientMap());
            modelBuilder.Configurations.Add(new tblINVClientPersonMap());
            modelBuilder.Configurations.Add(new tblINVClientSourceMap());
            modelBuilder.Configurations.Add(new tblINVConversationMap());
            modelBuilder.Configurations.Add(new tblINVDocumentMap());
            modelBuilder.Configurations.Add(new tblInvetoryMap());
            modelBuilder.Configurations.Add(new tblInvetoryDetailMap());
            modelBuilder.Configurations.Add(new tblINVInquiryMap());
            modelBuilder.Configurations.Add(new tblINVInvoiceMap());
            modelBuilder.Configurations.Add(new tblINVInvoiceTaxMap());
            modelBuilder.Configurations.Add(new tblINVLogInquiryMap());
            modelBuilder.Configurations.Add(new tblINVMilestoneMap());
            modelBuilder.Configurations.Add(new tblINVPaymentMap());
            modelBuilder.Configurations.Add(new tblINVProjectMap());
            modelBuilder.Configurations.Add(new tblINVProposalMap());
            modelBuilder.Configurations.Add(new tblINVTaxMasterMap());
            modelBuilder.Configurations.Add(new tblLocationMap());
            modelBuilder.Configurations.Add(new tblLOGPageActivityMap());
            modelBuilder.Configurations.Add(new tblPMSActivityLogProjectMap());
            modelBuilder.Configurations.Add(new tblPMSActivityLogProjectUserMap());
            modelBuilder.Configurations.Add(new tblPMSCommentMap());
            modelBuilder.Configurations.Add(new tblPMSCommentFileMap());
            modelBuilder.Configurations.Add(new tblPMSModuleMap());
            modelBuilder.Configurations.Add(new tblPMSProjectMap());
            modelBuilder.Configurations.Add(new tblPMSProjectUserMap());
            modelBuilder.Configurations.Add(new tblPMSToDoMap());
            modelBuilder.Configurations.Add(new tblRoleMap());
            modelBuilder.Configurations.Add(new tblSRAMCMap());
            modelBuilder.Configurations.Add(new tblSRExtraMap());
            modelBuilder.Configurations.Add(new tblSRFloorMap());
            modelBuilder.Configurations.Add(new tblSRMachineMap());
            modelBuilder.Configurations.Add(new tblSRParameterMap());
            modelBuilder.Configurations.Add(new tblSRPartMap());
            modelBuilder.Configurations.Add(new tblSRPartIssueMap());
            modelBuilder.Configurations.Add(new tblSRPurchaseMap());
            modelBuilder.Configurations.Add(new tblSRRepairMap());
            modelBuilder.Configurations.Add(new tblSRSubTypeMap());
            modelBuilder.Configurations.Add(new tblSRTypeMap());
            modelBuilder.Configurations.Add(new tblStateMap());
            modelBuilder.Configurations.Add(new tblSubCategoryMap());
            modelBuilder.Configurations.Add(new tblTechnologyMap());
            modelBuilder.Configurations.Add(new tblTechnologiesGroupMap());
            modelBuilder.Configurations.Add(new tblVendorMap());
            modelBuilder.Configurations.Add(new teledataMap());
            modelBuilder.Configurations.Add(new VisitorInOutMap());
        }

    }
}

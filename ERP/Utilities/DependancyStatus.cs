using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ERP.Models;
namespace ERP.Utilities
{
    public class DependancyStatus
    {
        private static ERPContext db;

        #region Dependancy for Category Status
        /// <summary>
        /// Return false if child record exist. 
        /// </summary>
        /// <param name="CategoryId"></param>
        /// <returns></returns>
        public static bool CategoryStatus(int CategoryId)
        {
            db = new ERPContext();
            var count = 0;
            count = db.tblSubCategories.Where(z => z.CategoryId == CategoryId).Count();
            if (count > 0)
                return false;
            count = db.tblInvetories.Where(z => z.CategoryId == CategoryId).Count();
            if (count > 0)
                return false;
            count = db.tblInvetoryDetails.Where(z => z.CategoryId == CategoryId).Count();
            if (count > 0)
                return false;

            return true;
        }
        #endregion

        #region Dependancy for Location Status
        /// <summary>
        /// Return false if child record exist. 
        /// </summary>
        /// <param name="LocationId"></param>
        /// <returns></returns>
        public static bool LocationStatus(int LocationId)
        {
            db = new ERPContext();
            var count = 0;
            count = db.tblInvetories.Where(z => z.LocationId == LocationId).Count();
            if (count > 0)
                return false;
            return true;
        }
        #endregion

        #region Dependancy for Vendor Status
        /// <summary>
        /// Return false if child record exist. 
        /// </summary>
        /// <param name="VendorId"></param>
        /// <returns></returns>
        public static bool VendorStatus(int VendorId)
        {
            db = new ERPContext();
            var count = 0;
            count = db.tblInvetories.Where(z => z.VendorId == VendorId).Count();
            if (count > 0)
                return false;
            return true;
        }
        #endregion

        #region Dependancy for Brand Status

        /// <summary>
        /// Check if any child exist for this brand.
        /// Return false if any.
        /// </summary>
        /// <param name="BrandId"></param>
        /// <returns></returns>
        public static bool BrandStatus(int BrandId)
        {
            db = new ERPContext();
            var count = 0;
            count = db.tblInvetories.Where(z => z.BrandId == BrandId).Count();
            if (count > 0)
                return false;
            count = db.tblInvetoryDetails.Where(z => z.BrandId == BrandId).Count();
            if (count > 0)
                return false;

            return true;
        }
        #endregion

        #region Dependancy for SubCategory Status

        /// <summary>
        /// Check if any child exist for this SubCategory.
        /// Return false if any.
        /// </summary>
        /// <param name="SubCategoryId"></param>
        /// <returns></returns>
        public static bool SubCategoryStatus(int SubCategoryId)
        {
            db = new ERPContext();
            var count = 0;
            count = db.tblInvetories.Where(z => z.SubCategoryId == SubCategoryId).Count();
            if (count > 0)
                return false;
            count = db.tblInvetoryDetails.Where(z => z.SubCategoryId == SubCategoryId).Count();
            if (count > 0)
                return false;

            return true;
        }
        #endregion

        #region Dependancy for TechnologiesGroup Status
        /// <summary>
        /// Check if any child exist for this Technology.
        /// Return false if any.
        /// </summary>
        /// <param name="TechnologyId"></param>
        /// <returns></returns>
        public static bool TechnologiesGroupStatus(int TechnologiesGroupId)
        {
            db = new ERPContext();
            var count = 0;
            count = db.tblTechnologies.Where(z => z.TechnologiesGroupId == TechnologiesGroupId).Count();
            if (count > 0)
                return false;
            count = db.tblEmpCompanyInformations.Where(z => z.TeamId == TechnologiesGroupId).Count();
            if (count > 0)
                return false;

            return true;
        }
        #endregion

        #region Dependancy for DesignationParent Status
        /// <summary>
        /// Check if any child exist for this DesignationParent.
        /// Return false if any.
        /// </summary>
        /// <param name="TechnologyId"></param>
        /// <returns></returns>
        public static bool DesignationParentStatus(int DesignationParentId)
        {
            db = new ERPContext();
            var count = 0;
            count = db.tblDesignations.Where(z => z.DesignationParentId == DesignationParentId).Count();
            if (count > 0)
                return false;

            return true;
        }
        #endregion

        #region Dependancy for DesignationGroup Status
        /// <summary>
        /// Check if any child exist for this DesignationGroup.
        /// Return false if any.
        /// </summary>
        /// <param name="TechnologyId"></param>
        /// <returns></returns>
        public static bool DesignationGroupStatus(int DesignationGroupId)
        {
            db = new ERPContext();
            var count = 0;
            count = db.tblDesignations.Where(z => z.DesignationGroupId == DesignationGroupId).Count();
            if (count > 0)
                return false;

            return true;
        }
        #endregion

        #region Dependancy for Designation Status
        /// <summary>
        /// Check if any child exist for this Designation.
        /// Return false if any.
        /// </summary>
        /// <param name="DesignationId"></param>
        /// <returns></returns>
        public static bool DesignationStatus(int DesignationId)
        {
            db = new ERPContext();
            var count = 0;
            count = db.tblEmpCompanyInformations.Where(z => z.DesignationId == DesignationId).Count();
            if (count > 0)
                return false;

            return true;
        }
        #endregion

        #region Dependancy for FestivalType Status
        /// <summary>
        /// Check if any child exist for this FestivalType.
        /// Return false if any.
        /// </summary>
        /// <param name="TechnologyId"></param>
        /// <returns></returns>
        public static bool FestivalTypeStatus(int FestivalTypeId)
        {
            db = new ERPContext();
            var count = 0;
            count = db.tblFestivals.Where(z => z.FestivalTypeId == FestivalTypeId).Count();
            if (count > 0)
                return false;

            return true;
        }
        #endregion

        #region Dependancy for Roles
        /// <summary>
        /// Check if any child exist for this Role.
        /// Return false if any.
        /// </summary>
        /// <param name="RoleId"></param>
        /// <returns></returns>
        public static bool RoleStatus(int RoleId)
        {
            db = new ERPContext();
            var count = 0;
            count = db.tblEmpCompanyInformations.Where(z => z.RolesId == RoleId).Count();
            if (count > 0)
                return false;

            return true;
        }
        #endregion

        #region Dependancy for Documents
        /// <summary>
        /// Check if any child exist for this Document.
        /// Return false if any.
        /// </summary>
        /// <param name="DocumentId"></param>
        /// <returns></returns>
        public static bool DocumentStatus(int DocumentId)
        {
            db = new ERPContext();
            var count = 0;
            count = db.tblEmpDocuments.Where(z => z.DocumentId == DocumentId).Count();
            if (count > 0)
                return false;
            count = db.tblHRDIssuedDocuments.Where(z => z.DocumentTypeId == DocumentId).Count();
            if (count > 0)
                return false;
            count = db.tblINVDocuments.Where(z => z.DocId == DocumentId && z.IsActive == true).Count();
            if (count > 0)
                return false;
            return true;
        }
        #endregion

        #region Dependancy for Module
        /// <summary>
        /// Check if any child exist for this Module.
        /// Return false if any.
        /// </summary>
        /// <param name="ModuleId"></param>
        /// <returns></returns>
        public static bool ModuleStatus(int ModuleId)
        {
            db = new ERPContext();
            var count = 0;
            count = db.tblARSubModules.Where(z => z.ModuleId == ModuleId).Count();
            if (count > 0)
                return false;
            count = db.tblARPermissionAssigneds.Where(z => z.ModuleId == ModuleId).Count();
            if (count > 0)
                return false;
            return true;
        }
        #endregion

        #region Dependancy for SR Type
        /// <summary>
        /// Check if any child exist for this SR Type.
        /// Return false if any.
        /// </summary>
        /// <param name="SRTypeid"></param>
        /// <returns></returns>
        public static bool SRTypeStatus(int SRTypeid)
        {
            db = new ERPContext();
            var count = 0;
            //count = db.tblSRSubTypes.Where(z => z.TypeId == SRTypeid).Count();
            if (db.tblSRSubTypes.Where(z => z.TypeId == SRTypeid).Count() > 0 || db.tblSRMachines.Where(z => z.TypeId == SRTypeid).Count() > 0)
                return false;

            return true;
        }
        #endregion

        #region Dependancy for SR Sub-Type
        /// <summary>
        /// Check if any child exist for this SR Sub-Type.
        /// Return false if any.
        /// </summary>
        /// <param name="SRSubTypeid"></param>
        /// <returns></returns>
        public static bool SRSubTypeStatus(int SRSubTypeid)
        {
            db = new ERPContext();
            //var count = 0;
            //count = db.tblSRParameters.Where(z => z.SubTypeId == SRSubTypeid).Count();
            if (db.tblSRParameters.Where(z => z.SubTypeId == SRSubTypeid).Count() > 0 || db.tblSRMachines.Where(z => z.SubTypeId == SRSubTypeid).Count() > 0)
                return false;
            return true;
        }
        #endregion

        #region Dependancy for SR Parameters
        /// <summary>
        /// Check if any child exist for this SR Parameters
        /// Return false if any.
        /// </summary>
        /// <param name="SRParameterId"></param>
        /// <returns></returns>
        public static bool SRParameterStatus(int SRParameterId)
        {
            db = new ERPContext();
            var count = 0;
            count = db.tblSRMachines.Where(z => z.ParameterId == SRParameterId).Count();
            if (count > 0)
                return false;
            return true;
        }
        #endregion

        #region Dependancy for SR Machine
        /// <summary>
        /// Check if any child exist for this Machine
        /// Return false if any.
        /// </summary>
        /// <param name="SRMachineId"></param>
        /// <returns></returns>
        public static bool SRMachineStatus(int SRMachineId)
        {
            db = new ERPContext();
            if (db.tblSRFloors.Where(z => z.MachineId == SRMachineId).Count() > 0 || db.tblSRPartIssues.Where(z => z.MachineId == SRMachineId).Count() > 0 || db.tblSRAMCs.Where(z => z.MachineId == SRMachineId).Count() > 0 || db.tblSRRepairs.Where(z => z.PartId == SRMachineId).Count() >0)
                return false;
            return true;
        }
        #endregion

        #region Dependancy for SR Part
        /// <summary>
        /// Check if any child exist for this Machine
        /// Return false if any.
        /// </summary>
        /// <param name="SRPartId"></param>
        /// <returns></returns>
        public static bool SRPartStatus(int SRPartId)
        {
            db = new ERPContext();
            var count = 0;
           // count = db.tblSRPartIssues.Where(z => z.PartId == SRPartId).Count();
            if (db.tblSRPartIssues.Where(z => z.PartId == SRPartId).Count() > 0 || db.tblSRPurchases.Where(z => z.PartId == SRPartId).Count() > 0 || db.tblSRRepairs.Where(z => z.PartId == SRPartId).Count()>0)
                return false;
            return true;
        }
        #endregion

        #region Dependancy for INVClientSource
        public static bool INVClientSource(int clientSourceId)
        {
            db = new ERPContext();
            var count = 0;
            if (db.tblINVClients.Where(z => z.FKSourceId == clientSourceId).Count() > 0)
                return false;
            return true;
        }
        #endregion

        #region Dependancy for Invoice
        /// <summary>
        /// Check if any payment received exist for this Document.
        /// Return false if any.
        /// </summary>
        /// <param name="DocumentId"></param>
        /// <returns></returns>
        public static bool InvoicePayment(int invoicetId)
        {
            db = new ERPContext();
            var count = 0;
            count = db.tblINVPayments.Where(z => z.FKInvoiceId == invoicetId && z.IsActive == true && z.IsDeleted == false).Count();
            if (count > 0)
                return false;
            return true;
        }
        #endregion
    }
}
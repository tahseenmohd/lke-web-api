using Exportal_DAL.Logging;
using Exportal_DAL.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LKE_DAL.Repositories
{
    public class RDMEntity : RepositoryBase
    {


        private IConnectionFactory dbFactory;
        public RDMEntity(IConnectionFactory factory)
        {
            dbFactory = factory;
        }

        //public List<Entity> GetDashboardLKE_LoginEntities1(int userId)
        //{
        
        //    List<Entity> entity = new List<Entity>();
        //    try
        //    {


        //        var parameters = new List<SqlParameter>();
        //        parameters.Add(new SqlParameter("@UserID", userId));
        //        DataSet dsResponse = null;

        //        dsResponse = base.ReturnDataSet(dbFactory, "PEXP_GET_ENTITY_LIST", parameters);

        //        if (dsResponse != null || dsResponse.Tables[0].Rows.Count != 0)
        //        {
        //            foreach (DataRow dr in dsResponse.Tables[0].Rows)
        //            {
        //                entity.Add(new Entity(dr));
        //            }
        //        }
             
        //        return entity;
        //    }
        //    catch(Exception E)
        //    {
        //        Logger.Write(E);
        //        return entity;
        //    }          
        //}
        //public DMEditEntity GetEditEntity(int EntityID)
        //{

        //    DMEditEntity editEntity = new DMEditEntity();
        //    var parameters = new List<SqlParameter>();
        //    parameters.Add(new SqlParameter("@Entity_ID", EntityID));
        //    DataSet dsResponse = null;

        //    dsResponse = base.ReturnDataSet(dbFactory, "PEXP_VIEW_ENTITY_ANNUALMAINTAINANCE", parameters);

        //    if (dsResponse != null || dsResponse.Tables[0].Rows.Count != 0)
        //    {
        //        editEntity.EntityInfo = new Entity(dsResponse.Tables[0].Rows[0]);
        //        foreach (DataRow dr in dsResponse.Tables[1].Rows)
        //        {
        //            editEntity.AddAnnualMaintenance(new EntityAnnualInformation(dr));

        //        }
        //    }

        //    return editEntity;
        //}
        //public List<EntityAnnualInformation_DISCReturnitems> GetAnnualInformation_DISCReturnitems(int EntityID)
        //{

        //    List<EntityAnnualInformation_DISCReturnitems> entityAIDisc = new List<EntityAnnualInformation_DISCReturnitems>();

        //    var parameters = new List<SqlParameter>();
        //    parameters.Add(new SqlParameter("@Entity_ID", EntityID));
        //    DataSet dsResponse = null;

        //    dsResponse = base.ReturnDataSet(dbFactory, "PEXP_VIEW_ANNUALICDISCMAINTAINANCE", parameters);

        //    if (dsResponse != null || dsResponse.Tables[0].Rows.Count != 0)
        //    {

        //        foreach (DataRow dr in dsResponse.Tables[0].Rows)
        //        {
        //            entityAIDisc.Add(new EntityAnnualInformation_DISCReturnitems(dr));
        //        }
        //    }

        //    return entityAIDisc;
        //}
        //public List<EntityOwnership> GetEntityOwnership(int EntityID)
        //{

        //    List<EntityOwnership> editOwnership = new List<EntityOwnership>();

        //    var parameters = new List<SqlParameter>();
        //    parameters.Add(new SqlParameter("@Entity_ID", EntityID));
        //    DataSet dsResponse = null;

        //    dsResponse = base.ReturnDataSet(dbFactory, "PEXP_VIEW_OWNERSHIP", parameters);

        //    if (dsResponse != null || dsResponse.Tables[0].Rows.Count != 0)
        //    {
        //        foreach (DataRow dr in dsResponse.Tables[0].Rows)
        //        {
        //            editOwnership.Add(new EntityOwnership(dr));
        //        }
        //    }
        //    return editOwnership;
        //}
        //public List<EntityRelatedSupplier> GetEntityRelatedSupplier(int EntityID)
        //{

        //    List<EntityRelatedSupplier> entityRelatedSupplier = new List<EntityRelatedSupplier>();
        //    var parameters = new List<SqlParameter>();
        //    parameters.Add(new SqlParameter("@Entity_ID", EntityID));
        //    DataSet dsResponse = null;

        //    dsResponse = base.ReturnDataSet(dbFactory, "PEXP_VIEW_RELATEDSUPPLIER", parameters);

        //    if (dsResponse != null || dsResponse.Tables[0].Rows.Count != 0)
        //    {

        //        foreach (DataRow dr in dsResponse.Tables[0].Rows)
        //        {
        //            entityRelatedSupplier.Add(new EntityRelatedSupplier(dr));
        //        }
        //    }
        //    return entityRelatedSupplier;
        //}
        //public List<EntityPointofContact> GetEntityPointofContact(int EntityID)
        //{

        //    List<EntityPointofContact> editPointOfContact = new List<EntityPointofContact>();

        //    var parameters = new List<SqlParameter>();
        //    parameters.Add(new SqlParameter("@Entity_ID", EntityID));
        //    DataSet dsResponse = null;

        //    dsResponse = base.ReturnDataSet(dbFactory, "PEXP_VIEW_POINTOFCONTACTS", parameters);

        //    if (dsResponse != null || dsResponse.Tables[0].Rows.Count != 0)
        //    {
        //        foreach (DataRow dr in dsResponse.Tables[0].Rows)
        //        {
        //            editPointOfContact.Add(new EntityPointofContact(dr));
        //        }
        //    }

        //    return editPointOfContact;
        //}
        //public List<EntityPointofContact_Roles> GetEntityPointofContact_Roles(int EntityID)
        //{

        //    List<EntityPointofContact_Roles> DMEditEntity = new List<EntityPointofContact_Roles>();

        //    var parameters = new List<SqlParameter>();
        //    parameters.Add(new SqlParameter("@Entity_ID", EntityID));
        //    DataSet dsResponse = null;

        //    dsResponse = base.ReturnDataSet(dbFactory, "PEXP_VIEW_LEADERSHIP", parameters);

        //    if (dsResponse != null || dsResponse.Tables[0].Rows.Count != 0)
        //    {
        //        foreach (DataRow dr in dsResponse.Tables[0].Rows)
        //        {
        //            DMEditEntity.Add(new EntityPointofContact_Roles(dr));
        //        }
        //    }
        //    return DMEditEntity;
        //}
        //public List<Documents> GetDocuments(int EntityID)
        //{
        //    List<Documents> documents = new List<Documents>();

        //    var parameters = new List<SqlParameter>();
        //    parameters.Add(new SqlParameter("@Entity_ID", EntityID));
        //    DataSet dsResponse = null;

        //    dsResponse = base.ReturnDataSet(dbFactory, "PEXP_VIEW_DOCUMENTS", parameters);

        //    if (dsResponse != null || dsResponse.Tables[0].Rows.Count != 0)
        //    {
        //        foreach (DataRow dr in dsResponse.Tables[0].Rows)
        //        {
        //            Documents doc = new Documents(dr);
        //            DataRow[] drFiles = dsResponse.Tables[1].Select("DocumentID=" + doc.ID.ToString());
        //            foreach (DataRow drfile in drFiles)
        //            {
        //                doc.AddFiles(new DocumentFiles(drfile));
        //            }
        //            documents.Add(doc);

        //        }
        //    }
        //    return documents;
        //}
        //public LookUpEditDashboard GetLookUps(int EntityID)
        //{

        //    LookUpEditDashboard DMLookups = new LookUpEditDashboard();
        //    var parameters = new List<SqlParameter>();
        //    parameters.Add(new SqlParameter("@Entity_ID", EntityID));
        //    DataSet dsResponse = null;

        //    dsResponse = base.ReturnDataSet(dbFactory, "PEXP_LOOKUPS_ENTITYTYPES_BAC_COUNTRY_STATE", parameters);

        //    if (dsResponse == null || dsResponse.Tables[0].Rows.Count == 0)
        //    {
        //        return DMLookups;

        //    }
        //    else
        //    {
        //        foreach (DataRow dr in dsResponse.Tables[0].Rows)
        //        {
        //            DMLookups.AddEntityTypes(new EntityTypes(dr));

        //        } 
        //        foreach (DataRow dr in dsResponse.Tables[1].Rows)
        //        {
        //            DMLookups.AddBusinessActivityCodes(new BusinessActivityCodes(dr));

        //        } 
               
        //        foreach (DataRow dr in dsResponse.Tables[2].Rows)
        //        {
        //            DMLookups.AddCountry(new Country(dr));

        //        }
        //        foreach (DataRow dr in dsResponse.Tables[3].Rows)
        //        {
        //            DMLookups.AddCurrency(new Currency(dr));

        //        }
        //        foreach (DataRow dr in dsResponse.Tables[4].Rows)
        //        {
        //            DMLookups.AddStates(new States(dr));

        //        }
        
        //        foreach (DataRow dr in dsResponse.Tables[5].Rows)
        //        {
        //            DMLookups.GetYearGroup(new YearGroup(dr));
        //        }

        //        foreach (DataRow dr in dsResponse.Tables[6].Rows)
        //        {
        //            DMLookups.GetCPAFirm(new CPAFirm(dr));
        //        }
        //        foreach (DataRow dr in dsResponse.Tables[7].Rows)
        //        {
        //            DMLookups.GetCase(new Case(dr));

        //        }

        //        foreach (DataRow dr in dsResponse.Tables[8].Rows)
        //        {
        //            DMLookups.GetDocumentGroup(new DocumentGroup(dr));

        //        }
        //        foreach (DataRow dr in dsResponse.Tables[9].Rows)
        //        {
        //            DMLookups.GetDocumentType(new DocumentType(dr));

        //        }
        //        foreach (DataRow dr in dsResponse.Tables[10].Rows)
        //        {
        //            DMLookups.GetEntityLegalNameLookUp(new EntityLegalNameLookUp(dr));

        //        }
        //        foreach (DataRow dr in dsResponse.Tables[11].Rows)
        //        {
        //            DMLookups.GetClient(new Client(dr));

        //        }
        //        foreach (DataRow dr in dsResponse.Tables[12].Rows)
        //        {
        //            DMLookups.GetShareType(new ShareTypes(dr));

        //        }
        //        foreach (DataRow dr in dsResponse.Tables[13].Rows)
        //        {
        //            DMLookups.GetYear(new Year(dr));

        //        }
        //        foreach (DataRow dr in dsResponse.Tables[14].Rows)
        //        {
        //            DMLookups.GetRole(new Role(dr));
        //        }
        //        foreach (DataRow dr in dsResponse.Tables[15].Rows)
        //        {
        //            DMLookups.GetPOCDisplayNameLookUp(new POCDisplayNameLookup(dr));

        //        }
        //        foreach (DataRow dr in dsResponse.Tables[16].Rows)
        //        {
        //            DMLookups.GetEntityProcessManagement(new EntityProcessManagement(dr));

        //        }
        //        return DMLookups;

        //    }
        //}


        //public int UpdateInsertEntity(Entity E)
        //{

        //    var parameters = new List<SqlParameter>();
        //    int newID = 0;
        //    parameters.Add(new SqlParameter("@DisplayName", E.DisplayName));
        //    parameters.Add(new SqlParameter("@LegalName", E.LegalName));
        //    parameters.Add(new SqlParameter("@EntityType", E.EntityType));
        //    parameters.Add(new SqlParameter("@TaxID", E.TaxID));
        //    parameters.Add(new SqlParameter("@BAC", E.BAC));
        //    parameters.Add(new SqlParameter("@Active", E.Active));
        //    parameters.Add(new SqlParameter("@EvergreenDiv", E.EvergreenDiv));
        //    parameters.Add(new SqlParameter("@MailAddressL1", E.MailAddressL1));
        //    parameters.Add(new SqlParameter("@MailAddressL2", E.MailAddressL2));
        //    parameters.Add(new SqlParameter("@MailCity", E.MailCity));
        //    parameters.Add(new SqlParameter("@MailZip", E.MailZip));
        //    parameters.Add(new SqlParameter("@MailCounty", E.MailCounty));
        //    parameters.Add(new SqlParameter("@CtryCde", E.CountryCode));
        //    parameters.Add(new SqlParameter("@MailState", E.MailStateID));
        //    parameters.Add(new SqlParameter("@DateofIncorp", E.DateofIncorp));
        //    parameters.Add(new SqlParameter("@ICDiscElection", E.ICDISCElectionDate));
        //    parameters.Add(new SqlParameter("@ID", E.ID));
        //    parameters.Add(new SqlParameter("@CountyofIncorporation", E.CountyofIncorporation));
        //    parameters.Add(new SqlParameter("@StateofIncorporation", E.StateofIncorporationID));
        //    parameters.Add(new SqlParameter("@CtryCdeIncorp", E.CountryofIncorporation));
        //    parameters.Add(new SqlParameter("@InitialTaxYearEnd", E.InitialTaxYearEnd));
        //    parameters.Add(new SqlParameter("@DateOfDispLiqMer", E.DateOfDispLiqMer));
        //    parameters.Add(new SqlParameter("@YrGroupID", E.YrGroupID));
        //    parameters.Add(new SqlParameter("@StatusNotes", E.StatusNotes));

        //    DataSet dsResponse = null;

        //    dsResponse = base.ReturnDataSet(dbFactory, "PEXP_UPDATE_ENTITY", parameters);

        //    if (dsResponse == null || dsResponse.Tables.Count == 0)
        //    {
        //        newID = 0;
        //    }
        //    else
        //    {
        //        //newID = Convert.ToInt32(dsResponse.Tables[0].Rows[0]["ID"]);
        //    }
        //    return newID;

        //}
        //public int UpdateAnnualInformation(EntityAnnualInformation AI)
        //{

        //    var parameters = new List<SqlParameter>();
        //    int newID = 0;

        //    parameters.Add(new SqlParameter("@EntityID", AI.EntityID));
        //    parameters.Add(new SqlParameter("@LegalName", AI.LegalName));
        //    parameters.Add(new SqlParameter("@CaseID", AI.CaseID));
        //    parameters.Add(new SqlParameter("@BAC", AI.BAC));
        //    parameters.Add(new SqlParameter("@FunctionalCurrencyID", AI.FunctionalCurrencyID));
        //    parameters.Add(new SqlParameter("@IsActive", AI.IsActive));
        //    parameters.Add(new SqlParameter("@ID", AI.ID));
        //    parameters.Add(new SqlParameter("@AccrualBasis", AI.AccrualBasis));
        //    parameters.Add(new SqlParameter("@DisplayName", AI.DisplayName));
        //    parameters.Add(new SqlParameter("@Notes", AI.Notes));
        //    parameters.Add(new SqlParameter("@YearID", AI.YearID));
        //    parameters.Add(new SqlParameter("@RSParentID", AI.RSParentID));
        //    //parameters.Add(new SqlParameter("@LastChangeStamp", AI.LastChangeStamp)); 
        //    // parameters.Add(new SqlParameter("@ClientID", AI.ClientID)); 
        //    parameters.Add(new SqlParameter("@ReconciliationFileName", AI.ReconciliationFileName));
        //    parameters.Add(new SqlParameter("@ReconciliationFile", AI.ReconciliationFile));
        //    parameters.Add(new SqlParameter("@StartDate_Custom", AI.StartDate_Custom));
        //    parameters.Add(new SqlParameter("@EndDate_Custom", AI.EndDate_Custom));
        //    parameters.Add(new SqlParameter("@Locked", AI.Locked));
        //    parameters.Add(new SqlParameter("@Domicile", AI.Domicile));
        //    DataSet dsResponse = null;

        //    dsResponse = base.ReturnDataSet(dbFactory, "PEXP_UPDATE_ANNUALINFORMATION", parameters);

        //    if (dsResponse == null || dsResponse.Tables.Count == 0)
        //    {
        //        newID = 0;
        //    }
        //    else
        //    {
        //        //newID = Convert.ToInt32(dsResponse.Tables[0].Rows[0]["ID"]);
        //    }
        //    return newID;

        //}
        //public int UpdateAnnualInformationDISCReturnitems(EntityAnnualInformation_DISCReturnitems IC)
        //{

        //    var parameters = new List<SqlParameter>();
        //    int newID = 0;

        //    parameters.Add(new SqlParameter("@ID", IC.ID));
        //    parameters.Add(new SqlParameter("@YearID", IC.YearID));
        //    parameters.Add(new SqlParameter("@CASEID", IC.CASEID));
        //    parameters.Add(new SqlParameter("@CommissionEstimate", IC.CommissionEstimate));
        //    parameters.Add(new SqlParameter("@CommissionActual", IC.CommissionActual));
        //    parameters.Add(new SqlParameter("@Prod1", IC.Prod1));
        //    parameters.Add(new SqlParameter("@Prod1Sales", IC.Prod1Sales));
        //    parameters.Add(new SqlParameter("@Prod2", IC.Prod2));
        //    parameters.Add(new SqlParameter("@Prod2Sales", IC.Prod2Sales));
        //    parameters.Add(new SqlParameter("@SalesMethod", IC.SalesMethod));
        //    parameters.Add(new SqlParameter("@TIMethod", IC.TIMethod));
        //    parameters.Add(new SqlParameter("@MCMethod", IC.MCMethod));
        //    parameters.Add(new SqlParameter("@482Method", IC.foureightytwoMethod));
        //    parameters.Add(new SqlParameter("@ExportGrossReceipts", IC.ExportGrossReceipts));
        //    parameters.Add(new SqlParameter("@CalcValue", IC.CalcValue));
        //    parameters.Add(new SqlParameter("@CalcDate", IC.CalcDate));
        //    parameters.Add(new SqlParameter("@PYTI", IC.PYTI));
        //    parameters.Add(new SqlParameter("@PYSales", IC.PYSales));
        //    parameters.Add(new SqlParameter("@PYCalc", IC.PYCalc));
        //    parameters.Add(new SqlParameter("@CommissionEstimateDate", IC.CommissionEstimateDate));
        //    parameters.Add(new SqlParameter("@CommissionActualdate", IC.CommissionActualdate));
        //    parameters.Add(new SqlParameter("@CalcNotes", IC.CalcNotes));
        //    parameters.Add(new SqlParameter("@InitialReturn", IC.InitialReturn));
        //    parameters.Add(new SqlParameter("@FinalReturn", IC.FinalReturn));
        //    parameters.Add(new SqlParameter("@NameChange", IC.NameChange));
        //    parameters.Add(new SqlParameter("@AddressChange", IC.AddressChange));
        //    parameters.Add(new SqlParameter("@AmendedReturn", IC.AmendedReturn));
        //    parameters.Add(new SqlParameter("@EvergreenDiv", IC.EvergreenDiv));
        //    parameters.Add(new SqlParameter("@TransProcessing", IC.TransProcessing));
        //    parameters.Add(new SqlParameter("@CompleteEstimate", IC.CommissionEstimate));
        //    parameters.Add(new SqlParameter("@TransCalc", IC.TransCalc));
        //    parameters.Add(new SqlParameter("@PYCOGS", IC.PYCOGS));
        //    parameters.Add(new SqlParameter("@PYTotalSales", IC.PYTotalSales));
        //    parameters.Add(new SqlParameter("@PYTotalCOGS", IC.PYTotalCOGS));
        //    parameters.Add(new SqlParameter("@PYTotalTI", IC.PYTotalTI));
        //    parameters.Add(new SqlParameter("@PYTotalOI", IC.PYTotalOI));
        //    parameters.Add(new SqlParameter("@Notes", IC.Notes));
        //    parameters.Add(new SqlParameter("@QualifiedCOGS", IC.QualifiedCOGS));
        //    parameters.Add(new SqlParameter("@QualifiedTi", IC.QualifiedTi));
        //    parameters.Add(new SqlParameter("@TotalSales", IC.TotalSales));
        //    parameters.Add(new SqlParameter("@TotalCOGS", IC.TotalCOGS));
        //    parameters.Add(new SqlParameter("@TotalTI", IC.TotalTI));
        //    parameters.Add(new SqlParameter("@DISCMgmt", IC.DISCMgmt));
        //    parameters.Add(new SqlParameter("@TotalOI", IC.TotalOI));

        //    DataSet dsResponse = null;

        //    dsResponse = base.ReturnDataSet(dbFactory, "PEXP_UPDATE_ANNUALICDISCMAINTAINANCE", parameters);

        //    if (dsResponse == null || dsResponse.Tables.Count == 0)
        //    {
        //        newID = 0;
        //    }
        //    else
        //    {
        //        //newID = Convert.ToInt32(dsResponse.Tables[0].Rows[0]["ID"]);
        //    }

        //    return newID;

        //}
        //public int UpdateOwnership(EntityOwnership OW)
        //{

        //    var parameters = new List<SqlParameter>();
        //    int newID = 0;
        //    parameters.Add(new SqlParameter("@ParentID", OW.ParentID));
        //    parameters.Add(new SqlParameter("@OwnershipPercent", OW.OwnershipPercent));
        //    parameters.Add(new SqlParameter("@OwnerName", OW.OwnerName));
        //    parameters.Add(new SqlParameter("@OwnerMailAddressL1", OW.OwnerMailAddressL1));
        //    parameters.Add(new SqlParameter("@OwnerMailAddressL2", OW.OwnerMailAddressL2));
        //    parameters.Add(new SqlParameter("@OwnerMailCity", OW.OwnerMailCity));
        //    parameters.Add(new SqlParameter("@OwnerMailZip", OW.OwnerMailZip));
        //    parameters.Add(new SqlParameter("@OwnerTaxID", OW.OwnerTaxID));
        //    parameters.Add(new SqlParameter("@SharesOwned", OW.SharesOwned));
        //    parameters.Add(new SqlParameter("@OwnerTaxYr", OW.OwnerTaxYr));
        //    parameters.Add(new SqlParameter("@OwnerServiceCenter", OW.OwnerServiceCenter));
        //    parameters.Add(new SqlParameter("@CertNum", OW.CertNum));
        //    parameters.Add(new SqlParameter("@ID", OW.ID));
        //    parameters.Add(new SqlParameter("@OwnershipBegin", OW.OwnershipBegin));
        //    parameters.Add(new SqlParameter("@OwnershipEnd", OW.OwnershipEnd));
        //    parameters.Add(new SqlParameter("@Active", OW.Active));
        //    parameters.Add(new SqlParameter("@OwnerCtryCde", OW.OwnerCtryCde));
        //    parameters.Add(new SqlParameter("@OwnerMailState", OW.OwnerMailStateID));
        //    parameters.Add(new SqlParameter("@OwnerType", OW.OwnerTypeID));
        //    parameters.Add(new SqlParameter("@ShareTypeID", OW.ShareTypeID));
        //    parameters.Add(new SqlParameter("@ClientID", OW.ClientID));
        //    parameters.Add(new SqlParameter("@ParentName", OW.ParentName));
        //    parameters.Add(new SqlParameter("@RSID", OW.RSID));
        //    parameters.Add(new SqlParameter("@POCID", OW.POCID));
        //    parameters.Add(new SqlParameter("@ShareTransferRowID", OW.ShareTransferRowID));
        //    parameters.Add(new SqlParameter("@OwnershipNotes", OW.OwnershipNotes));

        //    DataSet dsResponse = null;

        //    dsResponse = base.ReturnDataSet(dbFactory, "PEXP_UPDATE_OWNERSHIP", parameters);

        //    if (dsResponse == null || dsResponse.Tables.Count == 0)
        //    {
        //        newID = 0;
        //    }
        //    else
        //    {
        //        //newID = Convert.ToInt32(dsResponse.Tables[0].Rows[0]["ID"]);
        //    }
        //    return newID;

        //}
        //public int UpdateRelatedSupplier(EntityRelatedSupplier ERS)
        //{

        //    var parameters = new List<SqlParameter>();
        //    int newID = 0;

        //    parameters.Add(new SqlParameter("@RSParentID", ERS.RSParentID));
        //    parameters.Add(new SqlParameter("@RelatedSupplierLegalName", ERS.RelatedSupplierLegalName));
        //    parameters.Add(new SqlParameter("@RSAddressL1", ERS.RSAddressL1));
        //    parameters.Add(new SqlParameter("@Active", ERS.Active));
        //    parameters.Add(new SqlParameter("@RSAddressL2", ERS.RSAddressL2));
        //    // parameters.Add(new SqlParameter("@IsActive", AI.IsActive));
        //    parameters.Add(new SqlParameter("@RSZip", ERS.RSZip));
        //    parameters.Add(new SqlParameter("@RSCtryCde", ERS.RSCtryCde));
        //    parameters.Add(new SqlParameter("@RSNotes", ERS.RSNotes));
        //    parameters.Add(new SqlParameter("@ID", ERS.ID));
        //    parameters.Add(new SqlParameter("@RSCity", ERS.RSCity));
        //    parameters.Add(new SqlParameter("@RSPhone", ERS.RSPhone));
        //    parameters.Add(new SqlParameter("@RSIncorporatedCtryCde", ERS.IncorporatedCountry));
        //    parameters.Add(new SqlParameter("@RSState", ERS.RSState));
        //    parameters.Add(new SqlParameter("@RSIncorporatedState", ERS.RSIncorporatedState));
        //    parameters.Add(new SqlParameter("@RSType", ERS.RSType));
        //    parameters.Add(new SqlParameter("@RSTaxID", ERS.RSTaxID));
        //    parameters.Add(new SqlParameter("@RSBAC", ERS.RSBAC));
        //    parameters.Add(new SqlParameter("@RSStartDate", ERS.RSStartDate));
        //    parameters.Add(new SqlParameter("@RSEndDate", ERS.RSEndDate));
        //    parameters.Add(new SqlParameter("@RSEndNotes", ERS.RSEndNotes));
        //    parameters.Add(new SqlParameter("@YrGrpID", 1));

        //    DataSet dsResponse = null;


        //    dsResponse = base.ReturnDataSet(dbFactory, "PEXP_UPDATE_RELATEDSUPPLIER", parameters);

        //    if (dsResponse == null || dsResponse.Tables.Count == 0)
        //    {
        //        newID = 0;
        //    }
        //    else
        //    {
        //        //newID = Convert.ToInt32(dsResponse.Tables[0].Rows[0]["ID"]);
        //    }
        //    return newID;

        //}
        //public int UpdateEntityPointofContact(EntityPointofContact poc)
        //{

        //    List<EntityPointofContact> DMEditEntity = new List<EntityPointofContact>();
        //    int newid;
        //    var parameters = new List<SqlParameter>();
        //    parameters.Add(new SqlParameter("@ID", poc.ID));
        //    parameters.Add(new SqlParameter("@ParentID", poc.ParentID));
        //    parameters.Add(new SqlParameter("@MailAddressL1", poc.MailAddressL1));
        //    parameters.Add(new SqlParameter("@MailAddressL2", poc.MailAddressL2));
        //    parameters.Add(new SqlParameter("@MailCity", poc.MailCity));
        //    parameters.Add(new SqlParameter("@MailZip", poc.MailZip));
        //    parameters.Add(new SqlParameter("@FName", poc.FName));
        //    parameters.Add(new SqlParameter("@LName", poc.LName));
        //    parameters.Add(new SqlParameter("@Title", poc.Title));
        //    parameters.Add(new SqlParameter("@Email", poc.Email));
        //    parameters.Add(new SqlParameter("@Active", poc.Active));
        //    parameters.Add(new SqlParameter("@PhoneNumber", poc.PhoneNumber));
        //    parameters.Add(new SqlParameter("@POCNotes", poc.POCNotes));
        //    parameters.Add(new SqlParameter("@IRSPoc", poc.IRSPoc));
        //    parameters.Add(new SqlParameter("@IRSDesignee", poc.IRSDesignee));
        //    parameters.Add(new SqlParameter("@SSN", poc.SSN));
        //    parameters.Add(new SqlParameter("@MailState", poc.State));
        //    parameters.Add(new SqlParameter("@CtryCde", poc.Country));
        //    parameters.Add(new SqlParameter("@FaxNumer", poc.FaxNumer));
        //    parameters.Add(new SqlParameter("@MInitial", poc.MInitial));
        //    parameters.Add(new SqlParameter("@Salutation", poc.Salutation));
        //    parameters.Add(new SqlParameter("@PhoneNumberCell", poc.PhoneNumberCell));
        //    parameters.Add(new SqlParameter("@PhoneNumberWork", poc.PhoneNumberWork));
        //    parameters.Add(new SqlParameter("@Primary", poc.Primary));
        //    parameters.Add(new SqlParameter("@CC", poc.CC));
        //    parameters.Add(new SqlParameter("@CPAFirmName", poc.CPAFirmID));

        //    DataSet dsResponse = null;
        //    try
        //    {
        //        dsResponse = base.ReturnDataSet(dbFactory, "PEXP_UPDATE_POINTOFCONTACT", parameters);
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }

        //    if (dsResponse == null || dsResponse.Tables.Count == 0)
        //    {
        //        newid = 0;
        //    }
        //    else
        //    {
        //        newid = Convert.ToInt32(dsResponse.Tables[0].Rows[0]["ID"]);
        //    }

        //    return newid;
        //}
        //public int UpdateEntityPointofContactRoles(EntityPointofContact_Roles POCR)
        //{
        //    var parameters = new List<SqlParameter>();
        //    int newID = 0;
        //    parameters.Add(new SqlParameter("@DisplayName", POCR.DisplayName));
        //    parameters.Add(new SqlParameter("@RoleID", POCR.RoleID));
        //    parameters.Add(new SqlParameter("@InActive", POCR.InActive));
        //    parameters.Add(new SqlParameter("@DateStart", POCR.DateStart));
        //    parameters.Add(new SqlParameter("@DateEnd", POCR.DateEnd));
        //    parameters.Add(new SqlParameter("@Notes", POCR.Notes));
        //    parameters.Add(new SqlParameter("@ID", POCR.ID));
        //    parameters.Add(new SqlParameter("@EntityID", POCR.EntityID));
        //    DataSet dsResponse = null;

        //    dsResponse = base.ReturnDataSet(dbFactory, "PEXP_UPDATE_LEADERSHIP", parameters);

        //    if (dsResponse == null || dsResponse.Tables.Count == 0)
        //    {
        //        newID = 0;
        //    }
        //    else
        //    {
        //        //newID = Convert.ToInt32(dsResponse.Tables[0].Rows[0]["ID"]);
        //    }
        //    return newID;

        //}
        //public int UpdateDocuments(Documents doc)
        //{

        //    Documents DMEditEntity = new Documents();
        //    int newDocumentId = 0;
        //    int newId = 0;
        //    var parameters = new List<SqlParameter>();
        //    if (doc.ID != null)
        //        parameters.Add(new SqlParameter("@ID", doc.ID));
        //    parameters.Add(new SqlParameter("@DocumentGroupID", doc.DocumentGroupID));
        //    parameters.Add(new SqlParameter("@EntityID", doc.EntityID));
        //    parameters.Add(new SqlParameter("@DocumentTypeID", doc.DocumentTypeID));
        //    parameters.Add(new SqlParameter("@Notes", doc.Notes));
        //    parameters.Add(new SqlParameter("@DocumentName", doc.DocumentName));
        //    parameters.Add(new SqlParameter("@EntityProcessManagementID", doc.EntityProcessManagementID));
        //    parameters.Add(new SqlParameter("@YearID", doc.YearID));
        //    parameters.Add(new SqlParameter("@CaseID", doc.CaseID));
        //    parameters.Add(new SqlParameter("@Note", doc.Note));
        //    parameters.Add(new SqlParameter("@InActive", doc.InActive));
        //    parameters.Add(new SqlParameter("@UserID", doc.UserID));
        //    parameters.Add(new SqlParameter("@DateDue", doc.DateDue));
        //    parameters.Add(new SqlParameter("@DateClosed", doc.DateClosed));
        //    parameters.Add(new SqlParameter("@NoteSummary", doc.NoteSummary));
        //    parameters.Add(new SqlParameter("@UserIDAssignedto", doc.UserIDAssignedto));
        //    parameters.Add(new SqlParameter("@ClientID", doc.ClientID));

        //    DataSet dsResponse = null;
        //    try
        //    {
        //        dsResponse = base.ReturnDataSet(dbFactory, "PEXP_UPDATE_DOCUMENTS", parameters);
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }

        //    if (dsResponse == null || dsResponse.Tables.Count == 0)
        //    {
        //        newDocumentId = 0;
        //    }
        //    else
        //    {
        //        newDocumentId = Convert.ToInt32(dsResponse.Tables[0].Rows[0]["ID"]);
        //    }

        //    foreach (DocumentFiles file in doc.Files)
        //    {
        //        var parameters1 = new List<SqlParameter>();
        //        parameters1.Add(new SqlParameter("@ID", file.ID));
        //        parameters1.Add(new SqlParameter("@ActualDocument", file.ActualDocument));
        //        parameters1.Add(new SqlParameter("@Filename", file.FileName));
        //        if (doc.ID != null)
        //            parameters1.Add(new SqlParameter("@DocumentID", doc.ID));
        //        else
        //            parameters1.Add(new SqlParameter("@DocumentID", newDocumentId));
        //        parameters1.Add(new SqlParameter("@Notes", file.Notes));
        //        DataSet dsResponse1 = null;
        //        try
        //        {
        //            dsResponse = base.ReturnDataSet(dbFactory, "PEXP_UPDATE_DOCUMENTFILES", parameters1);

        //        }
        //        catch (Exception)
        //        {

        //            throw;
        //        }

        //        if (dsResponse1 == null || dsResponse1.Tables.Count == 0)
        //        {
        //            newId = 0;
        //        }
        //        else
        //        {
        //            newId = (int)dsResponse1.Tables[0].Rows[0]["ID"];
        //        }
        //    }

        //    return newDocumentId;
        //}

        //public void DeleteDocument(int ID)
        //{
        //    var parameters = new List<SqlParameter>();
        //    parameters.Add(new SqlParameter("@ID", ID));
        //    base.ExecuteStoredProcedure(dbFactory, "PEXP_DELETE_DOCUMENTSFILES", parameters);
        //}

    }
}


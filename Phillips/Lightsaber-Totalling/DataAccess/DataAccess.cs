using Apttus.Lightsaber.Extensibility.Framework.Library.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Apttus.Lightsaber.Phillips.Totalling
{
    public class DataAccess
    {
        private readonly IDBHelper dbHelper;

        public DataAccess(IDBHelper dbHelper)
        {
            this.dbHelper = dbHelper;
        }

        public async Task<List<PriceListItemQueryModel>> GetPLI(HashSet<string> priceListItemIdSet)
        {
            var pliQuery = QueryHelper.GetPLIQuery(priceListItemIdSet);
            List<PriceListItemQueryModel> pliDetails = await dbHelper.FindAsync<PriceListItemQueryModel>(pliQuery);
            return pliDetails;
        }

        public async Task<List<ThresholdApproverQueryModel>> GetThresholdApprovers(string salesOrganization)
        {
            var thresholdApproversQuery = QueryHelper.GetThresholdApproversQuery(salesOrganization);
            List<ThresholdApproverQueryModel> thresoldApprovers = await dbHelper.FindAsync<ThresholdApproverQueryModel>(thresholdApproversQuery);
            return thresoldApprovers;
        }

        public async Task<List<ProcurementApprovalQueryModel>> GetProcurementApproval(string salesOrganization)
        {
            var procurementApprovalQuery = QueryHelper.GetProcurementApprovalQuery(salesOrganization);
            List<ProcurementApprovalQueryModel> procurementApprovals = await dbHelper.FindAsync<ProcurementApprovalQueryModel>(procurementApprovalQuery);
            return procurementApprovals;
        }
    }
}
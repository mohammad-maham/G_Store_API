using GoldStore.Models;

namespace GoldStore.BusinessLogics.IBusinessLogics
{
    public interface IReports
    {
        List<GoldRepositoryReportFilterDataVM> GoldRepositoryReport(GoldRepositoryReportFilterVM reportFilterVM, string token);
        string ConvertToPersianDate(DateTime date);
        string GetUserName(string? userAdditionalData);
        string GetUserRole(string? userAdditionalData);
        string GetGoldType(short goldType);
        string GetGoldMaintenanceType(short goldMaintenanceType);
        string GetTransactionType(int transactionType);
        string GetArchiveOperationsType(string? operationType);
    }
}

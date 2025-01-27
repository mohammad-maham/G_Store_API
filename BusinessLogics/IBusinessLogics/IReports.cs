using GoldStore.Models;

namespace GoldStore.BusinessLogics.IBusinessLogics
{
    public interface IReports
    {
        List<RepositoryReportFilterDataVM> RepositoryReport(RepositoryReportFilterVM reportFilterVM, string token);
        string ConvertToPersianDate(DateTime date);
        string GetUserName(string? userAdditionalData);
        string GetUserRole(string? userAdditionalData);
        string GetEntityType(int entityId);
        string GetMaintenanceType(int maintenanceTypeId);
        string GetTransactionType(int transactionType);
        string GetArchiveOperationsType(string? operationType);
    }
}

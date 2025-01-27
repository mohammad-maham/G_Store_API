using GoldStore.BusinessLogics.IBusinessLogics;
using GoldStore.Models;
using System.Globalization;

namespace GoldStore.BusinessLogics
{
    public class Reports : IReports
    {
        private readonly GStoreDbContext _store;
        private readonly IAccounting _accounting;
        private readonly ILogger<Reports>? _logger;

        public Reports()
        {
            _store = new GStoreDbContext();
            _accounting = new Accounting();
        }

        public Reports(ILogger<Reports>? logger, GStoreDbContext store, IAccounting accounting)
        {
            _logger = logger;
            _store = store;
            _accounting = accounting;
        }

        public string ConvertToPersianDate(DateTime date)
        {
            string persianDateString = date.ToString("yyyy/MM/dd HH:mm:ss", new CultureInfo("fa-IR"));
            return persianDateString;
        }

        public string GetArchiveOperationsType(string? operationType)
        {
            string result = "تعریف سرمایه";
            if (!string.IsNullOrEmpty(operationType))
            {
                switch (operationType)
                {
                    case "UPDATE":
                        result = "ویرایش سرمایه";
                        break;
                    case "DELETE":
                        result = "حذف سرمایه";
                        break;
                    default:
                        break;
                }
            }
            return result;
        }

        public string GetMaintenanceType(int maintenanceTypeId) => _store.MaintenanceTypes.FirstOrDefault(x => x.Id == maintenanceTypeId)?.Title ?? "";

        public string GetEntityType(int entityId) => _store.Entities.FirstOrDefault(x => x.Id == entityId)?.Caption ?? "";

        public string GetTransactionType(int transactionType)
        {
            string result = string.Empty;
            switch (transactionType)
            {
                case 1:
                    result = "تبدیل طلا به پول";
                    break;
                case 2:
                    result = "تبدیل پول به طلا";
                    break;
                case 3:
                    result = "افزایش سرمایه";
                    break;
                case 4:
                    result = "کاهش سرمایه";
                    break;
                default:
                    break;
            }
            return result;
        }

        public string GetUserName(string? userAdditionalData)
        {
            string userName = string.Empty;
            UserInfoVM userInfo = new UserInfoVM();
            if (!string.IsNullOrEmpty(userAdditionalData))
            {
                userInfo = _accounting.ParseUserInfo(userAdditionalData);
                if (userInfo != null && userInfo.UserId != 0)
                {
                    userName = $"{userInfo.FirstName} {userInfo.LastName}";
                }
            }
            return userName;
        }

        public string GetUserRole(string? userAdditionalData)
        {
            string role = string.Empty;
            UserInfoVM userInfo = new UserInfoVM();
            if (!string.IsNullOrEmpty(userAdditionalData))
            {
                userInfo = _accounting.ParseUserInfo(userAdditionalData);
                if (userInfo != null && userInfo.UserId != 0)
                {
                    role = userInfo.UserRole ?? "";
                }
            }
            return role;
        }

        public List<RepositoryReportFilterDataVM> RepositoryReport(RepositoryReportFilterVM reportFilterVM, string token)
        {
            List<RepositoryReportFilterDataVM> reportFilterData = new List<RepositoryReportFilterDataVM>();

            IEnumerable<RepositoryReportFilterDataVM>? data = _store.Repositories
                .SelectMany(gr => _store.RepositoryTransactions.Where(x => x.Id == gr.TransactionId), (gr, grt) => new { gr, grt })
                .ToList()
                .Select(x => new RepositoryReportFilterDataVM()
                {
                    TransactionId = x.grt.Id,
                    RegDate = x.gr.RegDate,
                    EntityId = x.gr.Entity,
                    Entity = GetEntityType(x.gr.Entity),
                    MaintenanceTypeId = x.gr.MaintenanceTypeId,
                    MaintenanceType = GetMaintenanceType(x.gr.MaintenanceTypeId),
                    RegUserId = x.gr.RegUserId,
                    LastValue = x.grt.LastValue,
                    NewValue = x.grt.NewValue,
                    TransactionTypeId = x.grt.TransactionType,
                    TransactionType = GetTransactionType(x.grt.TransactionType),
                    Weight = x.grt.Value,
                    RegPersianDate = ConvertToPersianDate(x.gr.RegDate),
                    UserName = GetUserName(x.grt.UserAdditionalData),
                    Role = GetUserRole(x.grt.UserAdditionalData),
                    ArchiveOperation = GetArchiveOperationsType(""),
                });

            IEnumerable<RepositoryReportFilterDataVM>? archiveData = _store.ArchiveRepositories
                .SelectMany(agr => _store.RepositoryTransactions.Where(x => x.Id == agr.TransactionId), (agr, grt) => new { agr, grt })
                .ToList()
                .Select(x => new RepositoryReportFilterDataVM()
                {
                    TransactionId = x.grt.Id,
                    RegDate = x.agr.RegDate,
                    EntityId = x.agr.EntityType,
                    Entity = GetEntityType(x.agr.EntityType),
                    MaintenanceTypeId = x.agr.MaintenanceType,
                    MaintenanceType = GetMaintenanceType(x.agr.MaintenanceType),
                    RegUserId = x.agr.RegUserId,
                    LastValue = x.grt.LastValue,
                    NewValue = x.grt.NewValue,
                    TransactionTypeId = x.grt.TransactionType,
                    TransactionType = GetTransactionType(x.grt.TransactionType),
                    Weight = x.grt.Value,
                    RegPersianDate = ConvertToPersianDate(x.agr.RegDate),
                    UserName = GetUserName(x.grt.UserAdditionalData),
                    Role = GetUserRole(x.grt.UserAdditionalData),
                    ArchiveOperation = GetArchiveOperationsType(x.agr.ArchiveOperation),
                });

            if (data != null && data.Count() > 0 && archiveData != null && archiveData.Count() > 0)
            {
                data = archiveData.AsQueryable().Union(data.AsQueryable()).ToList();
            }
            else if (data != null && data.Count() > 0 && (archiveData == null || archiveData.Count() == 0))
            {
                data = data.AsQueryable().ToList();
            }
            else if ((data == null || data.Count() == 0) && archiveData != null && archiveData.Count() > 0)
            {
                data = archiveData.AsQueryable().ToList();
            }

            if (data != null)
            {
                //if (reportFilterVM.Carat != null && reportFilterVM.Carat != 0)
                //{
                //    data = data.Where(x => x.Carat == reportFilterVM.Carat).ToList();
                //}
                if (reportFilterVM.UserId != null && reportFilterVM.UserId != 0)
                {
                    data = data.Where(x => x.RegUserId == reportFilterVM.UserId).ToList();
                }
                if (reportFilterVM.EntityType != null && reportFilterVM.EntityType != 0)
                {
                    data = data.Where(x => x.EntityId == (int)reportFilterVM.EntityType.Value).ToList();
                }
                if (reportFilterVM.MaintenanceType != null && reportFilterVM.MaintenanceType != 0)
                {
                    data = data.Where(x => x.MaintenanceTypeId == reportFilterVM.MaintenanceType).ToList();
                }
                if (reportFilterVM.FromDate != null)
                {
                    data = data.Where(x => x.RegDate >= reportFilterVM.FromDate).ToList();
                }
                if (reportFilterVM.ToDate != null)
                {
                    data = data.Where(x => x.RegDate <= reportFilterVM.ToDate).ToList();
                }

                if (data != null && data.Count() > 0)
                {
                    reportFilterData = data.ToList();
                }
            }

            return reportFilterData;
        }
    }
}

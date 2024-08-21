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

        public string GetGoldMaintenanceType(short goldMaintenanceType)
        {
            string result = string.Empty;
            switch (goldMaintenanceType)
            {
                case 10:
                    result = "مالکیتی";
                    break;
                case 11:
                    result = "امانتی";
                    break;
                default:
                    break;
            }
            return result;
        }

        public string GetGoldType(short goldType)
        {
            string result = string.Empty;
            switch (goldType)
            {
                case 1:
                    result = "طلا آب شده";
                    break;
                case 2:
                    result = "طلا آب شده-حواله شده";
                    break;
                default:
                    break;
            }
            return result;
        }

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

        public List<GoldRepositoryReportFilterDataVM> GoldRepositoryReport(GoldRepositoryReportFilterVM reportFilterVM, string token)
        {
            List<GoldRepositoryReportFilterDataVM> reportFilterData = new List<GoldRepositoryReportFilterDataVM>();

            IEnumerable<GoldRepositoryReportFilterDataVM>? data = _store.GoldRepositories
                .SelectMany(gr => _store.GoldRepositoryTransactions.Where(x => x.Id == gr.TransactionId), (gr, grt) => new { gr, grt })
                .ToList()
                .Select(x => new GoldRepositoryReportFilterDataVM()
                {
                    TransactionId = x.grt.Id,
                    RegDate = x.gr.RegDate,
                    GoldTypeId = x.gr.GoldType,
                    GoldType = GetGoldType(x.gr.GoldType),
                    GoldMaintenanceTypeId = x.gr.GoldMaintenanceType,
                    GoldMaintenanceType = GetGoldMaintenanceType(x.gr.GoldMaintenanceType),
                    Carat = x.gr.Carat,
                    RegUserId = x.gr.RegUserId,
                    LastGoldValue = x.grt.LastGoldValue,
                    NewGoldValue = x.grt.NewGoldValue,
                    TransactionTypeId = x.grt.TransactionType,
                    TransactionType = GetTransactionType(x.grt.TransactionType),
                    Weight = x.grt.Weight,
                    RegPersianDate = ConvertToPersianDate(x.gr.RegDate),
                    UserName = GetUserName(x.grt.UserAdditionalData),
                    Role = GetUserRole(x.grt.UserAdditionalData),
                    ArchiveOperation = GetArchiveOperationsType(""),
                });

            IEnumerable<GoldRepositoryReportFilterDataVM>? archiveData = _store.ArchiveGoldRepositories
                .SelectMany(agr => _store.GoldRepositoryTransactions.Where(x => x.Id == agr.TransactionId), (agr, grt) => new { agr, grt })
                .ToList()
                .Select(x => new GoldRepositoryReportFilterDataVM()
                {
                    TransactionId = x.grt.Id,
                    RegDate = x.agr.RegDate,
                    GoldTypeId = x.agr.GoldType,
                    GoldType = GetGoldType(x.agr.GoldType),
                    GoldMaintenanceTypeId = x.agr.GoldMaintenanceType,
                    GoldMaintenanceType = GetGoldMaintenanceType(x.agr.GoldMaintenanceType),
                    Carat = x.agr.Carat,
                    RegUserId = x.agr.RegUserId,
                    LastGoldValue = x.grt.LastGoldValue,
                    NewGoldValue = x.grt.NewGoldValue,
                    TransactionTypeId = x.grt.TransactionType,
                    TransactionType = GetTransactionType(x.grt.TransactionType),
                    Weight = x.grt.Weight,
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
                if (reportFilterVM.Carat != null && reportFilterVM.Carat != 0)
                {
                    data = data.Where(x => x.Carat == reportFilterVM.Carat).ToList();
                }
                if (reportFilterVM.UserId != null && reportFilterVM.UserId != 0)
                {
                    data = data.Where(x => x.RegUserId == reportFilterVM.UserId).ToList();
                }
                if (reportFilterVM.GoldType != null && reportFilterVM.GoldType != 0)
                {
                    data = data.Where(x => x.GoldTypeId == reportFilterVM.GoldType).ToList();
                }
                if (reportFilterVM.MaintenanceType != null && reportFilterVM.MaintenanceType != 0)
                {
                    data = data.Where(x => x.GoldMaintenanceTypeId == reportFilterVM.MaintenanceType).ToList();
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

using PetsAPI.Models;

namespace PetsAPI.Services
{
    public interface IAlertService
    {
        Task<List<Alert>> CheckCollarDataAlertsAsync(CollarData data);
        Task<List<Alert>> CheckDailyLogAlertsAsync(DailyLog log, decimal avgFood);
    }
}

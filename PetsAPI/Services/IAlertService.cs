using PetsAPI.Models;

namespace PetsAPI.Services
{
    public interface IAlertService
    {
        Task<List<Alert>> CheckHealthDataAlertsAsync(HealthData data);
        Task<List<Alert>> CheckDailyLogAlertsAsync(DailyLog log, decimal avgFood);
    }
}

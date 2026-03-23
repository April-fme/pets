using Microsoft.EntityFrameworkCore;
using PetsAPI.Data;
using PetsAPI.Models;

namespace PetsAPI.Services
{
    public class AlertService : IAlertService
    {
        private readonly PetsDbContext _context;
        private readonly ILogger<AlertService> _logger;

        public AlertService(PetsDbContext context, ILogger<AlertService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<Alert>> CheckHealthDataAlertsAsync(HealthData data)
        {
            var alerts = new List<Alert>();

            // 體溫異常檢查
            if (data.Temperature > 39.5m)
            {
                var alert = new Alert
                {
                    PetID = data.PetID,
                    Type = "體溫異常",
                    Severity = "高",
                    Message = $"體溫過高: {data.Temperature}°C (正常值 < 39.5°C)"
                };
                alerts.Add(alert);
                _logger.LogWarning($"寵物 {data.PetID} 體溫異常: {data.Temperature}°C");
            }

            // 心率異常檢查 (靜止狀態)
            if (data.HeartRate > 120 && data.ActivityLevel < 30)
            {
                var alert = new Alert
                {
                    PetID = data.PetID,
                    Type = "心率異常",
                    Severity = "中",
                    Message = $"靜止狀態下心率過高: {data.HeartRate} bpm"
                };
                alerts.Add(alert);
                _logger.LogWarning($"寵物 {data.PetID} 心率異常: {data.HeartRate} bpm");
            }

            // 睡眠品質下降檢查 (連續三日下降 30%)
            var recentSleep = await _context.HealthData
                .Where(c => c.PetID == data.PetID)
                .OrderByDescending(c => c.Timestamp)
                .Take(3)
                .Select(c => c.SleepQuality)
                .ToListAsync();

            if (recentSleep.Count == 3)
            {
                var avgSleep = recentSleep.Average();
                if (data.SleepQuality < avgSleep * 0.7)
                {
                    var alert = new Alert
                    {
                        PetID = data.PetID,
                        Type = "睡眠品質下降",
                        Severity = "中",
                        Message = $"睡眠品質持續下降: {data.SleepQuality} (平均: {avgSleep:F2})"
                    };
                    alerts.Add(alert);
                    _logger.LogWarning($"寵物 {data.PetID} 睡眠品質下降");
                }
            }

            // 儲存警報到資料庫
            if (alerts.Any())
            {
                await _context.Alerts.AddRangeAsync(alerts);
                await _context.SaveChangesAsync();
            }

            return alerts;
        }

        public async Task<List<Alert>> CheckDailyLogAlertsAsync(DailyLog log, decimal avgFood)
        {
            var alerts = new List<Alert>();

            // 食慾不振檢查
            if (avgFood > 0 && log.FoodAmount < avgFood * 0.4m)
            {
                var alert = new Alert
                {
                    PetID = log.PetID,
                    Type = "食慾不振",
                    Severity = "高",
                    Message = $"食量低於平均值 40%: {log.FoodAmount} (平均: {avgFood:F2})"
                };
                alerts.Add(alert);
                _logger.LogWarning($"寵物 {log.PetID} 食慾不振");
            }

            // 消化系統異常檢查
            if (log.StoolStatus == "血便" || log.StoolStatus == "腹瀉")
            {
                var alert = new Alert
                {
                    PetID = log.PetID,
                    Type = "消化異狀",
                    Severity = "高",
                    Message = $"排便異常: {log.StoolStatus}"
                };
                alerts.Add(alert);
                _logger.LogWarning($"寵物 {log.PetID} 消化異狀: {log.StoolStatus}");
            }

            // 儲存警報到資料庫
            if (alerts.Any())
            {
                await _context.Alerts.AddRangeAsync(alerts);
                await _context.SaveChangesAsync();
            }

            return alerts;
        }
    }
}

using Microsoft.EntityFrameworkCore;
using PetsAPI.Data;
using PetsAPI.Models;

namespace PetsAPI.Services
{
    public class CollarSimulatorService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<CollarSimulatorService> _logger;

        public CollarSimulatorService(IServiceProvider serviceProvider, ILogger<CollarSimulatorService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("項圈模擬器服務已啟動");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await SimulateHealthDataAsync();
                    await Task.Delay(TimeSpan.FromMinutes(15), stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "項圈模擬器執行失敗");
                }
            }
        }

        private async Task SimulateHealthDataAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<PetsDbContext>();
            var alertService = scope.ServiceProvider.GetRequiredService<IAlertService>();

            var pets = await context.Pets.ToListAsync();

            foreach (var pet in pets)
            {
                // 根據寵物ID生成不同的數據模式
                var (tempMin, tempMax, hrMin, hrMax, actMin, actMax, sleepMin, sleepMax) = 
                    GetPetDataRange(pet.ID, pet.Species);

                var healthData = new HealthData
                {
                    PetID = pet.ID,
                    Temperature = GetRandomDecimal(tempMin, tempMax, 1),
                    HeartRate = Random.Shared.Next(hrMin, hrMax),
                    ActivityLevel = Random.Shared.Next(actMin, actMax),
                    SleepQuality = Random.Shared.Next(sleepMin, sleepMax),
                    Timestamp = DateTime.UtcNow
                };

                context.HealthData.Add(healthData);
                await context.SaveChangesAsync();

                // 檢查異常
                await alertService.CheckHealthDataAlertsAsync(healthData);

                _logger.LogInformation($"模擬健康數據: 寵物 {pet.Name} - 體溫 {healthData.Temperature}°C, 心率 {healthData.HeartRate} bpm, 活動 {healthData.ActivityLevel}%, 睡眠 {healthData.SleepQuality}%");
            }
        }

        private (decimal, decimal, int, int, int, int, int, int) GetPetDataRange(int petId, string species)
        {
            // 根據寵物ID和物種設定不同的數據範圍
            return petId switch
            {
                1 => // 第一隻寵物 - 活躍型（如狗）
                    (
                        tempMin: 38.0m, tempMax: 39.5m,           // 溫度範圍
                        hrMin: 70, hrMax: 110,                      // 心率範圍
                        actMin: 50, actMax: 100,                    // 活動水平
                        sleepMin: 40, sleepMax: 80                  // 睡眠質量
                    ),
                2 => // 第二隻寵物 - 懶惰型（如貓）
                    (
                        tempMin: 37.5m, tempMax: 39.0m,            // 溫度範圍
                        hrMin: 100, hrMax: 140,                     // 心率範圍（貓心率較快）
                        actMin: 10, actMax: 60,                     // 活動水平
                        sleepMin: 60, sleepMax: 95                  // 睡眠質量
                    ),
                _ => // 其他寵物 - 默認範圍
                    (
                        tempMin: 37.5m, tempMax: 40.5m,
                        hrMin: 80, hrMax: 130,
                        actMin: 0, actMax: 100,
                        sleepMin: 50, sleepMax: 100
                    )
            };
        }

        private decimal GetRandomDecimal(decimal min, decimal max, int decimals)
        {
            var value = (decimal)Random.Shared.NextDouble() * (max - min) + min;
            return Math.Round(value, decimals);
        }
    }
}

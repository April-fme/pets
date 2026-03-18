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
                    await SimulateCollarDataAsync();
                    await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "項圈模擬器執行失敗");
                }
            }
        }

        private async Task SimulateCollarDataAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<PetsDbContext>();
            var alertService = scope.ServiceProvider.GetRequiredService<IAlertService>();

            var pets = await context.Pets.ToListAsync();

            foreach (var pet in pets)
            {
                var collarData = new CollarData
                {
                    PetID = pet.ID,
                    Temperature = GetRandomDecimal(37.5m, 40.5m, 1),
                    HeartRate = Random.Shared.Next(80, 140),
                    ActivityLevel = Random.Shared.Next(0, 100),
                    SleepQuality = GetRandomDecimal(50m, 100m, 2),
                    Timestamp = DateTime.Now
                };

                context.CollarData.Add(collarData);
                await context.SaveChangesAsync();

                // 檢查異常
                await alertService.CheckCollarDataAlertsAsync(collarData);

                _logger.LogInformation($"模擬項圈數據: 寵物 {pet.Name} - 體溫 {collarData.Temperature}°C, 心率 {collarData.HeartRate} bpm");
            }
        }

        private decimal GetRandomDecimal(decimal min, decimal max, int decimals)
        {
            var value = (decimal)Random.Shared.NextDouble() * (max - min) + min;
            return Math.Round(value, decimals);
        }
    }
}

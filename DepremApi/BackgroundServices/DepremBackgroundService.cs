using DepremApi.Services;

namespace DepremApi.BackgroundServices;

public class DepremBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public DepremBackgroundService(
        IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();

                var depremService =
                    scope.ServiceProvider
                        .GetRequiredService<DepremService>();

                // DB'deki en son deprem tarihini bul
                var sonDepremTarihi =
                    await depremService.SonDepremTarihiniGetirAsync(
                        stoppingToken);

                // DB boşsa 1 Haziran'dan başla
                var startTime = sonDepremTarihi
                    ?? new DateTime(2026, 6, 1, 0, 0, 0);

                // Sorgunun bitiş zamanı
                var endTime = DateTime.Now;

                // Son kayıt ile şimdi arasında veri varsa AFAD'a git
                if (startTime < endTime)
                {
                    await depremService.DepremleriAfaddanGetirVeKaydetAsync(
                        startTime,
                        endTime,
                        stoppingToken);

                    Console.WriteLine(
                        $"Deprem verileri güncellendi: {DateTime.Now}");
                }
                else
                {
                    Console.WriteLine(
                        "Yeni sorgulanacak zaman aralığı yok.");
                }
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"Deprem verileri güncellenirken hata oluştu: {ex.Message}");
            }

            // 5 dakika bekle
            await Task.Delay(
                TimeSpan.FromMinutes(5),
                stoppingToken);
        }
    }
}
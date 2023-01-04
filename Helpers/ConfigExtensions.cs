using BankTransferTask.Core.Models;
using BankTransferTask.Core.Services.Paystack;


namespace BankTransferTask.Helpers;
/// <summary>
/// Contains Extension Method For  Class Configure Services
/// </summary>
public static class ConfigExtensions
    {
    /// <summary>
    /// Configures strong type for Paystack http access
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    public static void ConfigurePaystackHttpClient(this IServiceCollection services, IConfiguration configuration)
    {
        var paystackConfig = configuration.GetSection(nameof(PaystackConfig)).Get<PaystackConfig>();
        var paystackKey = configuration.GetSection("PaystackPrivateKey");
        services.AddHttpClient(StringConstants.PaystackHttpClient, client =>
        {
            client.BaseAddress = new Uri(paystackConfig.BaseUrl);
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", paystackKey.Value);
        });
    }
}


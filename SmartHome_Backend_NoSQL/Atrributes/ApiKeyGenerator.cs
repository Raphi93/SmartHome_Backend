using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace SmartHome_Backend_NoSQL.Atrributes
{
    public static class ApiKeyGenerator
    {
        private const string ApiKeyConfigKey = "ApiKey";
        private const string ApiKeySettingsConfigKey = "ApiKeySettings:GenerateWeekly";

        public static void GenerateNewApiKeyIfNeeded()
        {
            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, true);

            var configuration = configurationBuilder.Build();

            var generateWeekly = configuration.GetValue<bool>("ApiKeySettings:GenerateWeekly");
            var lastGeneration = configuration.GetValue<DateTime?>("ApiKeySettings:LastGeneration");

            if (!generateWeekly || (lastGeneration.HasValue && lastGeneration.Value.DayOfWeek == DayOfWeek.Sunday && DateTime.UtcNow.Subtract(lastGeneration.Value).TotalDays < 7))
            {
                return;
            }

            var newApiKey = Guid.NewGuid().ToString();

            var configFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
            var json = File.ReadAllText(configFile);
            dynamic jsonObj = JObject.Parse(json);

            jsonObj["ApiKey"] = newApiKey;
            jsonObj["ApiKeySettings"]["LastGeneration"] = DateTime.UtcNow;
            jsonObj["ApiKeySettings"]["GenerateWeekly"] = true;

            string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(configFile, output);
        }
    }
}

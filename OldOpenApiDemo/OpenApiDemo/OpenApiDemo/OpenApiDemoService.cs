using Microsoft.AspNetCore.SignalR.Client;
using OpenApiDemo.Common;
using OpenApiDemo.Models;
using SCADA.Modules.OpenApi.Models;
using System.Net.Http.Json;
using System.Reflection.Emit;
using System.Text;
using System.Text.Json;

namespace OpenApiDemo
{
    public class OpenApiDemoService(
        ILogger<OpenApiDemoService> logger,
        IHttpClientFactory httpClientFactory,
        IHubConnectionFactory hubConnectionFactory)
        : BackgroundService
    {

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //await Test(GetRedundancyStatusStreamAsync, stoppingToken);
            //await Test(GetLicenseStatusStreamAsync, stoppingToken);
            //await Test(GetAssetsAsync, stoppingToken);
            await Test(GetTagValuesStreamAsync, stoppingToken);
            //await Test(GetTagPropertiesStreamAsync, stoppingToken);
            //await Test(GetAlarmsStreamAsync, stoppingToken);
            //await Test(GetShelvedAlarmsStreamAsync, stoppingToken);
        }

        private async Task PrepareTestDataAsync(CancellationToken cancellationToken)
        {
            var httpClient = httpClientFactory.CreateClient(HttpClientNames.OpenApi);
            var tags = await httpClient.GetFromJsonAsync<IEnumerable<AssetTagModel>>("api/v1/tags");


        }



        private async Task Test(Func<CancellationToken, Task> action, CancellationToken cancellationToken)
        {
            try

            {
                logger.LogInformation($"Run {action.Method.Name} begin");
                await action(cancellationToken);
                logger.LogInformation($"Run {action.Method.Name} end");
            }
            catch (Exception ex)
            {
                logger.LogError($"Run {action.Method.Name} failed with error {ex.Message}");
            }

        }

        private async Task GetAssetsAsync(CancellationToken cancellationToken)
        {
            var httpClient = httpClientFactory.CreateClient(HttpClientNames.OpenApi);

            var assets = await httpClient.GetFromJsonAsync<IEnumerable<AssetTreeModel>>("api/v1/assets", cancellationToken);
            logger.LogInformation($"Retrieved {assets!.Count()} assets from server");

        }

        private async Task GetLicenseStatusStreamAsync(CancellationToken cancellationToken)
        {
            var hubConnection = hubConnectionFactory.Create("ws/v1/node");
            await hubConnection.StartAsync(cancellationToken);

            using CancellationTokenSource cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cancellationToken = cancellationTokenSource.Token;
            //cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(10));

            var stream = hubConnection.StreamAsync<LicenseStatusRecord>("License", cancellationToken);
            await foreach (var record in stream)
            {
                var data = Encoding.UTF8.GetBytes(record.ToString());

                logger.LogInformation($"Received tag values from server:{JsonSerializer.Serialize(record)}");

                DateTime timestamp = DateTime.Parse(record.Timestamp);



                if (CertUtil.Verify(data, record.Signature))
                {
                    logger.LogInformation($"The signature is valid");
                }
                else
                {
                    logger.LogInformation($"The signature is invalid");
                }

                if (DateTime.UtcNow - timestamp <= TimeSpan.FromMinutes(10))
                {
                    logger.LogInformation($"The timestamp is valid");
                }
                else
                {
                    logger.LogInformation($"The timestamp is invalid");
                }

            }
        }

        private async Task GetRedundancyStatusStreamAsync(CancellationToken cancellationToken)
        {
            var hubConnection = hubConnectionFactory.Create("ws/v1/node");
            await hubConnection.StartAsync(cancellationToken);

            using CancellationTokenSource cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cancellationToken = cancellationTokenSource.Token;


            var stream = hubConnection.StreamAsync<RedundancyStatusRecord>("RedundancyStatus", cancellationToken);
            await foreach (var record in stream)
            {
                

                logger.LogInformation($"Received tag values from server:{JsonSerializer.Serialize(record)}");

                

            }
        }


        private async Task GetTagValuesStreamAsync(CancellationToken cancellationToken)
        {

            var hubConnection = hubConnectionFactory.Create("ws/v1/realtimeData");
            await hubConnection.StartAsync(cancellationToken);
            List<string> tags = ["Default:m1"];
            using CancellationTokenSource cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cancellationToken = cancellationTokenSource.Token;
            cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(10));
            var stream = hubConnection.StreamAsync<IEnumerable<TagValueModel>>("TagValues", tags, cancellationToken);
            await foreach (var tagValues in stream)
            {
                logger.LogInformation($"Received tag values from server:{JsonSerializer.Serialize(tagValues)}");
                break;
            }
        }

        private async Task GetTagPropertiesStreamAsync(CancellationToken cancellationToken)
        {

            var hubConnection = hubConnectionFactory.Create("ws/v1/realtimeData");
            await hubConnection.StartAsync(cancellationToken);
            List<string> tags = ["Default:m1#Value", "Default:m1#Unit", "Default:m1#Simulation", "Default:m1#DisplayValue", "Default:m1#Timestamp", "Default:m1#Quality", "Default:m1#Alarm"];
            using CancellationTokenSource cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cancellationToken = cancellationTokenSource.Token;
            //cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(10));
            var stream = hubConnection.StreamAsync<IEnumerable<TagPropertyModel>>("TagProperties", tags, cancellationToken);
            int count = 0;
            await foreach (var tagProperties in stream)
            {
                logger.LogInformation($"Received tag properties from server:{JsonSerializer.Serialize(tagProperties)}");
                if (count++ >= 10)
                {
                    break;
                }
            }
        }

        private async Task GetAlarmsStreamAsync(CancellationToken cancellationToken)
        {
            var hubConnection = hubConnectionFactory.Create("ws/v1/realtimeData");
            await hubConnection.StartAsync(cancellationToken);

            using CancellationTokenSource cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cancellationToken = cancellationTokenSource.Token;
            //cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(10));
            var stream = hubConnection.StreamAsync<IEnumerable<AlarmModel>>("Alarms", cancellationToken);
            await foreach (var alarms in stream)
            {
                logger.LogInformation($"Received tag alarms from server:{JsonSerializer.Serialize(alarms)}");

            }
        }


        private async Task GetShelvedAlarmsStreamAsync(CancellationToken cancellationToken)
        {
            var hubConnection = hubConnectionFactory.Create("ws/v1/realtimeData");
            await hubConnection.StartAsync(cancellationToken);

            using CancellationTokenSource cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cancellationToken = cancellationTokenSource.Token;
            //cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(10));
            var stream = hubConnection.StreamAsync<IEnumerable<ShelvedAlarmModel>>("ShelvedAlarms", cancellationToken);
            await foreach (var alarms in stream)
            {
                logger.LogInformation($"Received shelved alarms from server:{JsonSerializer.Serialize(alarms)}");
                break;
            }
        }











    }
}

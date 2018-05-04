using CommandLine;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace LatencyClient
{
    class Options
    {
        [Option('u', "url", Required = true)]
        public string Url { get; set; }

        [Option('r', "rps", Default = 10)]
        public int RequestsPerSecond { get; set; }
    }

    class Program
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        static Task<int> Main(string[] args)
        {
            return Parser.Default.ParseArguments<Options>(args).MapResult(
                options => Run(options),
                _ => Task.FromResult(1)
            );
        }

        private static async Task<int> Run(Options options)
        {
            var requestTracker = new RequestTracker();

            await Task.WhenAll(
                PrintResults(requestTracker),
                ExecuteRequests(options.Url, options.RequestsPerSecond, requestTracker));

            return 0;
        }

        private static async Task PrintResults(RequestTracker requestTracker)
        {
            var last = DateTime.MinValue;
            while (true)
            {
                var now = DateTime.Now;

                var (requestsStarted, requestsCompleted, averageLatency,
                     recentRequestsStarted, recentRequestsCompleted, recentAverageLatency) = requestTracker.GetSummary(last, now);

                Console.WriteLine($"Total: {requestsStarted}/{requestsCompleted}/{Math.Round(averageLatency.TotalMilliseconds)}" +
                    $", Recent: {recentRequestsStarted}/{recentRequestsCompleted}/{Math.Round(recentAverageLatency.TotalMilliseconds)}");

                last = now;
                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }

        private static async Task ExecuteRequests(string url, int requestsPerSecond, RequestTracker requestTracker)
        {
            var start = DateTime.Now;

            while (true)
            {
                var now = DateTime.Now;
                var elapsed = now - start;
                var expectedRequestsStarted = requestsPerSecond * elapsed.TotalSeconds;
                var requestsStarted = requestTracker.RequestsStarted;

                if (requestsStarted < expectedRequestsStarted)
                {
                    requestTracker.RequestStarted();
                    ExecuteRequest(url, requestTracker);
                }
                else
                {
                    await Task.Delay(TimeSpan.FromSeconds(1 / requestsPerSecond));
                }
            }
        }

        private static async Task ExecuteRequest(string url, RequestTracker requestTracker)
        {
            var requestStarted = DateTime.Now;
            await _httpClient.GetAsync(url);
            requestTracker.CompleteRequest(DateTime.Now - requestStarted);
        }
    }
}

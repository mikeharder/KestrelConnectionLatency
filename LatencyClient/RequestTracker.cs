using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LatencyClient
{
    class RequestTracker
    {
        private ConcurrentStack<DateTime> _requestsStarted = new ConcurrentStack<DateTime>();
        private ConcurrentStack<(DateTime Completed, TimeSpan Latency)> _requestsCompleted =
            new ConcurrentStack<(DateTime Completed, TimeSpan Latency)>();

        public (int TotalRequestsStarted, int TotalRequestsCompleted, TimeSpan TotalAverageLatency,
                int SelectedRequestsStarted, int SelectedRequestsCompleted, TimeSpan SelectedAverageLatency)
            GetSummary(DateTime startInclusive, DateTime endExclusive)
        {
            var selectedRequestsStarted = _requestsStarted.Where(d => d >= startInclusive && d < endExclusive);
            var selectedRequestsCompleted = _requestsCompleted.Where(d => d.Completed >= startInclusive && d.Completed < endExclusive);

            var totalAverageLatency = _requestsCompleted.Count() > 0 ?
                TimeSpan.FromTicks((long)_requestsCompleted.Select(r => r.Latency.Ticks).Average()) :
                TimeSpan.Zero;

            var selectedAverageLatency = selectedRequestsCompleted.Count() > 0 ?
                TimeSpan.FromTicks((long)selectedRequestsCompleted.Select(r => r.Latency.Ticks).Average()) :
                TimeSpan.Zero;

            return (
                _requestsStarted.Count(),
                _requestsCompleted.Count(),
                totalAverageLatency,
                selectedRequestsStarted.Count(),
                selectedRequestsCompleted.Count(),
                selectedAverageLatency
                );
        }

        public int RequestsStarted => _requestsStarted.Count;

        public void RequestStarted()
        {
            _requestsStarted.Push(DateTime.Now);
        }

        public void CompleteRequest(TimeSpan latency)
        {
            _requestsCompleted.Push((DateTime.Now, latency));
        }
    }
}

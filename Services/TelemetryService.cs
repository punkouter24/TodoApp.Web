using Microsoft.ApplicationInsights;
using TodoApp.Web.Models;

namespace TodoApp.Web.Services
{
    public class TelemetryService
    {
        private readonly TelemetryClient _telemetryClient;

        public TelemetryService(TelemetryClient telemetryClient)
        {
            _telemetryClient = telemetryClient;
        }

        public void TrackTodoCreated(Todo todo)
        {
            _telemetryClient.TrackEvent("TodoCreated", new Dictionary<string, string>
            {
                {"TodoId", todo.Id.ToString()},
                {"Title", todo.Title},
                {"IsComplete", todo.IsComplete.ToString()}
            });
        }

        public void TrackTodoUpdated(Todo todo)
        {
            _telemetryClient.TrackEvent("TodoUpdated", new Dictionary<string, string>
            {
                {"TodoId", todo.Id.ToString()},
                {"Title", todo.Title},
                {"IsComplete", todo.IsComplete.ToString()}
            });
        }

        public void TrackCompletedTodosCount(int count)
        {
            _telemetryClient.TrackMetric("CompletedTodosCount", count);
        }
    }
}
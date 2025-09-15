using System.Text.Json;

namespace RemoteExecutorGateWayApi.Controllers
{
    public class ExecutorJsonRequest
    {
        public int ExecutorType { get; set; }
        public JsonElement RequestBody { get; set; } // Use JsonElement to handle the dynamic content
        public JsonElement Policy { get; set; }
        // Other properties like requestId, correlationId, etc.
    }
}

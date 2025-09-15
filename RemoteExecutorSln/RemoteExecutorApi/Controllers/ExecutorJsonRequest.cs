using System.Text.Json;

namespace RemoteExecutorGateWayApi.Controllers
{
    public class ExecutorJsonRequest
    {
        public int ExecutorType { get; set; }
        public JsonElement RequestBody { get; set; } 
        public JsonElement Policy { get; set; }

    }
}

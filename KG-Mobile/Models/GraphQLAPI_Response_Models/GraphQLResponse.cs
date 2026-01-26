using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace KG.Mobile.Models.GraphQLAPI_Response_Models
{
    using System.Text.Json.Serialization;

    public class GraphQLResponse<T>
    {
        [JsonPropertyName("data")]
        public T Data { get; set; }

        [JsonPropertyName("errors")]
        public List<GraphQLError> Errors { get; set; }

        public bool HasErrors => Errors != null && Errors.Count > 0;
    }

    public class GraphQLError
    {
        [JsonPropertyName("message")]
        public string Message { get; set; }
    }

}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Formatting;

namespace SubmitToEventGridTopic
{

    // from https://blogs.msdn.microsoft.com/brandonh/2018/02/06/publishing-to-an-event-grid-topic-from-net/
    class EventGridObject
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        private readonly Uri _endpoint;
        private readonly string _sasKey;

        public EventGridObject(string topicEndpoint, string sasKey, string subject, string eventType, string id = null, DateTime? eventTime = null) : this(new Uri(topicEndpoint), sasKey, subject, eventType, id, eventTime) { }
        public EventGridObject(Uri topicEndpoint, string sasKey, string subject, string eventType, string id = null, DateTime? eventTime = null)
        {
            _endpoint = topicEndpoint ?? throw new ArgumentNullException(nameof(topicEndpoint));
            _sasKey = !string.IsNullOrWhiteSpace(sasKey) ? sasKey : throw new ArgumentNullException(nameof(sasKey));
            Id = string.IsNullOrWhiteSpace(id) ? Guid.NewGuid().ToString() : id;
            Subject = !string.IsNullOrWhiteSpace(subject) ? subject : throw new ArgumentNullException(nameof(subject));
            EventType = !string.IsNullOrWhiteSpace(eventType) ? eventType : throw new ArgumentNullException(nameof(eventType));
            EventTime = eventTime ?? DateTime.UtcNow;

            // http://byterot.blogspot.co.uk/2016/07/singleton-httpclient-dns.html
            System.Net.ServicePointManager.FindServicePoint(_endpoint).ConnectionLeaseTimeout = (int)TimeSpan.FromMinutes(1).TotalMilliseconds;
        }

        [JsonProperty(@"data")]
        public object Data { get; set; }
        [JsonProperty(@"id")]
        public string Id { get; }
        [JsonProperty(@"subject")]
        public string Subject { get; }
        [JsonProperty(@"eventType")]
        public string EventType { get; }
        [JsonProperty(@"eventTime")]
        public DateTime EventTime { get; }

        public async Task<HttpResponseMessage> SendAsync()
        {
            if (_httpClient.DefaultRequestHeaders.Contains(@"aeg-sas-key"))
                _httpClient.DefaultRequestHeaders.Remove(@"aeg-sas-key");
            _httpClient.DefaultRequestHeaders.Add(@"aeg-sas-key", _sasKey);

            return await _httpClient.PostAsJsonAsync(_endpoint.ToString(), new[] { this });
        }
    }
}

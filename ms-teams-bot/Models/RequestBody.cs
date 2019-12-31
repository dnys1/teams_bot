using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BC.ServerTeamsBot.Models
{
    public class From
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string AadObjectId { get; set; }
    }

    public class Conversation
    {
        public string ConversationType { get; set; }
        public string TenantId { get; set; }
        public string ID { get; set; }
    }

    public class Recipient
    {
        public string ID { get; set; }
        public string Name { get; set; }
    }

    public class Entity
    {
        public string Locale { get; set; }
        public string Country { get; set; }
        public string Platform { get; set; }
        public string Type { get; set; }
    }

    public class Tenant
    {
        public string ID { get; set; }
    }

    public class Source
    {
        public string Name { get; set; }
    }

    public class ChannelData
    {
        public Tenant Tenant { get; set; }
        public Source Source { get; set; }
    }

    public class Data
    {
        public string Link { get; set; }
        public string GUID { get; set; }
    }

    public class Context
    {
        public string Theme { get; set; }
    }

    public class Value
    {
        public string CommandId { get; set; }
        public string CommandContext { get; set; }
        public Data Data { get; set; }
        public Context Context { get; set; }
    }

    public class RequestBody
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public DateTime Timestamp { get; set; }
        public DateTime LocalTimestamp { get; set; }
        public string ID { get; set; }
        public string ChannelId { get; set; }
        public string ServiceUrl { get; set; }
        public From From { get; set; }
        public Conversation Conversation { get; set; }
        public Recipient Recipient { get; set; }
        public List<Entity> Entities { get; set; }
        public ChannelData channelData { get; set; }
        public Value Value { get; set; }
        public string Locale { get; set; }
    }
}

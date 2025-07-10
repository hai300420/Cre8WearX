using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace _4_BusinessObject.RequestDTO
{
    public class SepayWebhookRequest
    {
        public int Id { get; set; }
        public string Gateway { get; set; }

        [JsonConverter(typeof(DateTimeConverterUsingParse))]
        public DateTime TransactionDate { get; set; }
        public string AccountNumber { get; set; }

        //public string Code { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? Code { get; set; }
        public string? SubAccount { get; set; }
        public string TransferType { get; set; }
        public decimal TransferAmount { get; set; }
        public decimal Accumulated { get; set; }
        public string ReferenceCode { get; set; }
        public string Description { get; set; }
    }

    public class DateTimeConverterUsingParse : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var stringValue = reader.GetString();
            return DateTime.Parse(stringValue); // fallback to all .NET supported formats
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString("O")); // ISO 8601
        }
    }

}

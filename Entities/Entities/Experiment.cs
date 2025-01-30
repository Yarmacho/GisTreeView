using System;
using System.Text.Json.Serialization;

namespace Entities
{
    [Serializable]
    public class Experiment : EntityBase<int>
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("desc")]
        public string Description { get; set; }

        public override string ToString()
        {
            return string.Format("Id: {3}{1}Name: {0}{1}Description: {2}{1}", Name, Environment.NewLine,
                Description, Id);
        }
    }
}

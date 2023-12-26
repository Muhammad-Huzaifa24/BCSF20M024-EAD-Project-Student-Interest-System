using System.Runtime.Serialization;

namespace EAD_Project_II.Models
{
    // DataContract for serializing data - required to serve in JSON format
    [DataContract]
    public class DataPoint
    {
        public DataPoint(string label, double y)
        {
            this.Label = label;
            this.Y = y;
        }

        // Explicitly setting the name to be used while serializing to JSON.
        [DataMember(Name = "label")]
        public string Label { get; set; }

        // Explicitly setting the name to be used while serializing to JSON.
        [DataMember(Name = "y")]
        public double Y { get; set; }
    }
}

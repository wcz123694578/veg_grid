using System.ComponentModel;

namespace VegGridLayouter.Core.Attributes
{
    public class PropertiesControlDescriptionAttribute : DescriptionAttribute
    {
        public PropertiesControlDescriptionAttribute() : base(string.Empty)
        {
        }

        public PropertiesControlDescriptionAttribute(string description, bool isCollection = false) : base(description)
        {
            IsCollection = isCollection;
        }

        public string Name => Description;
        public bool IsCollection { get; set; } = false;
    }
}

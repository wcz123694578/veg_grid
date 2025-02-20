using ScriptPortal.Vegas;
using System.Xml.Serialization;

namespace VegGridLayouter.Core
{
    public class VegGridLayouterObject
    {
        public VegGridLayouterObject()
        {
            this.CurVegas = VegasManager.Instance;
            this.CurProject = this.CurVegas.Project;
        }

        
        [XmlIgnore]
        public Vegas CurVegas { get; set; }
        [XmlIgnore]
        public Project CurProject { get; set; }
        [XmlIgnore]
        public VegTrack Track { get; set; }

        [XmlIgnore]
        public virtual Effect maskEffect {
            get;
            set;
        }
    }
}

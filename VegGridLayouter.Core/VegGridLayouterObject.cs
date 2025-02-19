using ScriptPortal.Vegas;

namespace VegGridLayouter.Core
{
    public class VegGridLayouterObject
    {
        public VegGridLayouterObject()
        {
            this.CurVegas = VegasManager.Instance;
            this.CurProject = this.CurVegas.Project;
        }

        

        public Vegas CurVegas { get; set; }
        public Project CurProject { get; set; }
        public VegTrack Track { get; set; }
    }
}

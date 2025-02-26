using ScriptPortal.Vegas;
using System.Threading;

namespace VegGridLayouter.Core
{
    public enum ScriptStateType
    {
        Processing,
        WaitToGenerate
    }

    public static class VegasManager
    {
        public static Vegas Instance { get; set; }
    }
}

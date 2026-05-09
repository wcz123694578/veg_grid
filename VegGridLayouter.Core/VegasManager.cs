using ScriptPortal.Vegas;

namespace VegGridLayouter.Core
{
    public enum ScriptStateType
    {
        Processing,
        WaitToGenerate
    }

    public static class VegasContextFactory
    {
        public static Vegas Context { get; private set; }

        public static void Initialize(Vegas vegas)
        {
            Context = vegas;
        }
    }
}

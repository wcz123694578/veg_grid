using ScriptPortal.Vegas;
using static VegGridLayouter.Core.VegasContextFactory;

namespace VegGridLayouter.Core
{
    public class VegTrackHelper
    {
        public VegTrackHelper()
        {

        }

        public static VideoEvent AddTrackWithPlugIn(VegTrack vegTrack, PlugInNode plugIn)
        {
            Media media = new Media(plugIn);
            VideoEvent videoEvent = vegTrack.VegasTrack.AddVideoEvent(new Timecode("0.0.0"), new Timecode("3.0.0"));
            vegTrack.VegasTrack.Effects.AddEffect(plugIn);
            return videoEvent;
        }

        public static Effect AddVideoFX(VegTrack track, PlugInNode plugIn)
        {
            // return videoEvent.Effects.AddEffect(plugIn);
            return track.VegasTrack.Effects.AddEffect(plugIn);

            
        }

        public static VideoTrack AppendTrack()
        {
            VideoTrack videoTrack = new VideoTrack(Context.Project.Tracks.Count);
            Context.Project.Tracks.Add(videoTrack);
            return videoTrack;
        }

        public static VideoTrack AppendTrack(string name)
        {
            VideoTrack videoTrack = new VideoTrack(Context.Project.Tracks.Count, name);
            Context.Project.Tracks.Add(videoTrack);
            return videoTrack;
        }
    }
}

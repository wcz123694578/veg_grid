using ScriptPortal.Vegas;

namespace VegGridLayouter.Core.Element
{
    public class VegEventHelper
    {
        private VegEventHelper()
        {
            
        }

        public static VideoEvent AddEventWithPlugIn(VegTrack vegTrack, PlugInNode plugIn)
        {
            Media media = new Media(plugIn);
            VideoEvent videoEvent = vegTrack.VegasTrack.AddVideoEvent(new Timecode("0.0.0"), new Timecode("3.0.0"));
            Take take = videoEvent.AddTake(media.GetVideoStreamByIndex(0));
            return videoEvent;
        }

        public static Effect AddVideoFX(VideoEvent videoEvent, PlugInNode plugIn)
        {
            return videoEvent.Effects.AddEffect(plugIn);
        }
    }
}

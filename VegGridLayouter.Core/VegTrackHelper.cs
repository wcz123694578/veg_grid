using ScriptPortal.Vegas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public static VideoTrack AppendTrack(Vegas curVegas)
        {
            VideoTrack videoTrack = new VideoTrack(curVegas.Project.Tracks.Count);
            curVegas.Project.Tracks.Add(videoTrack);
            return videoTrack;
        }

        public static VideoTrack AppendTrack(Vegas curVegas, string name)
        {
            VideoTrack videoTrack = new VideoTrack(curVegas.Project.Tracks.Count, name);
            curVegas.Project.Tracks.Add(videoTrack);
            return videoTrack;
        }
    }
}

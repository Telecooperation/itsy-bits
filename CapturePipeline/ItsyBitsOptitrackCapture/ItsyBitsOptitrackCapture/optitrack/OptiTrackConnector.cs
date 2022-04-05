using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace OptiTrack
{

    public class OptiTrackConnector
    {
        private bool _stop = true;
        private Thread _tracker;

        public delegate void OnFrameReady(Frame frame);

        public bool Init(String path)
        {
            int a = OptiTrackNativeWrapper.TT_Initialize();
            int b = OptiTrackNativeWrapper.TT_LoadProject(path);
            Console.WriteLine("a:" + a + ", b:" + b);
            if (a + b != 0)
            {
                // breakpoint
            }
            return a + b == 0;
        }

        public void StartTracking(OnFrameReady frameReadyDelegate)
        {
            _stop = false;
            _tracker = new Thread(delegate ()
            {
                HighPerformanceTimer hpt = new HighPerformanceTimer();
                int i = 0;

                hpt.Start();
                while (!_stop)
                {
                    if (i++ == 200)
                    {
                        //Console.WriteLine("Tracker FPS: " + 200.0 / hpt.Stop());
                        i = 0;
                        hpt.Start();
                    }
                    OptiTrackNativeWrapper.TT_Update();

                    frameReadyDelegate(BuildFrame());

                    Thread.Sleep(34); //15 originally
                }
            });
            _tracker.Start();
        }

        private Frame _lastFrame;

        private Frame BuildFrame()
        {
            Frame frame = new Frame(OptiTrackNativeWrapper.TT_FrameMarkerCount(), OptiTrackNativeWrapper.TT_TrackableCount());

            for (int i = 0; i < OptiTrackNativeWrapper.TT_FrameMarkerCount(); i++)
            {
                frame.Markers[i] = new Marker(
                    OptiTrackNativeWrapper.TT_FrameMarkerX(i),
                    OptiTrackNativeWrapper.TT_FrameMarkerY(i),
                    OptiTrackNativeWrapper.TT_FrameMarkerZ(i))
                {
                    BoundToTrackable = null
                };
            }

            for (int i = 0; i < OptiTrackNativeWrapper.TT_TrackableCount(); i++)
            {
                Trackable t = new Trackable
                {
                    IsTracked = OptiTrackNativeWrapper.TT_IsTrackableTracked(i),
                    Id = OptiTrackNativeWrapper.TT_TrackableID(i)
                };

                if (t.IsTracked)
                {
                    OptiTrackNativeWrapper.TT_TrackableLocation(i, out t.X, out t.Y, out t.Z, out t.Qx, out t.Qy, out t.Qz, out t.Qw,
                                                             out t.Yaw, out t.Pitch, out t.Roll);

                    if (_lastFrame != null)
                    {
                        if (_lastFrame.Trackables[i].IsAlmostSameCoordinates(t))
                        {
                            t.IsTracked = false;
                        }
                    }

                    /*for (int j = 0; j < OptiTrackNativeWrapper.TT_TrackableMarkerCount(i); j++)
                    {
                        float markerRadius = trackableMarker(i, j);

                        frame.markers[boundTrackable(frame, markerRadius,t)].boundToTrackable = t;
                    }*/

                }

                frame.Trackables[i] = t;
            }
            _lastFrame = frame;

            return frame;
        }

        /*private float trackableMarker(int trackableId, int markerId)
        {
            Marker tempMarker = new Marker(0, 0, 0);

            OptiTrackNativeWrapper.TT_TrackableMarker(trackableId, markerId, out tempMarker.x, out tempMarker.y, out tempMarker.z);

            float markerRadius = (float)Math.Pow(((float)Math.Pow(tempMarker.x, 2.00) + (float)Math.Pow(tempMarker.y, 2.00) + (float)Math.Pow(tempMarker.z, 2.00)),0.50);
            markerRadius += (float)0.01;

            return markerRadius;
        }

        private int boundTrackable(Frame frame, float markerRadius, Trackable trackable)
        {
            List<float> distanceValues = new List<float>();

            for (int k = 0; k < frame.markers.Count(); k++)
                distanceValues.Add((float)Math.Pow(((float)Math.Pow(trackable.x - frame.markers[k].x, 2.00) + (float)Math.Pow(trackable.y - frame.markers[k].y, 2.00) + (float)Math.Pow(trackable.z - frame.markers[k].z, 2.00)),0.50));

            float minimumDistance = markerRadius;
            
            int boundMarkerId = 0;

            for (int i = 0; i < distanceValues.Count; ++i)
            {
               
                if (distanceValues[i] <= minimumDistance)
                {
                    minimumDistance = distanceValues[i];
                    boundMarkerId = i;
                }                           
            }

            return boundMarkerId;
        }*/

        public void StopTracking()
        {
            _stop = true;
            OptiTrackNativeWrapper.TT_Shutdown();
        }
    }

    internal class OptiTrackNativeWrapper
    {
        [DllImport("NPTrackingTools.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int TT_Initialize();

        [DllImport("NPTrackingTools.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int TT_Shutdown();

        [DllImport("NPTrackingTools.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int TT_LoadProject([MarshalAs(UnmanagedType.LPStr)] string file);

        [DllImport("NPTrackingTools.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int TT_Update();

        [DllImport("NPTrackingTools.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int TT_FrameMarkerCount();

        [DllImport("NPTrackingTools.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int TT_TrackableCount();

        [DllImport("NPTrackingTools.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int TT_TrackableLocation([In] int index, [Out] out float x, [Out] out float y, [Out] out float z, [Out] out float qx, [Out] out float qy, [Out] out float qz, [Out] out float qw, [Out] out float yaw, [Out] out float pitch, [Out] out float roll);

        [DllImport("NPTrackingTools.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void TT_TrackableMarker([In] int rigidIndex, [In] int markerIndex, [Out] out float x, [Out] out float y, [Out] out float z);

        [DllImport("NPTrackingTools.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern float TT_FrameMarkerX([In] int index);

        [DllImport("NPTrackingTools.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern float TT_FrameMarkerY([In] int index);

        [DllImport("NPTrackingTools.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern float TT_FrameMarkerZ([In] int index);

        [DllImport("NPTrackingTools.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool TT_IsTrackableTracked([In] int index);

        [DllImport("NPTrackingTools.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int TT_TrackableID(int index);

        [DllImport("NPTrackingTools.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int TT_TrackableMarkerCount(int index);

    }
}

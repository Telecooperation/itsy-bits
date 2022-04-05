namespace OptiTrack
{
    public class Frame
    {
        public Trackable[] Trackables;
        public Marker[] Markers;

        public Frame(int markerCount, int trackableCount)
        {
            Trackables = new Trackable[trackableCount];
            Markers = new Marker[markerCount];
        }
    }

    public class Trackable
    {
        public int Id;
        public float X, Y, Z, Qx, Qy, Qz, Qw, Pitch, Roll, Yaw;
        public bool IsTracked;

        internal bool IsAlmostSameCoordinates(Trackable t)
        {
            return X == t.X && Y == t.Y && Z == t.Z;
        }
    }

    public class Marker
    {
        public float X, Y, Z;
        public Trackable BoundToTrackable;

        public Marker(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }
}

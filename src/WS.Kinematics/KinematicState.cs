namespace WS.Kinematics
{
    /// <summary>
    /// Represents the kinematic state of a body at a point in time:
    /// position, velocity, and acceleration.
    /// </summary>
    public class KinematicState
    {
        public Vector3D Position { get; set; }
        public Vector3D Velocity { get; set; }
        public Vector3D Acceleration { get; set; }

        public KinematicState()
        {
            Position = Vector3D.Zero;
            Velocity = Vector3D.Zero;
            Acceleration = Vector3D.Zero;
        }

        public KinematicState(Vector3D position, Vector3D velocity, Vector3D acceleration)
        {
            Position = position;
            Velocity = velocity;
            Acceleration = acceleration;
        }
    }
}

namespace WS.Kinematics
{
    /// <summary>
    /// Provides kinematic equations for bodies under constant acceleration
    /// (SUVAT equations).
    /// </summary>
    public static class LinearKinematics
    {
        /// <summary>
        /// Calculates the new kinematic state after a time step using
        /// constant-acceleration integration (Euler method).
        /// </summary>
        /// <param name="state">The current kinematic state.</param>
        /// <param name="deltaTime">The time step in seconds.</param>
        /// <returns>The updated kinematic state.</returns>
        public static KinematicState Step(KinematicState state, double deltaTime)
        {
            var newVelocity = state.Velocity + state.Acceleration * deltaTime;
            var newPosition = state.Position + state.Velocity * deltaTime
                              + state.Acceleration * (0.5 * deltaTime * deltaTime);

            return new KinematicState(newPosition, newVelocity, state.Acceleration);
        }

        /// <summary>
        /// Calculates velocity given initial velocity, constant acceleration, and time.
        /// v = u + at
        /// </summary>
        public static Vector3D FinalVelocity(Vector3D initialVelocity, Vector3D acceleration, double time) =>
            initialVelocity + acceleration * time;

        /// <summary>
        /// Calculates displacement given initial velocity, constant acceleration, and time.
        /// s = ut + 0.5*a*t^2
        /// </summary>
        public static Vector3D Displacement(Vector3D initialVelocity, Vector3D acceleration, double time) =>
            initialVelocity * time + acceleration * (0.5 * time * time);
    }
}

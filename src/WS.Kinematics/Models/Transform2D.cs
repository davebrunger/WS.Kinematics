namespace WS.Kinematics.Models;

public record Transform2D(Point2D Point, AngleRadians Rotation, Vector2D Velocity, AngleRadians AngularVelocity)
{
    public Point2D Translate(Point2D localPoint)
    {
        var cos = Math.Cos(Rotation);
        var sin = Math.Sin(Rotation);
        var translation = new Point2D(
            localPoint.X * cos - localPoint.Y * sin,
            localPoint.X * sin + localPoint.Y * cos
        );
        return translation + Point;
    }

    public Transform2D Step(double deltaTime)
    {
        var newPoint = Point + Velocity * deltaTime;
        var newRotation = Rotation + AngularVelocity * deltaTime;
        return new Transform2D(newPoint, newRotation, Velocity, AngularVelocity);
    }
}

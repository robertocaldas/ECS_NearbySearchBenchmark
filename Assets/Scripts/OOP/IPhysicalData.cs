using System.Collections.Generic;

public interface IPhysicalData
{
    float X { get; set; }
    float Y { get; set; }
    void SetPosition(float x, float y);
    void AddPosition(float x, float y);
    float Distance(Entity target);
    // Normalized direction to target.
    (float, float) DirectionTo(Entity target);
    IReadOnlyList<Entity> GetNearbyEntities(float radius);
}

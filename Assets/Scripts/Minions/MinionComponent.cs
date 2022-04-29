using Unity.Entities;

[GenerateAuthoringComponent]
public struct MinionComponent : IComponentData {
    public int maxHealth;
    public int currentHealth;
    public int owner;
    public int damage;
    public int speed;
}

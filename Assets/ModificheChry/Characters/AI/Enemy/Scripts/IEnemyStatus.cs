
public interface IEnemyStatus
{
    float Health { get; set; }
    bool IsDead { get; set; }
    void TakeDamage(float damage);
    bool IsEnemyAlive();
}

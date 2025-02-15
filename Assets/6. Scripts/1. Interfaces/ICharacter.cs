public interface ICharacter
{
    string Name { get; }
    CharacterStats Stats { get; }
    CharacterCombat Combat { get; }

    void TakeDamage(int damage);
    void Heal(int amount);
    void UseActionPoints(int amount);
}

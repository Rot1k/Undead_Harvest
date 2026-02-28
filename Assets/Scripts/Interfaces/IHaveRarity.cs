public enum Rarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}
public interface IHaveRarity
{
    Rarity Rarity { get; }
}

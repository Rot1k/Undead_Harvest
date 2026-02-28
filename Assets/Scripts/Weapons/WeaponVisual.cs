using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class WeaponVisual : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private RarityConfigSO _rarityConfigSO;

    private MaterialPropertyBlock _materialPropertyBlock;

    private static readonly int MASK_TEX_ID = Shader.PropertyToID("_MaskTex");
    private static readonly int RARITY_COLOR_ID = Shader.PropertyToID("_RarityColor");

    private void Awake()
    {
        _materialPropertyBlock = new MaterialPropertyBlock();
    }

    public void Apply(WeaponSO weaponSO)
    {
        //_spriteRenderer.sprite = weaponSO.UISprite;

        _spriteRenderer.GetPropertyBlock(_materialPropertyBlock);

        _materialPropertyBlock.SetTexture(MASK_TEX_ID, weaponSO.MaskSprite.texture);
        _materialPropertyBlock.SetColor(RARITY_COLOR_ID, _rarityConfigSO.GetColor(weaponSO.Rarity));

        _spriteRenderer.SetPropertyBlock(_materialPropertyBlock);
    }
}
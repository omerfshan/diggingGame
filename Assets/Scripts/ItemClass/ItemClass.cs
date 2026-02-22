using UnityEngine;

public class ItemClass 
{
    [SerializeField] private ItemType _itemType;
    [SerializeField] private float _health;
    [SerializeField] private float _attackPow;

    public ItemType ItemType=>_itemType;
    public float Health=>_health;
    public float AttackPow=>_attackPow;
}

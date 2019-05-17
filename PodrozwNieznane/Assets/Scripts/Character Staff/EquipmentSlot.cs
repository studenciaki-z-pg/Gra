using UnityEngine;
using UnityEngine.UI;
public class EquipmentSlot : ItemSlot
{
    public EquipmentType EquipmentType;

    private void Start()
    {
        OnValidate();
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        gameObject.name = EquipmentType.ToString() + " Slot";
    }


}

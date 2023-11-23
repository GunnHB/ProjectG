using TMPro;

using UnityEngine;

public class UIItemInteract : UIInteractBase
{
    [SerializeField] private GameObject _interactBackground;
    [SerializeField] private TextMeshProUGUI _interactText;

    private ItemBase _item;

    protected override void Awake()
    {
        base.Awake();
    }

    public void ShowInteractUI(ItemBase item)
    {
        Show();

        _item = item;

        UpdatePosition();
        _interactText.text = $"{item} 줍기";
    }

    public void UpdatePosition()
    {
        if (_item == null)
            return;
    }

    public void CloseInteractUI(ItemBase item = null)
    {
        // 닫힐 때 파라미터가 비었으면 걍 끄기만
        // 아니면 인벤토리에 해당 아이템 넣어줌
        if (item != null)
        {
            ItemManager.Instance.AddItemToInventory(item.ItemData);
            ItemManager.Instance.DestoryItem(item);
        }

        Hide();
    }
}
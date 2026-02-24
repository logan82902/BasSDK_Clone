using UnityEngine;
using System;
using TMPro;


using TriInspector;


public class ShopTransactionUILine : MonoBehaviour
{
    public TextMeshProUGUI textName;
	public TextMeshProUGUI textQuantity;
    public TextMeshProUGUI textPrice;

    [NonSerialized]
    public int itemHashId;

    public string itemName
    {
        get { return textName.text; }
        set { textName.text = value; }
    }

    public int quantity
    {
        get { return _Quantity; } 
        set { _Quantity = value; textQuantity.text = $"{_Quantity}x"; textPrice.text = $"{unitPrice * _Quantity}"; }
    }
    protected int _Quantity = 1;

    public int unitPrice
    {
        get { return _unitPrice; }
        set { _unitPrice = value; textPrice.text = $"{_unitPrice * _Quantity}"; }
    }
    protected int _unitPrice;
}
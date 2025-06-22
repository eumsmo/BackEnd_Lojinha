using UnityEngine;
using UnityEngine.UI;

public class SlotUI : MonoBehaviour {
    public Text tituloTxt, precoTxt;
    public Image iconImg;

    public void SetSlot(Item item) {
        tituloTxt.text = item.nome;
        precoTxt.text = "$ " + item.preco;
        iconImg.sprite = item.icone;
    }
}

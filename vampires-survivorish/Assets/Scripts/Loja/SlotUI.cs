using UnityEngine;
using UnityEngine.UI;

public class SlotUI : MonoBehaviour {
    public Text tituloTxt, precoTxt;
    public Image iconImg;
    public Item item;

    public void SetSlot() {
        SetSlot(new Item()); // Reset the slot to an empty item
    }

    public void SetSlot(Item item) {
        this.item = item;

        if (!item.isNull) {
            int nivel = GameManager.instance.GetQuantidadeItem(item.nome);

            tituloTxt.text = item.nome;
            precoTxt.text = "$ " + item.GetPrecoReal(nivel);
            iconImg.sprite = item.icone;
            iconImg.color = Color.white; // Reset color to white when item is set
        } else {
            tituloTxt.text = "";
            precoTxt.text = "";
            iconImg.sprite = null;
            iconImg.color = new Color(1, 1, 1, 0); // Make icon transparent when no item is set
        }
    }

    public void TentarComprar() {
        UIController.loja.TentarComprar(this);
    }
}

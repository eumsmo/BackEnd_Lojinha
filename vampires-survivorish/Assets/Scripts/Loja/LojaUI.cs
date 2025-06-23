using System.Collections.Generic;
using Unity.Entities.UniversalDelegates;
using UnityEngine;
using UnityEngine.UI;

public class LojaUI : MonoBehaviour {
    public SlotUI[] slots;
    public Text descricaoTxt;

    public uint precoReset = 10;

    public void ToggleLoja() {
        bool mostrar = !gameObject.activeSelf;
        MostrarLoja(mostrar);
    }

    public void MostrarLoja(bool mostrar) {
        gameObject.SetActive(mostrar);
        Time.timeScale = mostrar ? 0 : 1;
    }
    
    public void PegarItens(bool apenasVazios = true) {
        List<SlotUI> slotsVazios = new List<SlotUI>();
        foreach (SlotUI slot in slots) {
            if (slot.item.isNull || !apenasVazios) {
                slotsVazios.Add(slot);
            }
        }

        if (slotsVazios.Count > 0)
            RequisitarItens(slotsVazios);
    }

    public void RequisitarItens(List<SlotUI> slotsVazios) {
        int quant = slotsVazios.Count;

        List<Item> itens = new List<Item>();
        for (int i = 0; i < quant; i++) {
            Item item = GameManager.instance.itens[Random.Range(0, GameManager.instance.itens.Length)];
            itens.Add(item);
        }

        for (int i = 0; i < quant; i++) {
            slotsVazios[i].SetSlot(itens[i]);
        }
    }

    public void TentarComprar(SlotUI slot) {
        if (slot.item.isNull) return;

        int nivel = GameManager.instance.GetQuantidadeItem(slot.item.nome);
        int preco = slot.item.GetPrecoReal(nivel);

        uint xpAtual = GameManager.instance.xp;
        if (xpAtual >= preco) {
            uint xpRestante = xpAtual - ((uint) preco);
            GameManager.instance.ChangeXPInGame(xpRestante);
            RequisitarItem(slot.item.nome);
            slot.SetSlot();
            PegarItens();
            MostrarLoja(false);
        } else {
            Debug.Log("XP insuficiente para comprar " + slot.item.nome);
        }
    }

    public void RequisitarItem(string nome) {
        Debug.Log("Requisitando item: " + nome);
        string json = Resources.Load<TextAsset>(nome).text;
        Item item = ItemAdapter.FromItemJSON(JsonUtility.FromJson<ItemJSON>(json));
        GameManager.instance.AddItem(item);
    }


    public void TentarComprarReset() {
        uint xpAtual = GameManager.instance.xp;
        if (GameManager.instance.xp >= precoReset) {
            uint xpRestante = GameManager.instance.xp - precoReset;
            GameManager.instance.ChangeXPInGame(xpRestante);
        } else {
            Debug.Log("XP insuficiente para comprar");
        }
    }

    
}

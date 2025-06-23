using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class LojaUI : MonoBehaviour {
    public GameObject lojaPanel;
    public SlotUI[] slots;
    public Text descricaoTxt, resetQuantidadeTxt;
    public float precoResetProgression = 1.5f;
    public uint precoReset = 10;


    public string linkBase = "localhost:80";
    public string linkVitrine = "/vitrine.php";
    public string linkComprar = "/comprar.php";

    void Awake() {
        if (ManterInfo.host != null && ManterInfo.host != "") {
            linkBase = ManterInfo.host;
        }
    }

    public void ToggleLoja() {
        bool mostrar = !lojaPanel.activeSelf;
        MostrarLoja(mostrar);
    }

    public void MostrarLoja(bool mostrar) {
        lojaPanel.SetActive(mostrar);
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

    public void ForcarResetItens() {
        if (GameManager.instance.xp < precoReset) return;

        GameManager.instance.ChangeXPInGame(GameManager.instance.xp - precoReset);

        precoReset = (uint)(precoReset * precoResetProgression);
        resetQuantidadeTxt.text = "$ " + precoReset;


        PegarItens(false);
        MostrarLoja(false);
    }


    #region Vitrine
    // Chama vitrine para requisitar itens
    public void RequisitarItens(List<SlotUI> slotsVazios) {
        if (slotsVazios.Count > 0) {
            StartCoroutine(CarregarVitrine(slotsVazios));
        }
    }

    IEnumerator CarregarVitrine(List<SlotUI> slotsVazios) {
        int quantidade = slotsVazios.Count;
        string link = linkBase + linkVitrine + "?quantidade=" + quantidade;

        using (UnityWebRequest webRequest = UnityWebRequest.Get(link)) {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError) {
                Debug.LogError($"Erro ao carregar vitrine: {webRequest.error}");
            } else {
                ResultadoVitrine(JsonUtility.FromJson<ItemResponse>(webRequest.downloadHandler.text), slotsVazios);
            }
        }
    }

    void ResultadoVitrine(ItemResponse response, List<SlotUI> slots) {
        if (response.status == "success") {
            if (response.quantidade > 0) {
                Item[] itens = ItemAdapter.FromItemJSONArray(response.itens);
                for (int i = 0; i < response.quantidade && i < slots.Count; i++) {
                    slots[i].SetSlot(itens[i]);
                }
                Debug.Log($"Vitrine carregada com {response.quantidade} itens.");
            } else {
                Debug.Log("Nenhum item disponível na vitrine.");
            }
        } else {
            Debug.LogError("Erro ao carregar vitrine: " + response.mensagem);
        }
    }

    
    #endregion



    #region Comprar
    public void TentarComprar(SlotUI slot) {
        if (slot.item.isNull) return;

        int nivel = GameManager.instance.GetQuantidadeItem(slot.item.nome);
        StartCoroutine(ComprarItem(slot.item.nome, nivel, slot));
    }

    IEnumerator ComprarItem(string nome, int nivel, SlotUI slot) {
        string link = linkBase + linkComprar + "?nome=" + nome + "&nivel=" + nivel + "&xp=" + GameManager.instance.xp;

        using (UnityWebRequest webRequest = UnityWebRequest.Get(link)) {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError) {
                Debug.LogError($"Erro ao carregar item '{nome}': {webRequest.error}");
            } else {
                ResultadoCompra(JsonUtility.FromJson<ItemResponse>(webRequest.downloadHandler.text), slot);
            }
        }
    }


    void ResultadoCompra(ItemResponse response, SlotUI slot) {
        if (response.status == "success") {
            GameManager.instance.ChangeXPInGame((uint) response.xpRestante);
            Item item = ItemAdapter.FromItemJSON(response.item);
            GameManager.instance.AddItem(item);

            slot.SetSlot();
            PegarItens();
            MostrarLoja(false);

            Debug.Log($"Item '{item.nome}' comprado com sucesso!");
        } else {
            Debug.LogError("Erro ao comprar item: " + response.mensagem);
        }
    }


    #endregion

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

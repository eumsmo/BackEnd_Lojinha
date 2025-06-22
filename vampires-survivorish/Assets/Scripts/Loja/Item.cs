using UnityEngine;

public class Item {
    public string nome, descricao;
    public Sprite icone;
    public int preco;

    public Item(string nome, string descricao, Sprite icone, int preco) {
        this.nome = nome;
        this.descricao = descricao;
        this.icone = icone;
        this.preco = preco;
    }
}

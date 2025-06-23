using UnityEngine;

public enum TipoItem { Instantaneo, Arma, Passivo }

[System.Serializable]
public struct Item {
    public TipoItem tipo;
    public string nome, descricao;
    public Sprite icone;
    public int preco;

    public bool isNull => string.IsNullOrEmpty(nome);

    public Item(TipoItem tipo, string nome, string descricao, Sprite icone, int preco) {
        this.tipo = tipo;
        this.nome = nome;
        this.descricao = descricao;
        this.icone = icone;
        this.preco = preco;
    }

    public int GetPrecoReal(int nivel = 0) {
        return preco * (nivel + 1);
    }
}

[System.Serializable]
public struct ItemJSON {
    public string tipo;
    public string nome, descricao;
    public string icone;
    public int preco;

    public ItemJSON(string tipo, string nome, string descricao, string icone, int preco) {
        this.tipo = tipo;
        this.nome = nome;
        this.descricao = descricao;
        this.icone = icone;
        this.preco = preco;
    }
}


[System.Serializable]
public struct ItemList {
    public ItemJSON[] itens;
    public int count;

    public ItemList(ItemJSON[] itens) {
        this.itens = itens;
        this.count = itens.Length;
    }
}
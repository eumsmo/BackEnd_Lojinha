using UnityEngine;

public class ItemAdapter {
    public static ItemJSON ToItemJSON(Item item) {
        return new ItemJSON(item.tipo.ToString(), item.nome, item.descricao, item.icone.name, item.preco);
    }

    public static Item FromItemJSON(ItemJSON itemJSON) {
        TipoItem tipo = (TipoItem)System.Enum.Parse(typeof(TipoItem), itemJSON.tipo);
        Sprite icone = Resources.Load<Sprite>(itemJSON.icone);
        return new Item(tipo, itemJSON.nome, itemJSON.descricao, icone, itemJSON.preco);
    }

    public static Item[] FromItemJSONArray(ItemJSON[] itemJSONs) {
        Item[] items = new Item[itemJSONs.Length];
        for (int i = 0; i < itemJSONs.Length; i++) {
            items[i] = FromItemJSON(itemJSONs[i]);
        }
        return items;
    }
}
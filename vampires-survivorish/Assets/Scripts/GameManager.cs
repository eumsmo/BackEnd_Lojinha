using UnityEngine;
using System.Collections.Generic;
using Unity.Entities;

public class GameManager : MonoBehaviour {
    public static GameManager instance { get; private set; }
    private Actions actions;

    public uint xp = 0;
    Dictionary<Item, int> inventory = new Dictionary<Item, int>();

    void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    void Start() {
        actions = new Actions();
        actions.Enable();

        actions.Game.Space.performed += ctx => UIController.loja.ToggleLoja();
        UIController.loja.PegarItens(false);
    }

    public void SetXP(uint amount) {
        xp = amount;
        UIController.instance.UpdateXP(xp);
    }

    public void ChangeXPInGame(uint value) {
        var _manager = World.DefaultGameObjectInjectionWorld.EntityManager;

        var playerEntity = _manager.CreateEntityQuery(typeof(PlayerData)).GetSingletonEntity();

        AtualizarXPFromMonoToDOTS valor = new AtualizarXPFromMonoToDOTS {
            xp = value
        };

        if (!_manager.HasComponent<AtualizarXPFromMonoToDOTS>(playerEntity)) {
            _manager.AddComponentData(playerEntity, valor);
        } else {
            _manager.SetComponentData(playerEntity, valor);
        }

        _manager.SetComponentEnabled<AtualizarXPFromMonoToDOTS>(playerEntity, true);
        SetXP(value);
    }

    public int GetQuantidadeItem(string nome) {
        foreach (var kvp in inventory) {
            if (kvp.Key.nome == nome) {
                return kvp.Value;
            }
        }
        return 0;
    }

    public void AddItem(Item item) {
        if (inventory.ContainsKey(item)) {
            inventory[item]++;
        } else {
            inventory[item] = 1;
        }

        AplicarEfeitoDoItem(item, inventory[item]);
    }

    public void AplicarEfeitoDoItem(Item item, int quant) {
        switch (item.nome) {
            case "Remendo":
                DOTSRecuperarVida(20);
                break;
            case "Interacao":
                if (quant % 2 == 0) DOTSSetarMouse(0f, 0.2f);
                else DOTSSetarMouse(-0.2f, 0f);
                break;
            case "Pressa":
                DOTSSetarPressa(0.2f);
                break;
            case "Vontade":
                DOTSSetarVontade(1);
                break;
            case "Introspecto":
                DOTSSetarIntrospecto(1);
                break;
        }
    }

     public void DOTSRecuperarVida(uint value) {
        var _manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var playerEntity = _manager.CreateEntityQuery(typeof(PlayerData)).GetSingletonEntity();

        EfeitoRecurarVida valor = new EfeitoRecurarVida { vidaRecuperada = value };

        if (!_manager.HasComponent<EfeitoRecurarVida>(playerEntity)) _manager.AddComponentData(playerEntity, valor);
        else _manager.SetComponentData(playerEntity, valor);
    
        _manager.SetComponentEnabled<EfeitoRecurarVida>(playerEntity, true);
    }

    public void DOTSSetarPressa(float value) {
        var _manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var playerEntity = _manager.CreateEntityQuery(typeof(PlayerData)).GetSingletonEntity();

        SetarPressa valor = new SetarPressa { mult = value };

        if (!_manager.HasComponent<SetarPressa>(playerEntity)) _manager.AddComponentData(playerEntity, valor);
        else _manager.SetComponentData(playerEntity, valor);
    
        _manager.SetComponentEnabled<SetarPressa>(playerEntity, true);
    }

    public void DOTSSetarVontade(int value) {
        var _manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var playerEntity = _manager.CreateEntityQuery(typeof(PlayerData)).GetSingletonEntity();

        SetarVontade valor = new SetarVontade { extra = value };

        if (!_manager.HasComponent<SetarVontade>(playerEntity)) _manager.AddComponentData(playerEntity, valor);
        else _manager.SetComponentData(playerEntity, valor);
    
        _manager.SetComponentEnabled<SetarVontade>(playerEntity, true);
    }

    public void DOTSSetarIntrospecto(int value) {
        var _manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var playerEntity = _manager.CreateEntityQuery(typeof(PlayerData)).GetSingletonEntity();

        SetarIntrospecto valor = new SetarIntrospecto { defesa = value };

        if (!_manager.HasComponent<SetarIntrospecto>(playerEntity)) _manager.AddComponentData(playerEntity, valor);
        else _manager.SetComponentData(playerEntity, valor);
    
        _manager.SetComponentEnabled<SetarIntrospecto>(playerEntity, true);
    }

    public void DOTSSetarMouse(float cooldown, float speed) {
        var _manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var playerEntity = _manager.CreateEntityQuery(typeof(PlayerData)).GetSingletonEntity();

        SetarMouse valor = new SetarMouse { multCooldown = cooldown, multSpeed = speed };

        if (!_manager.HasComponent<SetarMouse>(playerEntity)) _manager.AddComponentData(playerEntity, valor);
        else _manager.SetComponentData(playerEntity, valor);
    
        _manager.SetComponentEnabled<SetarMouse>(playerEntity, true);
    }

}

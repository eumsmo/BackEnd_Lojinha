using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {
    public static UIController instance { get; private set; }
    public static LojaUI loja => instance.lojaUI;
    
    public LojaUI lojaUI;


    [Header("HUD")]
    public Text xpText;

    void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    public void UpdateXP(uint xp) {
        xpText.text = "" + xp;
    }
}

using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {
    public static UIController Instance { get; private set; }


    [Header("HUD")]
    public Text xpText;

    void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    public void UpdateXP(uint xp) {
        xpText.text = "" + xp;
    }
}

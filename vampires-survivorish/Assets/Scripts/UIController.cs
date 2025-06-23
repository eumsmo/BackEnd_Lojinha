using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {
    public static UIController instance { get; private set; }
    public static LojaUI loja => instance.lojaUI;

    public string cenaMenu = "MainMenu";
    public GameObject derrotaPanel;
    
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

    public void VoltarProMenu() {
        Time.timeScale = 1;
        UnityEngine.SceneManagement.SceneManager.LoadScene(cenaMenu);
    }

    public void TentarDenovo() {
        Time.timeScale = 1;
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    public void MostrarDerota(bool mostrar) {
        derrotaPanel.SetActive(mostrar);
        Time.timeScale = mostrar ? 0 : 1;
    }
}

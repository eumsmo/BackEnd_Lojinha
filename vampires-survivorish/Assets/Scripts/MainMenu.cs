using UnityEngine;
using UnityEngine.UI;

public class ManterInfo {
    public static string host;
}

public class MainMenu : MonoBehaviour {
    public InputField hostInputField;
    public Text reclamacaoText;
    public string cenaJogo = "Jogo";

    public void IniciarJogo() {
        if (string.IsNullOrEmpty(hostInputField.text)) {
            reclamacaoText.text = "Host não pode ser vazio!";
            return;
        }

        if (hostInputField.text.EndsWith("/")) {
            hostInputField.text = hostInputField.text.TrimEnd('/');
        }

        ManterInfo.host = hostInputField.text;
        UnityEngine.SceneManagement.SceneManager.LoadScene(cenaJogo);
    }

    public void SairJogo() {
        Application.Quit();
    }
}

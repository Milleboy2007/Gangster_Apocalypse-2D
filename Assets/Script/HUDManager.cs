using UnityEngine;
using TMPro;

public class HUDManager : MonoBehaviour
{
    public TextMeshProUGUI textMun;

    public void updateMunText(int newNb)
    {
        textMun.text = newNb.ToString();
    }
}

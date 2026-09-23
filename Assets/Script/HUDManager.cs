using UnityEngine;
using TMPro;

public class HUDManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMun;

    public void updateMunText(int newNb)
    {
        textMun.text = newNb.ToString();
    }
}

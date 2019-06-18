using UnityEngine;
using UnityEngine.UI;

public class Info : MonoBehaviour
{
    private Info() { }
    public static Info Instance { get; private set; }

    private Text _text;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        _text = GetComponent<Text>();
    }

    /// <summary>
    /// 打印
    /// </summary>
    public void Print(string str, bool warning = false)
    {
        if (warning)
            Debug.LogWarning(str);
        else
            Debug.Log(str);

        _text.text = str;
    }
}
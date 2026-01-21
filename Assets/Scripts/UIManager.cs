using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public Transform CanvasWS;
    static public Transform UICanvasWS;
    public Transform Canvas;
    static public Transform UICanvas;

    public Image healthImage;
    static public Image healthBar;
    public Image silkImage;
    static public Image silkBar;

    public TextMeshProUGUI healthTMP;
    public TextMeshProUGUI silkTMP;
    public static TextMeshProUGUI healthTMPro;
    public static TextMeshProUGUI silkTMPro;



    // Start is called before the first frame update
    void Awake()
    {
        UICanvasWS = CanvasWS;
        UICanvas = Canvas;

        healthBar = healthImage;
        silkBar = silkImage;
        healthTMPro = healthTMP;
        silkTMPro = silkTMP;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void SetHealth(int current, int maxHealth)
    {
        healthBar.fillAmount = (float)current / (float)maxHealth;

        healthTMPro.text = current.ToString() + " / " + maxHealth.ToString();
    }

    public static void SetSilk(int current, int maxSilk)
    {
        silkBar.fillAmount = (float)current / (float)maxSilk;

        silkTMPro.text = current.ToString() + " / " + maxSilk.ToString();
    }
}

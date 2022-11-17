using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class DiceController : MonoBehaviour
{
    public static int me = 0;//サイコロの目
    System.Random saikoro = new System.Random();
    public static Button dice;
    // Start is called before the first frame update
    void Start()
    {
       
    }
    
    public void OnClick(){
        dice = GameObject.Find("Dice").GetComponent<Button>();
        dice.interactable = false;
        me = saikoro.Next(1,10);
        dice.GetComponentInChildren<TextMeshProUGUI>().text=me.ToString();
        GameController.Pop();
        //SceneManager.LoadScene("problem");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

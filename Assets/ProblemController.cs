using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;


public class ProblemController : MonoBehaviour
{
    string[] problem_list = new string [] {"", "log<sub>2</sub>4=", "1+2=", "sin<sup>2</sup><i>Θ</i>+cos<sup>2</sup><i>Θ</i>=", "<i>a</i><sub>1</sub>=2, <i>a<sub>n</i>+1</sub>=2<i>a<sub>n</sub></i>+1, <i>a</i><sub>3</sub>=</i>", "(2+<i>i</i>)(2-<i>i</i>)=", "2sin(<i>π</i>/4)cos(<i>π</i>/4)=", "log<sub>3</sub>9=", "2<sup>2</sup>=", "-6cos<i>π</i>="};
    string[] ans_list = new string [] {"", "2", "3", "1", "11", "5", "1", "2", "4", "6"};
    public TextMeshProUGUI Problem;
    public TextMeshProUGUI Timer;
    public TMP_InputField Answer;
    bool isTimeUp = false;
    bool solved = false;
    float time = 10f;
    public static bool isWalk;
    

    int me = 1;
    void Start()
    {
        int me = GameController.me;
        Debug.Log(me);
        Problem.text = "Solve me!<br>"+problem_list[me];
        //isTimeUp = false;
        Timer.text = "Timer:"+time.ToString("F1");
    }

    // Update is called once per frame
    void Update()
    {

    }
    

    IEnumerator Erase(float time){
        if (isTimeUp && solved==false)Problem.text = "Time up";
        yield return new WaitForSeconds(time);
        Debug.Log("erased");
    }


    //InputFieldの文字が変更されたらコールバックされる。
    //TMProの、InputFieldである、AnswerWindow、のOn End Editによって、GameMasterの、この関数(InputText)を選択し、コールバックできるようにした
    public void InputText(){
        int me = GameController.me;
        if(Answer.text == ans_list[me] && solved==false){
            solved = true;
            Problem.text += ans_list[me]+"<br>Congraturations!";
            Timer.text = "";
            time = -1;
            SceneManager.LoadScene("SampleScene");
            isWalk = true;
            //GameController.Walk(me);
        }
     }
}

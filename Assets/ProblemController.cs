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
    public static int ans;
    

    
    void Start()
    {
        int me = GameController.me;
        Problem.text = "Solve me!<br>"+problem_list[me];
        //isTimeUp = false;
        Timer.text = "Timer:"+time.ToString("F1");
        solved = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (0 < time) {
            time -= Time.deltaTime;
            Timer.text = "Timer:"+time.ToString("F1");
        }else if (time < 0 && isTimeUp==false && solved==false){
            isTimeUp = true;
            StartCoroutine(Erase(3));//時間切れ
            GameController.players_turn += 1;
            GameController.players_turn %= 1;
        }
    }
    

    IEnumerator Erase(float time){
        if (isTimeUp && solved==false)Problem.text = "Time up";
        yield return new WaitForSeconds(time);
        SceneManager.LoadScene("SampleScene");
        if(solved)isWalk = true;
    }


    //InputFieldの文字が変更されたらコールバックされる。
    //TMProの、InputFieldである、AnswerWindow、のOn End Editによって、GameMasterの、この関数(InputText)を選択し、コールバックできるようにした
    public void InputText(){
        int me = GameController.me;
        if(Answer.text == ans_list[me] && solved==false){
            ans = int.Parse(ans_list[me]);
            solved = true;
            Problem.text += ans_list[me]+"<br>Congraturations!";
            Timer.text = "";
            time =- 1;//タイマーが減らないようにする
            StartCoroutine(Erase(3f));
        }
     }
}

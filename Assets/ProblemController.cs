using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;//ランダム変数用
using UnityEngine.UI;

public class ProblemController : MonoBehaviour
{
    string[] problem_list = new string [] {"", "log<sub>2</sub>4=", "1+2=", "sin<sup>2</sup><i>Θ</i>+cos<sup>2</sup><i>Θ</i>=", "<i>a</i><sub>1</sub>=2, <i>a<sub>n</i>+1</sub>=2<i>a<sub>n</sub></i>+1, <i>a</i><sub>3</sub>=</i>", "(2+<i>i</i>)(2-<i>i</i>)=", "2sin(<i>π</i>/4)cos(<i>π</i>/4)=", "log<sub>3</sub>9=", "2<sup>2</sup>=", "-6cos<i>π</i>="};
    string[] ans_list = new string [] {"", "2", "3", "1", "11", "5", "1", "2", "4", "6"};
    public TextMeshProUGUI Problem;
    public TextMeshProUGUI Timer;
    public TMP_InputField Answer;
    bool isTimeUp = false;
    bool solved = false;
    float time = 1000000000000000000f;
    public static bool isWalk;
    public static int ans;

    public TextMeshProUGUI problem1;
    public TextMeshProUGUI problem2;
    public TextMeshProUGUI problem3;
    public TextMeshProUGUI problem4;
    public TextMeshProUGUI problem5;
    public TextMeshProUGUI problem6;

    public GameObject blackboard;
    System.Random saikoro = new System.Random();
    int one;
    int two;
    int three;
    int four;
    int five;
    int six;
    int last_problem;
    int me = 0;
    public Button dice;
    public AudioSource audioSource;//ProblemControllerObjectに追加したオーディオソースコンポーネント
    public AudioClip taikoSound;
    void Start()
    {
        dice.interactable = false;//サイコロ動作のディレイ
        audioSource.PlayOneShot(taikoSound);
        one = saikoro.Next(1,10);
        two = saikoro.Next(1,10);
        three = saikoro.Next(1,10);
        four = saikoro.Next(1,10);
        five = saikoro.Next(1,10);
        six = saikoro.Next(1,10);
        problem1.text = problem_list[one];
        problem2.text = problem_list[two];
        problem3.text = problem_list[three];
        problem4.text = problem_list[four];
        problem5.text = problem_list[five];
        problem6.text = problem_list[six];

        solved = false;
        StartCoroutine(MoveDice());
    }

    IEnumerator MoveDice(){//サイコロの動き始めを遅らせる
        yield return new WaitForSeconds(1f);
        dice.interactable = true;
    }

    
    // Update is called once per frame
    float currentTime = 0f;
    public Image diceImage;
    public Sprite[] diceImages;
    void Update()
    {
        currentTime += Time.deltaTime;
        if(currentTime>0.1f && dice.interactable ){
            me+=1;
            me%=6;
            currentTime = 0f;
            diceImage.sprite = diceImages[me];
        }
        if (0 < time && time<=10) {//10秒にセットされないと減らない。
            time -= Time.deltaTime;
            Timer.text = "Timer:"+time.ToString("F1");
        }else if (time < 0 && isTimeUp==false && solved==false){
            audioSource.Stop();//時計の音を止める
            audioSource.PlayOneShot(batu);
            isTimeUp = true;
            GameController.players_turn += 1;
            GameController.players_turn %= 3;
            StartCoroutine(Erase(3));//時間切れ
        }
    }
    

    IEnumerator Erase(float time){
        if (isTimeUp && solved==false)Problem.text = "Time up";
        yield return new WaitForSeconds(time);
        blackboard.SetActive(false);
        maru_image.SetActive(false);
        SceneManager.LoadScene("SampleScene");
        if(solved)isWalk = true;
    }


    //InputFieldの文字が変更されたらコールバックされる。
    //TMProの、InputFieldである、AnswerWindow、のOn End Editによって、GameMasterの、この関数(InputText)を選択し、コールバックできるようにした
    public AudioClip maru;
    public AudioClip batu;
    public GameObject maru_image;
    public void InputText(){
        if(Answer.text == ans_list[last_problem] && solved==false){
            audioSource.Stop();//時計の音を止める
            audioSource.PlayOneShot(maru);
            Problem.text += ans_list[last_problem];
            maru_image.SetActive(true);
            ans = int.Parse(ans_list[last_problem]);
            solved = true;
            Timer.text = "";
            time =- 1;//タイマーが減らないようにする
            StartCoroutine(Erase(3f));
        }else if (isTimeUp==false && solved==false){
            audioSource.PlayOneShot(batu);
        }
    }

    public void Dice(){
        dice.interactable = false;
        int [] selected_problems = {one, two, three, four, five, six};
        last_problem = selected_problems[me];
        StartCoroutine(Show());
    }
    public AudioClip syutsudai;
    public AudioClip tokeiSound;
    public IEnumerator Show(){
        yield return new WaitForSeconds(3f);
        blackboard.SetActive(true);
        Problem.text = "Solve me!<br>"+problem_list[last_problem];
        audioSource.PlayOneShot(syutsudai);
        yield return new WaitForSeconds(0.5f);
        time = 10f;
        audioSource.PlayOneShot(tokeiSound);
    }
    
}

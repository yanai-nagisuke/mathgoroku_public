using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;//ランダム変数用
using TMPro;
using UnityEngine.Tilemaps;//マス目を記録したタイルマップ
using System.Text;//String Builderを使うためのもの
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public GameObject ProblemPannel;
    public TextMeshProUGUI Problem;
    public TMP_InputField Answer;
    public Tilemap tilemap;//地図のタイルマップを取得。地図のタイルマップとワールド座標は異なるためGetCellCentorWordlでタイルマップの中心の位置に変換する必要がある。
    public GameObject player1;
    public GameObject player2;
    public GameObject player3;
    public Button dice;
    List<GameObject> players = new List<GameObject>();
    int me = 0;//サイコロの目
    int[,] players_position = new int[,] {{-8,-3}, {-8,-3}, {-8,-3}};//それぞれのプレイヤーのいるマス目の座標
    int players_turn = 0;//今誰のターンか
    System.Random saikoro = new System.Random();
    string[] problem_list = new string [] {"", "log<sub>2</sub>4=", "1+2=", "sin<sup>2</sup><i>Θ</i>+cos<sup>2</sup><i>Θ</i>=", "<i>a</i><sub>1</sub>=2, <i>a<sub>n</i>+1</sub>=2<i>a<sub>n</sub></i>+1, <i>a</i><sub>3</sub>=</i>", "(2+<i>i</i>)(2-<i>i</i>)=", "2sin(<i>π</i>/4)cos(<i>π</i>/4)=", "log<sub>3</sub>9=", "2<sup>2</sup>=", "-6cos<i>π</i>="};
    string[] ans_list = new string [] {"", "2", "3", "1", "11", "5", "1", "2", "4", "6"};
    float speed = 0.5f;
    List<Vector3> player_destination = new List<Vector3>();
    public TextMeshProUGUI pointer_button_text;
    public TextMeshProUGUI Timer;

    int[,,] used = new int[3, 16, 8];//プレイヤー数、縦、横
    int pointer = 0;

    float time = 5f;
    bool isTimeUp = false;
    bool solved = false;


    void Start()
    {
        StartCoroutine(Erase(0));
        player_destination.Add(tilemap.GetCellCenterWorld(new Vector3Int(-8, -3, 0)));//各プレイヤーの向かう先を記録している。
        player_destination.Add(tilemap.GetCellCenterWorld(new Vector3Int(-8, -3, 0)));//幅優先探索で更新していく
        player_destination.Add(tilemap.GetCellCenterWorld(new Vector3Int(-8, -3, 0)));
        players.Add(player1);//プレイヤーのゲームオブジェクトを配列として保持している
        players.Add(player2);
        players.Add(player3);
        used[0, 0, 1] = 1;//xを+8, yを+4した値にする
        used[1, 0, 1] = 1;//幅優先探索ように訪れた頂点を初期化している
        used[2, 0, 1] = 1;

        var builder = new StringBuilder();//タイルマップ表示用プログラム
        var bound = tilemap.cellBounds;
        for (int y = bound.max.y - 1; y >= bound.min.y; --y)
        {
            for (int x = bound.min.x; x < bound.max.x; ++x)
            {
                builder.Append(tilemap.HasTile(new Vector3Int(x, y, 0)) ? "■" : "□");
            }
            builder.Append("\n");
        }
        //Debug.Log(builder.ToString());
    }

    
    private IEnumerator Change(int x, int y, int nokori, float waitTime){
        yield return new WaitForSeconds(waitTime);
        player_destination[players_turn] = tilemap.GetCellCenterWorld(new Vector3Int(x, y, 0));//タイル換算の位置にしている
        if(nokori==0){
            yield return new WaitForSeconds(waitTime);//目的地を変えてから直ぐにターン変更すると次のプレイヤーが動いてしまう
            players_turn += 1;
            players_turn %= 3;
            while(ProblemPannel.activeSelf==true)yield return null;//黒板が消えてからサイコロを降らないと、新しい黒板が代わりに消える。
            dice.interactable = true;
        }
    }


    void Update(){
        players[players_turn].transform.position = Vector3.MoveTowards(players[players_turn].transform.position,  player_destination[players_turn], speed);
        if (0 < time) {
            time -= Time.deltaTime;
            Timer.text = "Timer:"+time.ToString("F1");
        }else if (time < 0 && isTimeUp==false && solved==false){
            isTimeUp = true;
            StartCoroutine(Erase(3));//時間切れ
            StartCoroutine(Change(players_position[players_turn, 0], players_position[players_turn, 1], 0, 1.5f));//ターンを進める処理はChangeに任せている。
        }
    }


    public void change_pointer(){
        pointer = pointer^1;
        if(pointer==0)pointer_button_text.text = "Right";
        else pointer_button_text.text = "Left";
    }


    IEnumerator WaitInput (int me) {
        while(!Input.GetKey("a")) yield return null;
        Walk(me, 1);//無限ループ防止用フラグ
    }


    private void Walk(int me, int flg=0){
        if(flg==0)StartCoroutine(Erase(3));
        int[,] delta = new int[,] {{1,0}, {0,1}, {-1,0}, {0,-1}};
        for(int i=0; i<me; i++){
            List<List<int>> Nexts = new List<List<int>>();
            for(int j=0; j<4; j++){
                List<int> next = new List<int>();
                int nx_kouho = players_position[players_turn, 0] + delta[j, 0];
                int ny_kouho = players_position[players_turn, 1] + delta[j, 1];
                if (!tilemap.HasTile(new Vector3Int(nx_kouho, ny_kouho, 0)))continue;
                if (used[players_turn, nx_kouho+8, ny_kouho+4] >= 1)continue;
                next.Add(nx_kouho);
                next.Add(ny_kouho);
                Nexts.Add(next);
            } 
            int nx, ny;
            if((players_position[players_turn, 0]==-8&&players_position[players_turn, 1]==0) || (players_position[players_turn, 0]==1&&players_position[players_turn, 1]==-4)){
                if(flg==0){
                    StartCoroutine(WaitInput(me-i));
                return;
                }else{
                    nx = Nexts[pointer][0];
                    ny = Nexts[pointer][1];
                }
            }else{
                nx = Nexts[0][0];
                ny = Nexts[0][1];
            }
            
            players_position[players_turn, 0] = nx;
            players_position[players_turn, 1] = ny;
            used[players_turn, nx+8, ny+4] = 1;
            StartCoroutine(Change(nx, ny, me-i-1, 0.3f*i));
            player_destination[players_turn] = tilemap.GetCellCenterWorld(new Vector3Int(nx, ny, 0));//タイル換算の位置にしている
        }
    }

    
    public void Dice(){
        dice.interactable = false;
        me = saikoro.Next(1,10);
        dice.GetComponentInChildren<TextMeshProUGUI>().text=me.ToString();
        Debug.Log("サイコロ:"+me.ToString());
        Pop();
    }
    


    //InputFieldの文字が変更されたらコールバックされる。
    //TMProの、InputFieldである、AnswerWindow、のOn End Editによって、GameMasterの、この関数(InputText)を選択し、コールバックできるようにした
    public void InputText(){
        if(Answer.text == ans_list[me] && solved==false){
            solved = true;
            Problem.text += ans_list[me]+"<br>Congraturations!";
            Timer.text = "";
            time = -1;
            Walk(me);
        }
     }
    

    IEnumerator Erase(float time){
        if (isTimeUp && solved==false)Problem.text = "Time up";
        yield return new WaitForSeconds(time);
        ProblemPannel.SetActive(false);//黒板をけす
        Debug.Log("erased");
    }


    void Pop(){
        solved = false;
        ProblemPannel.SetActive(true);//黒板を出す
        Problem.text = "Solve me!<br>"+problem_list[me];
        time = 10f;
        isTimeUp = false;
        Timer.text = "Timer:"+time.ToString("F1");
    }
}

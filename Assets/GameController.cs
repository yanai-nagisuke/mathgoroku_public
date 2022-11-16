using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;//ランダム変数用
using TMPro;
using UnityEngine.Tilemaps;//マス目を記録したタイルマップ
using System.Text;//String Builderを使うためのもの
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{   
    public Tilemap tilemap;//地図のタイルマップを取得。地図のタイルマップとワールド座標は異なるためGetCellCentorWordlでタイルマップの中心の位置に変換する必要がある。
    GameObject player1;
    GameObject player2;
    GameObject player3;
    public Button dice;
    List<GameObject> players = new List<GameObject>();


    public static int me = 0;//サイコロの目
    static int[,] players_position; 
    static int players_turn = 0;//今誰のターンか
    static int[,,] used;
    static List<Vector3> player_destination = new List<Vector3>();

    System.Random saikoro = new System.Random();
    float speed = 0.5f;
    public TextMeshProUGUI pointer_button_text;

    int pointer = 0;

    static bool syokika = true;
    void Awake(){
        Debug.Log(syokika);
        player1 = GameObject.Find("fox");
        player2 = GameObject.Find("fox_red");
        player3 = GameObject.Find("fox_yellow");
        players.Add(player1);//プレイヤーのゲームオブジェクトを配列として保持している。
        players.Add(player2);//ゲームオブジェクトは毎回作成し、目的地と現在地だけ記録しておく
        players.Add(player3);
        if (syokika){
            players_position = new int[,]{{-8,-3}, {-8,-3}, {-8,-3}};//それぞれのプレイヤーのいるマス目の座標。
            player_destination.Add(tilemap.GetCellCenterWorld(new Vector3Int(-8, -3, 0)));
            player_destination.Add(tilemap.GetCellCenterWorld(new Vector3Int(-8, -3, 0)));
            player_destination.Add(tilemap.GetCellCenterWorld(new Vector3Int(-8, -3, 0)));
            used = new int[3, 16, 8];//プレイヤー数、縦、横
            used[0, 0, 1] = 1;//xを+8, yを+4した値にする
            used[1, 0, 1] = 1;//幅優先探索ように訪れた頂点を初期化している
            used[2, 0, 1] = 1;
            syokika = false;
        }else{
            player1.transform.position = player_destination[0];//プレイヤーをワープさせる。
            player2.transform.position = player_destination[1];
            player3.transform.position = player_destination[2];
        }
    }


    void Start(){
        if(ProblemController.isWalk)Walk(me);
        ProblemController.isWalk = false;
    }

    
    private IEnumerator Change(int x, int y, int nokori, float waitTime){
        yield return new WaitForSeconds(waitTime);
        player_destination[players_turn] = tilemap.GetCellCenterWorld(new Vector3Int(x, y, 0));//タイル換算の位置にしている
        if(nokori==0){
            yield return new WaitForSeconds(waitTime);//目的地を変えてから直ぐにターン変更すると次のプレイヤーが動いてしまう
            players_turn += 1;
            players_turn %= 3;
            dice.interactable = true;
        }
    }


    void Update(){
        players[players_turn].transform.position = Vector3.MoveTowards(players[players_turn].transform.position,  player_destination[players_turn], speed);
        if (Input.GetKey("b")){
            Debug.Log("b");
            player_destination[0] = tilemap.GetCellCenterWorld(new Vector3Int(0, 0, 0));
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
        Pop();
    }


    void Pop(){
        SceneManager.LoadScene("problem");
    }
}

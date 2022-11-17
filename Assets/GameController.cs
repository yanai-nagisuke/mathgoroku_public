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
    public static int players_turn = 0;//今誰のターンか
    static int[,,] used;
    static List<Vector3> player_destination = new List<Vector3>();

    System.Random saikoro = new System.Random();

   

    static bool syokika = true;
    
    void Start(){
        player1 = GameObject.Find("fox");
        player2 = GameObject.Find("fox_red");
        player3 = GameObject.Find("fox_yellow");
        players = new List<GameObject>() {player1, player2, player3};//プレイヤーのゲームオブジェクトを配列として保持している。プレイヤーのゲームオブジェクトを配列として保持している。
        var bound = tilemap.cellBounds;
        if (syokika){
            int sx = -5;//スタート地点の座標。
            int sy = -1;
            players_position = new int[,]{{sx,sy}, {sx,sy}, {sx,sy}};//それぞれのプレイヤーのいるマス目の座標。
            player_destination = new List<Vector3>() {tilemap.GetCellCenterWorld(new Vector3Int(sx, sy, 0)), tilemap.GetCellCenterWorld(new Vector3Int(sx, sy, 0)), tilemap.GetCellCenterWorld(new Vector3Int(sx, sy, 0))};
            
            used = new int[3, bound.max.x-bound.min.x, bound.max.y-bound.min.y];//プレイヤー数、縦、横
            used[0, sx-bound.min.x, sy-bound.min.y] = 1;//xを+8, yを+4した値にする
            used[1, sx-bound.min.x, sy-bound.min.y] = 1;//幅優先探索ように訪れた頂点を初期化している
            used[2, sx-bound.min.x, sy-bound.min.y] = 1;
            syokika = false;
        }else{
            Vector3 delta = new Vector3(0,0.5f,0);
            player1.transform.position = delta + player_destination[0];//プレイヤーをワープさせる。
            player2.transform.position = delta + player_destination[1];
            player3.transform.position = delta+player_destination[2];
        }
       

        if(ProblemController.isWalk){
            dice.interactable = false;
            Walk(me);
        }
        ProblemController.isWalk = false;
    }


    
    private IEnumerator Change(int x, int y, int nokori, float waitTime){
        yield return new WaitForSeconds(waitTime);
        player_destination[players_turn] = tilemap.GetCellCenterWorld(new Vector3Int(x, y, 0));
       
        if(nokori==0){
            yield return new WaitForSeconds(waitTime);//目的地を変えてから直ぐにターン変更すると次のプレイヤーが動いてしまう
            players_turn += 1;
            players_turn %= 3;
            dice.interactable = true;
        }
    }
    
    float speed = 0.5f;
  
    void Update(){
        Vector3 delta = new Vector3(0,0.5f,0);//パネルの上に立ってるように見える補正
        Vector3 dist = player_destination[players_turn] + delta;
        players[players_turn].transform.position = Vector3.MoveTowards(players[players_turn].transform.position,  dist, speed);//player_destination[players_turn], speed);

        /*if (Input.GetMouseButtonDown(0)){
                Vector3 pos = Input.mousePosition;   
                Debug.Log(tilemap.WorldToCell(Camera.main.ScreenToWorldPoint(pos)));
        }*/
    }



    public TileBase m_tileGray;
    public TileBase m_tileRed;
    IEnumerator WaitInput (int me, List<List<int>> Nexts) {
        int nexts_index = 0;
        Vector3Int before;
        Vector3Int selectCellPos = new Vector3Int(Nexts[0][0],Nexts[0][1],0);
        tilemap.SetTile(selectCellPos,m_tileGray);
        before = selectCellPos;
        bool canMove=false;
        while(!canMove) {
            if (Input.GetMouseButtonDown(0)){
                Vector3 pos = Input.mousePosition;   
                selectCellPos = tilemap.WorldToCell(Camera.main.ScreenToWorldPoint(pos));
                for(int i=0; i<Nexts.Count; i++){
                    if(selectCellPos.x == Nexts[i][0] && selectCellPos.y == Nexts[i][1]){
                        if(before==selectCellPos)canMove=true;
                        nexts_index = i;
                        tilemap.SetTile(selectCellPos,m_tileGray);
                        tilemap.SetTile(before,m_tileRed);
                        before = selectCellPos;
                    }
                }
            }
            yield return null;
        }
        tilemap.SetTile(selectCellPos,m_tileRed);
        Walk(me, 1, nexts_index);//無限ループ防止用フラグ
    }


    private void Walk(int me, int flg=0, int nexts_index=0){
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
            int[,] bunki = {{-2, -1}};
            bool isBunki = false;
           
            for(int j=0; j<bunki.GetLength(0); j++){
                if((players_position[players_turn, 0]==bunki[j,0]&&players_position[players_turn, 1]==bunki[j,1]))isBunki = true;
            }

            if(isBunki){
                if(flg==0){
                    StartCoroutine(WaitInput(me-i, Nexts));
                return;
                }else{
                    nx = Nexts[nexts_index][0];
                    ny = Nexts[nexts_index][1];
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


    public static void Pop(){
        SceneManager.LoadScene("problem");
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class CameraControl2 : MonoBehaviour
{
    //カメラの移動量
    //[SerializeField, Range(0.1f, 10.0f)]
    private float _positionStep = 30.0f;

 
    //カメラのtransform  
    public static Transform _camTransform;
   
    public Camera camera_;        
    void Start ()
    {
        _camTransform = this.gameObject.transform;

    }
    public static Vector3 campos;
    public static void MoveCamera () {    
        if(GameController.players_turn==0) campos = GameController.player1.transform.position;
        if(GameController.players_turn==1) campos = GameController.player2.transform.position;
        if(GameController.players_turn==2) campos = GameController.player3.transform.position;
        campos.z = -10;
        _camTransform.position =campos;
    }

    

    void Update () {
        campos = _camTransform.position;

        if (Input.GetKey(KeyCode.RightArrow)) { campos += _camTransform.right * Time.deltaTime * _positionStep; }
        if (Input.GetKey(KeyCode.LeftArrow)) { campos -= _camTransform.right * Time.deltaTime * _positionStep; }
        if (Input.GetKey(KeyCode.UpArrow)) { campos += _camTransform.up * Time.deltaTime * _positionStep; }
        if (Input.GetKey(KeyCode.DownArrow)) { campos -= _camTransform.up * Time.deltaTime * _positionStep; }
        if (Input.GetKey(KeyCode.U)) {camera_.orthographicSize -= 0.01f;}
        if (Input.GetKey(KeyCode.L)) {camera_.orthographicSize += 0.01f;}

        if(!(Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.UpArrow) ||
            Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.U) || Input.GetKey(KeyCode.L))){
            if(GameController.players_turn==0) campos += (GameController.player1.transform.position-campos)*0.05f;
            if(GameController.players_turn==1) campos += (GameController.player2.transform.position-campos)*0.05f;
            if(GameController.players_turn==2) campos += (GameController.player3.transform.position-campos)*0.05f;
            campos.z = -10;
            }
        _camTransform.position = campos;
    }
    




  
}
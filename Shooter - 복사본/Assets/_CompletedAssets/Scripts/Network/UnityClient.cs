using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using UnitySampleAssets.CrossPlatformInput;
using Dll;
using JsonFx;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace CompleteProject
{
	public class UnityClient : MonoBehaviour
	{
		//player object
		public GameObject player;
		public GameObject gunbarrelend;
		
		public GameObject player1;
		public GameObject gunbarrelend1;
		
		//상대 player form 
		Transform pr1;
		Transform gb1;
		
		//player들 생성 위치
		public Transform[] SpawnPoint;
		
		//자신의 Move, Shooting 정보
		PlayerMovement playermv;
		PlayerShooting playerst;
		
		//상대 player의 Move, Shooting 정보
		PlayerMovement1 playermv1;
		PlayerShooting1 playerst1;
		
		//player 정보에 대한 protocol
		gameProtocol myPlayer;
		gameProtocol enemyPlayer;
		
		//연결, 수신, 게임 준비 여부, 초기 생성 위치
		bool connecting = false;
		bool getInfo = false;
		bool gameReady = false;
		float spIndex;
		
		//client socket 생성
		Socket clientSocket = new Socket (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		
		/*
		void Awake ()
		{
		
		}
		*/
		
		void Start ()
		{
			//game server와 connect
			clientSocket.Connect ("127.0.0.1", 9001);
			
			//Protocol 생성
			myPlayer = new gameProtocol ();
			enemyPlayer = new gameProtocol ();
			
			//Game 방 접속 알리기
			byte[] firstStream = SendGameReady ();
			clientSocket.Send (firstStream);
			
			// 
			Thread ctThread = new Thread(GetPlayerInfo);
			ctThread.Start();
			
		}
		
		// Update is called once per frame
		void Update ()
		{
			//game ready 정보 server로 전송
			if (connecting == true && gameReady == true) 
			{
				setPlayers (spIndex);
				gameReady = false;
			}
			
			//실시간 자신의 player 정보 server로 전송
			if(connecting == true)
				SendPlayerInfo ();
			
			//상대방 player 정보 server로부터 수신
			if (getInfo == true) 
			{
				UpdateEnemyInfo ();
				getInfo = false;
			}
		}
		
		//상대 player 정보 수신
		void GetPlayerInfo()
		{
			while (true) 
			{
				int buffSize = 0;
				byte[] inStream = new byte[LENGTH.MAX_PACKET_LEN];
				buffSize = clientSocket.ReceiveBufferSize;
				clientSocket.Receive (inStream);
				enemyPlayer = ByteToPlayer (inStream);
				
				if (enemyPlayer.task == "GameStart") 
				{
					connecting = true;
					gameReady = true;
					spIndex = enemyPlayer.spawnIndex;
				} 
				else if (connecting == true && enemyPlayer.task == "Playing")
					getInfo = true;
			}	
		}
		
		//자신의 player 정보 송신
		void SendPlayerInfo()
		{
			byte[] outStream = PlayerInfoToByte ();
			clientSocket.Send (outStream);
		}
		
		//game ready 송신
		byte [] SendGameReady()
		{
			//myPlayer = new gameProtocol ();
			myPlayer.task = "PlayerEnter";
			string outPut = JsonFx.Json.JsonWriter.Serialize (myPlayer);
			byte [] outStream = System.Text.Encoding.UTF8.GetBytes (outPut);
			return outStream;
		}
		
		//protocol을 byte[]로 변환
		byte [] PlayerInfoToByte()
		{
			//myPlayer = new gameProtocol ();
			myPlayer.task = "Playing";
			myPlayer.h = playermv.h;
			myPlayer.v = playermv.v;
			myPlayer.x = playermv.playerToMouse.x;
			myPlayer.y = playermv.playerToMouse.y;
			myPlayer.z = playermv.playerToMouse.z;
			myPlayer.fire1 = playerst.f1;
			
			string outPut = JsonFx.Json.JsonWriter.Serialize(myPlayer);
			byte [] outStream = System.Text.Encoding.UTF8.GetBytes (outPut);
			
			return outStream;
			
		}
		
		//byte[]를 gameprotocol로 변환
		gameProtocol ByteToPlayer(byte [] inStream)
		{
			string inPut = System.Text.Encoding.UTF8.GetString (inStream);
			enemyPlayer = JsonFx.Json.JsonReader.Deserialize <gameProtocol> (inPut);
			return enemyPlayer;
		}
		
		//게임 속에 player들 생성
		void setPlayers(float spawnIndex)
		{
			if (spawnIndex == 1)
			{
				Instantiate (player, SpawnPoint[0].position, SpawnPoint[0].rotation);
				Instantiate (player1, SpawnPoint[1].position, SpawnPoint[1].rotation);
			}
			
			else if (spawnIndex == 0) 
			{
				Instantiate (player, SpawnPoint[1].position, SpawnPoint[1].rotation);
				Instantiate (player1, SpawnPoint[0].position, SpawnPoint[0].rotation);
			}
			
			pr1 = GameObject.FindGameObjectWithTag ("Player1").transform;
			gb1 = GameObject.FindGameObjectWithTag ("GunBarrelEnd1").transform;
			playermv1 = pr1.GetComponent <PlayerMovement1> ();
			playerst1 = gb1.GetComponent <PlayerShooting1> ();
			
		}
		
		//상대 player의 정보 update
		private void UpdateEnemyInfo()
		{
			//input axes.
			float h = enemyPlayer.h;
			float v = enemyPlayer.v;
			//mouse cursor.
			Vector3 enemyMousePosition;
			enemyMousePosition.x = enemyPlayer.x;
			enemyMousePosition.y = enemyPlayer.y;
			enemyMousePosition.z = enemyPlayer.z;
			//mouse click
			bool fire1 = enemyPlayer.fire1;
			
			//update
			playermv1.Move (h, v);
			playermv1.Turning (enemyMousePosition);
			playermv1.Animating (h, v);
			playerst1.fire1 = fire1;
		}
		
	}
}
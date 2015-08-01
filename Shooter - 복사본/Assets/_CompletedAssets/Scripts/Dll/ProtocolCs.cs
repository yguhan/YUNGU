using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace Dll
{
	static class LENGTH
	{
		public const int MAX_USERID_LEN = 50;
		public const int MAX_PASSWD_LEN = 32;
		public const int MAX_CHATMESSAGE_LEN = 512;
		public const int MAX_PACKET_LEN = 8192;
	}
	
	static class LOGIN_RESULT
	{
		public const int LOGIN_DEFAULT = 0;
		public const int LOGIN = 1;
		public const int SIGNUP = 2;
		
		public const int LOGIN_SUCCESS = 1;
		public const int LOGIN_FAIL_GENERAL = 0;
		public const int LOGIN_FAIL_NO_ID = -1;
		public const int LOGIN_FAIL_PASSWORD = 2;
		
		public const int SIGNUP_SUCCESS = 1;
		public const int SIGNUP_FAIL_ID_EXISTS = -1;
	}
	
	static class CHAT_TARGET
	{
		public const int CHAT_DEFAULT = 0;
		public const int CHAT_ALL = 1;
		public const int CHAT_WHISPER = 2;
		public const int CHAT_GAMEROOM = 3;
	}
	
	static class LOBBY_STATUS
	{
		public const int LOBBY_DEFAULT = 0;
		public const int LOBBY_IN = 1;
		public const int ROOM_IN = 2;
		public const int GAME_IN = 3;
		
		public const int LOBBY_OUT = -1;
	}
	
	
	public class UserInfo
	{
		public int task;
		public string id;
		public string passwd;
		
		public UserInfo(int task, string id, string passwd)
		{
			this.task = task;
			this.id = id;
			this.passwd = passwd;
		}
	}
	
	
	public class LoginReturn
	{
		public int task;
		public int result;
		
		public LoginReturn(int task, int result)
		{
			this.task = task;
			this.result = result;
		}
	}
	
	public class ChatClientInformation
	{
		public int chat_target;
		public string user_id;
		public string target_id;
		public List<string> roomUserList;
		
		public ChatClientInformation()
		{
			this.chat_target = CHAT_TARGET.CHAT_DEFAULT;
			this.user_id = "A";
			this.target_id = "A";
			this.roomUserList = new List<string>();
		}
	}
	
	public class LobbyClientInformation
	{
		public int status;
		public string user_id;
		public List<User> lobbyList;
		
		public LobbyClientInformation()
		{
			this.status = LOBBY_STATUS.LOBBY_DEFAULT;
			this.user_id = "A";
			this.lobbyList = new List<User>();
		}
	}
	
	public class ChatProtocol
	{
		public int chat_target;
		public string sender_id;
		public List<string> targetUserList;
		public string message;
		
		public ChatProtocol()
		{
			this.chat_target = CHAT_TARGET.CHAT_DEFAULT;
			this.sender_id = "A";
			this.message = "A";
			this.targetUserList = new List<string>();
		}
	}
	
	public class User
	{
		public int status;
		public string user_id;
		
		public User()
		{
			this.status = LOBBY_STATUS.LOBBY_DEFAULT;
			this.user_id = "A";
		}
		
		public User(int status, string user_id)
		{
			this.status = status;
			this.user_id = user_id;
		}
	}
	
	public class UserWithSocket
	{
		public User user;
		public TcpClient clientSocket;
		
		public UserWithSocket(User user, TcpClient clientSocket)
		{
			this.user = user;
			this.clientSocket = clientSocket;
		}
	}
}

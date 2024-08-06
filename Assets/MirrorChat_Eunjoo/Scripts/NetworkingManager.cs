using UnityEngine;
using Mirror;

public class NetworkingManager : NetworkManager
{
    [SerializeField] LoginPopup _loginPopup;
    [SerializeField] ChattingUI _chattingUI;

    // 사용자가 입력한 호스트 이름을 네트워크 주소에 설정하는 메소드
    public void OnInputValueChanged_SetHostName(string hostName)
    {
        // NetworkManager의 networkAddress에 호스트 정보 세팅
        this.networkAddress = hostName;
    }

    // 서버에서 클라이언트 연결이 끊어졌을 때 호출되는 메소드
    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        // 채팅 UI가 존재할 경우 연결이 끊어진 클라이언트 이름을 제거
        if (_chattingUI != null)
        {
            _chattingUI.RemoveNameOnServerDisonnect(conn);
        }

        // 부모 클래스의 OnServerDisconnect 메소드 호출
        base.OnServerDisconnect(conn);
    }


    // 클라이언트에서 연결이 끊어졌을 때 호출되는 메소드
    // NetworkManager에 로그인 팝업 참조변수 및 클라 연결 해제 시 관련 UI 처리를 해주도록 호출
    public override void OnClientDisconnect()
    {
        // 부모 클래스의 OnClientDisconnect 메소드 호출
        base.OnClientDisconnect();

        // 로그인 팝업이 존재할 경우 UI 초기화 메소드 호출
        if (_loginPopup != null)
        {
            _loginPopup.SetUIOnClientDisconnected();
        }
    }

}
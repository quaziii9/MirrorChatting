using UnityEngine;
using Mirror;

public class NetworkingManager : NetworkManager
{
    [SerializeField] LoginPopup _loginPopup;
    [SerializeField] ChattingUI _chattingUI;

    // ����ڰ� �Է��� ȣ��Ʈ �̸��� ��Ʈ��ũ �ּҿ� �����ϴ� �޼ҵ�
    public void OnInputValueChanged_SetHostName(string hostName)
    {
        // NetworkManager�� networkAddress�� ȣ��Ʈ ���� ����
        this.networkAddress = hostName;
    }

    // �������� Ŭ���̾�Ʈ ������ �������� �� ȣ��Ǵ� �޼ҵ�
    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        // ä�� UI�� ������ ��� ������ ������ Ŭ���̾�Ʈ �̸��� ����
        if (_chattingUI != null)
        {
            _chattingUI.RemoveNameOnServerDisonnect(conn);
        }

        // �θ� Ŭ������ OnServerDisconnect �޼ҵ� ȣ��
        base.OnServerDisconnect(conn);
    }


    // Ŭ���̾�Ʈ���� ������ �������� �� ȣ��Ǵ� �޼ҵ�
    // NetworkManager�� �α��� �˾� �������� �� Ŭ�� ���� ���� �� ���� UI ó���� ���ֵ��� ȣ��
    public override void OnClientDisconnect()
    {
        // �θ� Ŭ������ OnClientDisconnect �޼ҵ� ȣ��
        base.OnClientDisconnect();

        // �α��� �˾��� ������ ��� UI �ʱ�ȭ �޼ҵ� ȣ��
        if (_loginPopup != null)
        {
            _loginPopup.SetUIOnClientDisconnected();
        }
    }

}
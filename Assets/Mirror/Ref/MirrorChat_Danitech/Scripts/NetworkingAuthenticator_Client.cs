using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public partial class NetworkingAuthenticator
{
    [SerializeField] LoginPopup _loginPopup;

    [Header("Client Username")]
    public string _playerName;

    public void SetPlayerName(string username)
    {
        _playerName = username;
        // UI�� ������Ʈ
        _loginPopup.SetUIOnAuthValueChanged();
    }

    // Ŭ���̾�Ʈ�� ���۵� �� ���� ���� �޽��� �ڵ鷯�� ���
    public override void OnStartClient()
    {
        NetworkClient.RegisterHandler<AuthResMsg>(OnAuthResponseMessage, false);
    }

    // Ŭ���̾�Ʈ�� ����� �� ���� ���� �޽��� �ڵ鷯�� ��� ����
    public override void OnStopClient()
    {
        NetworkClient.UnregisterHandler<AuthResMsg>();
    }

    // Ŭ�󿡼� ���� ��û �� �ҷ���
    public override void OnClientAuthenticate()
    {
        // AuthReqMsg �޽����� �����Ͽ� authUserName �ʵ忡 _playerName ���� ����,
        // �̸� ������ �����մϴ�.
        NetworkClient.Send(new AuthReqMsg { authUserName = _playerName });
    }

    public void OnAuthResponseMessage(AuthResMsg msg)
    {
        if(msg.code ==100) // ����
        {
            Debug.Log($"Auto Response:{msg.code} {msg.message}");
            ClientAccept(); // Ŭ�� ���� �Ϸ�
        }
        else
        {
            Debug.LogError($"Auth Response : {msg.code} {msg.message}");
            NetworkManager.singleton.StopHost();

            _loginPopup.SetUIOnAuthError(msg.message);
        }
    }
}

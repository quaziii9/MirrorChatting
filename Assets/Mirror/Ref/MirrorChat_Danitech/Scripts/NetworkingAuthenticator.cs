using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

// conn(connection) : Ŭ���̾�Ʈ�� ���� ���� ��Ʈ��ũ ������ ��Ÿ���� ��ü

public partial class NetworkingAuthenticator : NetworkAuthenticator
{
    // _connectionPendingDisconnect : ������ ���� ������ Ŭ���̾�Ʈ ���
    // _playerNames : ������ ����� �̸� ���
    readonly HashSet<NetworkConnection> _connectionPendingDisconnect = new HashSet<NetworkConnection>();
    internal static readonly HashSet<string> _playerNames = new HashSet<string>();


    // AuthReqMsg : Ŭ���̾�Ʈ�� ���� ���� ��û �޽���, ����� �̸��� ����
    public struct AuthReqMsg : NetworkMessage
    {
        // ������ ���� ���
        // OAuth ������ ���� �� �κп� ������ ��ū ���� ������ �߰��ϸ� ��
        public string authUserName;
    }

    // AuthResMsg : ������ ���� ���� ���� �޽���, ���� �ڵ�� �޽����� ����
    public struct AuthResMsg : NetworkMessage
    {
        public byte code;
        public string message;
    }

    #region ServerSide
    [UnityEngine.RuntimeInitializeOnLoadMethod]

    static void ResetStatics()
    {

    }


    // ������ ���۵ɶ� ȣ��, AuthReqMsg �޽����� ó���� �ڵ鷯�� ���
    // �ڵ鷯 : Ư�� �̺�Ʈ�� �۾��� �߻������� �̸� ó���ϴ� �ڵ�
    public override void OnStartServer()
    {
        NetworkServer.RegisterHandler<AuthReqMsg>(OnAuthRequestMessage, false);
    }

    // ������ ���⶧ ȣ�� AuthResMsg �޽��� �ڵ鷯�� ��� ����
    public override void OnStopServer()
    {
        NetworkServer.UnregisterHandler<AuthResMsg>();
    }

    public override void OnServerAuthenticate(NetworkConnectionToClient conn)
    {
    }


    // Ŭ���̾�Ʈ�� ���� ��û �޽����� ������ ȣ��
    public void OnAuthRequestMessage(NetworkConnectionToClient conn, AuthReqMsg msg)
    {
        // Ŭ�� ���� ��û �޼��� ���� �� ó��
        Debug.Log($"���� ��û : {msg.authUserName}");

        // �̹̿��� ��� ���� Ŭ���̾�Ʈ�� ����
        if (_connectionPendingDisconnect.Contains(conn)) return;

        // ������ , DB, Playerfab API ���� ȣ���� ���� Ȯ��
        // ���ο� ����� �̸��� ��� ��Ͽ� �߰�, ���� �޽����� ���� �� ������ ����
        if(!_playerNames.Contains(msg.authUserName))
        {
            _playerNames.Add(msg.authUserName);

            // ������ ���� ���� Player.OnStartServer �������� ����
            conn.authenticationData = msg.authUserName;

            AuthResMsg authResMsg = new AuthResMsg
            {
                code = 100,
                message = "Auth Success"
            };

            conn.Send(authResMsg);
            ServerAccept(conn);
        }

        // �̹� ������� �̸��� ��� ���� �޽����� ������, ������ ��� ��Ͽ� �߰�
        else
        {
            _connectionPendingDisconnect.Add(conn);

            AuthResMsg authResMsg = new AuthResMsg
            {
                code = 200,
                message = "User Name already in use! Try again!"
            };

            conn.Send(authResMsg);
            conn.isAuthenticated = false;

            StartCoroutine(DelayedDisconnect(conn, 1.0f));
        }
    }
   
    IEnumerator DelayedDisconnect(NetworkConnectionToClient conn, float waitTime)
    {
        // �����ð��� ������ Ŭ���̾�Ʈ ������ �ź�
        yield return new WaitForSeconds(waitTime);
        ServerReject(conn);

        // ��� ��Ͽ��� �ش� Ŭ���̾�Ʈ ����
        yield return null;
        _connectionPendingDisconnect.Remove(conn);
    }
    #endregion
}

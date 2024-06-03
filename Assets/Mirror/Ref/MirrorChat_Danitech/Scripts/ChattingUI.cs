using UnityEngine;
using UnityEngine.UI;
using Mirror;
using Mirror.Examples.Chat;
using System.Collections.Generic;

public class ChattingUI : NetworkBehaviour
{
    [Header("UI")]
    [SerializeField] Text Text_ChatHistory;
    [SerializeField] Scrollbar ScrollBar_Chat;
    [SerializeField] InputField Input_ChatMsg;
    [SerializeField] Button Btn_Send;

    // ���� �¸� - ����� �÷��̾�� �̸�
    internal static readonly Dictionary<NetworkConnectionToClient, string> _connectedNameDic = new Dictionary<NetworkConnectionToClient, string>();
    public override void OnStartServer()
    {
        _connectedNameDic.Clear();
    }

    public override void OnStartClient()
    {
        Text_ChatHistory.text = string.Empty;
    }

    // [Command] ��� ��Ʈ����Ʈ�� �̿��� Ŭ�� -> ������ Ư�� ��� ������ ��û
    // ������ ���� CommandSendMsg��� �Լ��� ���� ������ �޼��� �۽�
    [Command(requiresAuthority = false)]
    void CommandSendMsg(string msg, NetworkConnectionToClient sender = null)
    {
        if(!_connectedNameDic.ContainsKey(sender))
        {
            // -GetComponent�� Player�� ��������, Player�� playerName ������
            // -������ playerName�� Dictionary�� ����
            // - Player �ڷ����� ������ �� �ֵ��� using Mirror.Examples.Chat�� ����

            var player = sender.identity.GetComponent<Player>();
            var playerName = player.playerName;
            _connectedNameDic.Add(sender, playerName);
        }

            // -CommandSendMsg�� OnRecvMessage �Լ� ȣ���� ��ε�ĳ���� �κ� �߰�
        if (!string.IsNullOrWhiteSpace(msg))
        {
            var senderName = _connectedNameDic[sender];
            OnRecvMessage(senderName, msg.Trim());
        }
    }

    // -�������̵忡�� ��� Ŭ���̾�Ʈ���� Ư�� �Լ��� �����ų �� �ֵ���[ClientRpc]�� ����
    // - [ClientRpc]�� ���� OnRecvMessage() �Լ� �߰�
    // -Ŭ����� Ư�� ������ ��� �ޱ� ������ "On"�� �ٿ� ������ �Լ���� ���� ���
    [ClientRpc]
    void OnRecvMessage(string senderName, string msg)
    {

    }

    public void OnClick_SendMsg()
    {
        var currentChatMsg = Input_ChatMsg.text;
        if(!string.IsNullOrWhiteSpace(Input_ChatMsg.text))
        {
            CommandSendMsg(currentChatMsg.Trim());
        }
    }
}

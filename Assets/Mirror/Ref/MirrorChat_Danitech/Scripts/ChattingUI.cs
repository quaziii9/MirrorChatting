using UnityEngine;
using UnityEngine.UI;
using Mirror;
using Mirror.Examples.Chat;
using System.Collections.Generic;
using System.Collections;

public class ChattingUI : NetworkBehaviour
{
    [Header("UI")]
    [SerializeField] Text Text_ChatHistory;
    [SerializeField] Scrollbar ScrollBar_Chat;
    [SerializeField] InputField Input_ChatMsg;
    [SerializeField] Button Btn_Send;

    internal static string _localPlayerName;

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
        //- �����ڿ� ���� �÷��̾��� �̸� �� �� �޼��� ������(���� �־���)
        string formatedMsg = (senderName == _localPlayerName) ?
            $"<color=red>{senderName}:</color> {msg}" :
            $"<color=blue>{senderName}:</color> {msg}";

        AppendMessage(formatedMsg);
    }


    // ===================== [UI] =================================
    // -UIó���� ���� AppendMessage �Լ� �߰�
    void AppendMessage(string msg)
    {
        StartCoroutine(AppendAndScroll(msg));
    }


    //Unitask�� ���߿� �ٲ㺸��
    // - Text�� ä�� ���� �߰�, ��ũ�� �����ֱ�
    IEnumerator AppendAndScroll(string msg)
    {
        Text_ChatHistory.text += msg + "\n";

        yield return null;
        yield return null;

        ScrollBar_Chat.value = 0;
    }

    // ============================================================

    public void OnClick_SendMsg()
    {
        var currentChatMsg = Input_ChatMsg.text;
        if(!string.IsNullOrWhiteSpace(Input_ChatMsg.text))
        {
            CommandSendMsg(currentChatMsg.Trim());
        }
    }

    public void OnClick_Exit()
    {
        //���� ��ư ó��
        NetworkManager.singleton.StopHost();
    }

    public void OnValueChange_ToggleButton(string input)
    {
        // ä�� �Է� �� ���� ��ư Ȱ��ȭ
        Btn_Send.interactable = !string.IsNullOrWhiteSpace(input);
    }

    public void OnEndEdit_SendMsg(string input)
    {
        
        // ���� �� ���� �Լ� ȣ��
        if (Input.GetKeyDown(KeyCode.Return)
            || Input.GetKeyDown(KeyCode.KeypadEnter)
            || Input.GetButtonDown("Submit"))
        {
            OnClick_SendMsg();
        }
    }
}

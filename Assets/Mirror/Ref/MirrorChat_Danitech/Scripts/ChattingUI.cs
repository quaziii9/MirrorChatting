using UnityEngine;
using UnityEngine.UI;
using Mirror;
using Mirror.Examples.Chat;
using System.Collections.Generic;
using System.Collections;

public class ChattingUI : NetworkBehaviour
{
    [Header("UI")]
    [SerializeField] Text Text_ChatHistory;      // ä�� ����� ǥ���� �ؽ�Ʈ UI
    [SerializeField] Scrollbar ScrollBar_Chat;   // ä�� ��ũ�ѹ�
    [SerializeField] InputField Input_ChatMsg;   // ä�� �޽��� �Է� �ʵ�
    [SerializeField] Button Btn_Send;            // �޽��� ���� ��ư

    internal static string _localPlayerName;     // ���� �÷��̾� �̸� ����

    // ���� �¸� - ����� �÷��̾�� �̸�
    internal static readonly Dictionary<NetworkConnectionToClient, string> _connectedNameDic = new Dictionary<NetworkConnectionToClient, string>();
    
    public void SetLocalPlayerName(string userName)
    {
        _localPlayerName = userName;
    }
    
    public override void OnStartServer()
    {
        _connectedNameDic.Clear(); // ���� ���� �� ����� �̸� ��� �ʱ�ȭ
    }

    public override void OnStartClient()
    {
        Text_ChatHistory.text = string.Empty; // Ŭ���̾�Ʈ ���� �� ä�� ��� �ʱ�ȭ
    }

    // [Command] ��� ��Ʈ����Ʈ�� �̿��� Ŭ�� -> ������ Ư�� ��� ������ ��û
    // ������ ���� CommandSendMsg��� �Լ��� ���� ������ �޼��� �۽�
    // requiresAuthority = false�� ȣ���� Ŭ���̾�Ʈ�� �� ��ü�� ���� ������ ��� ����� ������ �� ������ �ǹ�
    [Command(requiresAuthority = false)] 
    void CommandSendMsg(string msg, NetworkConnectionToClient sender = null)
    {
        if(!_connectedNameDic.ContainsKey(sender))
        {
            // -GetComponent�� Player�� ��������, Player�� playerName ������
            // -������ playerName�� Dictionary�� ����
            // - Player �ڷ����� ������ �� �ֵ��� using Mirror.Examples.Chat�� ����

            var player = sender.identity.GetComponent<ChatUser>();
            var playerName = player.PlayerName;
            _connectedNameDic.Add(sender, playerName);
        }

        // -CommandSendMsg�� OnRecvMessage �Լ� ȣ���� ��ε�ĳ���� �κ� �߰�
        // �޽����� ��ȿ�ϸ� ��� Ŭ���̾�Ʈ�� �޽��� ����
        if (!string.IsNullOrWhiteSpace(msg))
        {
            var senderName = _connectedNameDic[sender];
            OnRecvMessage(senderName, msg.Trim());
        }
    }

    // -�������̵忡�� ��� Ŭ���̾�Ʈ���� Ư�� �Լ��� �����ų �� �ֵ���[ClientRpc]�� ����
    // - [ClientRpc]�� ���� OnRecvMessage() �Լ� �߰�
    // -Ŭ����� Ư�� ������ ��� �ޱ� ������ "On"�� �ٿ� ������ �Լ���� ���� ���
    
    public void RemoveNameOnServerDisonnect(NetworkConnectionToClient conn)
    {
        _connectedNameDic.Remove(conn);
    }
    
    
    
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


    // Unitask�� ���߿� �ٲ㺸��
    // - Text�� ä�� ���� �߰�, ��ũ�� �����ֱ�
    IEnumerator AppendAndScroll(string msg)
    {
        Text_ChatHistory.text += msg + "\n";

        yield return null; // �� ������ ���
        yield return null;

        ScrollBar_Chat.value = 0; // ��ũ���� �� �Ʒ��� ����
    }
    // ============================================================


    // �޽��� ���� ��ư Ŭ�� �� ȣ��Ǵ� �Լ�
    public void OnClick_SendMsg()
    {
        var currentChatMsg = Input_ChatMsg.text;
        if(!string.IsNullOrWhiteSpace(Input_ChatMsg.text))
        {
            CommandSendMsg(currentChatMsg.Trim());
        }
    }

    //���� ��ư ó��
    public void OnClick_Exit()
    {  
        NetworkManager.singleton.StopHost();
    }

    // ä�� �Է� �� ���� ��ư Ȱ��ȭ
    public void OnValueChange_ToggleButton(string input)
    {
        Btn_Send.interactable = !string.IsNullOrWhiteSpace(input);
    }

    // ���� �� ���� �Լ� ȣ�� �޼��� ����
    public void OnEndEdit_SendMsg(string input)
    {     
        if (Input.GetKeyDown(KeyCode.Return)
            || Input.GetKeyDown(KeyCode.KeypadEnter)
            || Input.GetButtonDown("Submit"))
        {
            OnClick_SendMsg();
        }
    }
}

using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class LoginPopup : MonoBehaviour
{
    // internal
    // ���� �����(������Ʈ) �������� ���ٰ���

    [Header("UI")]
    [SerializeField] internal InputField Input_NetworkAdress;
    [SerializeField] internal InputField Input_UserName;

    [SerializeField] internal Button Btn_StartAsHostServer;
    [SerializeField] internal Button Btn_StartAsClient;

    [SerializeField] internal Text Text_Error;

    // LoginPopup Ŭ������ ���� �ν��Ͻ� ����(���� �ν��Ͻ��� ����)
    // private set�� ����Ͽ� �ܺο��� �ν��Ͻ� �����Ҽ� ����
    public static LoginPopup instance { get; private set; }

    // ��Ʈ��ũ �ּ��� �⺻�� ���� 
    private string _originNetworkAddress;

    private void Awake()
    {
        // instance�� ���� ��ü�� �����Ͽ� �̱��� �ν��Ͻ� �ʱ�ȭ,
        // Loginpopup Ŭ������ �ν��Ͻ��� �ϳ��� �����ϵ��� ����
        instance = this;
    }

    private void Start()
    {
        // �⺻ ��Ʈ��ũ �ּҸ� �����ϴ� �޼��� ȣ��
        SetDefaultNetworkAddress();
    }
    private void Update()
    {
        CheckNatworkAddressValidOnUpdate();
    }

    private void OnEnable()
    {
        // ����� �̸� �Է� �ʵ��� �� ���� �̺�Ʈ ������ ���
        Input_UserName.onValueChanged.AddListener(OnValueChanged_ToggleButton);
    }

    private void OnDisable()
    {
        // ����� �̸� �Է� �ʵ��� �� ���� �̺�Ʈ ������ ����
        Input_UserName.onValueChanged.RemoveListener(OnValueChanged_ToggleButton);
    }

    private void SetDefaultNetworkAddress()
    {
        // ��Ʈ��ũ �ּ� ���� ���, ����Ʈ ����
        // IsNullOrWhiteSpace : ���ڿ��� 'null', ���ڿ�, �Ǵ� ���鹮�ڷθ� �̷���� �ִ��� Ȯ��

        // ����
        // string input = "   ";
        // bool isNullOrWhiteSpace = string.IsNullOrWhiteSpace(input);
        // isNullOrWhiteSpace�� true�� ��
        if (string.IsNullOrWhiteSpace(NetworkManager.singleton.networkAddress))
        {
            NetworkManager.singleton.networkAddress = "localhost";
        }

        // ��Ʈ��ũ �ּ� �������� ����� ��츦 ����� ���� ��Ʈ��ũ �ּ� ����
        _originNetworkAddress = NetworkManager.singleton.networkAddress;
    }


    private void CheckNatworkAddressValidOnUpdate()
    {
        // ��Ʈ��ũ �ּҰ� ��ȿ���������� _originNetworkAddress�� �ǵ���
        if(string.IsNullOrWhiteSpace(NetworkManager.singleton.networkAddress))
        {
            NetworkManager.singleton.networkAddress = _originNetworkAddress;
        }

        // �Էµ� ��Ʈ��ũ �ּҰ� ���� ��Ʈ��ũ �Ŵ����� �ּҿ� ��ġ���� ������ ����ȭ
        if(Input_NetworkAdress.text != NetworkManager.singleton.networkAddress)
        {
            Input_NetworkAdress.text = NetworkManager.singleton.networkAddress;
        }
    }

    public void SetUIOnClientDisconnected()
    {
        this.gameObject.SetActive(true);
        // ����� �̸� �Է� �ʵ��� ������ ����
        Input_UserName.text = string.Empty;
        // ����� �̸� �Է� �ʵ忡 ��Ŀ���� ����
        Input_UserName.ActivateInputField();
    }

    // �������� ����ɶ� ui ������Ʈ
    public void SetUIOnAuthValueChanged()
    {
        Text_Error.text = string.Empty;
        Text_Error.gameObject.SetActive(false);
    }

    public void SetUIOnAuthError(string msg)
    {
        Text_Error.text = msg;
        Text_Error.gameObject.SetActive(true);
    }

    public void OnValueChanged_ToggleButton(string userName)
    {
        //  ���� ����ɶ� ȣ��, ������� ���� ��� ��ư�� Ȱ��ȭ
        bool isUserNameValid = !string.IsNullOrWhiteSpace(userName);
        Btn_StartAsHostServer.interactable = isUserNameValid;
        Btn_StartAsClient.interactable = isUserNameValid;
    }
}

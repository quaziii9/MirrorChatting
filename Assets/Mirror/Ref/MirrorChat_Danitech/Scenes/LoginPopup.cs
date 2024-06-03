using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class LoginPopup : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] internal InputField Input_NetworkAdress;
    [SerializeField] internal InputField Input_UserName;

    [SerializeField] internal Button Btn_StartAsHostServer;
    [SerializeField] internal Button Btn_StartAsClient;

    [SerializeField] internal Text Text_Error;

    public static LoginPopup instance { get; private set; }

    private string _originNetworkAddress;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        SetDefaultNetworkAddress();
    }

    private void OnEnable()
    {
        Input_UserName.onValueChanged.AddListener(OnValueChanged_ToggleButton);
    }

    private void OnDisable()
    {
        Input_UserName.onValueChanged.RemoveListener(OnValueChanged_ToggleButton);
    }

    private void SetDefaultNetworkAddress()
    {
        // ��Ʈ��ũ �ּ� ���� ���, ����Ʈ ����
        if(string.IsNullOrWhiteSpace(NetworkManager.singleton.networkAddress))
        {
            NetworkManager.singleton.networkAddress = "localhost";
        }

        // ��Ʈ��ũ �ּ� �������� ����� ��츦 ����� ���� ��Ʈ��ũ �ּ� ����
        _originNetworkAddress = NetworkManager.singleton.networkAddress;
    }

    private void CheckNatworkAddressValidOnUpdate()
    {
        if(string.IsNullOrWhiteSpace(NetworkManager.singleton.networkAddress))
        {
            NetworkManager.singleton.networkAddress = _originNetworkAddress;
        }

        if(Input_NetworkAdress.text != NetworkManager.singleton.networkAddress)
        {
            Input_NetworkAdress.text = NetworkManager.singleton.networkAddress;
        }
    }

    public void OnValueChanged_ToggleButton(string userName)
    {
        bool isUserNameValid = !string.IsNullOrWhiteSpace(userName);
        Btn_StartAsHostServer.interactable = isUserNameValid;
        Btn_StartAsClient.interactable = isUserNameValid;
    }
}

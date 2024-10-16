using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class LoginPopup : MonoBehaviour
{
    // internal
    // 동일 어셈블리(프로젝트) 내에서만 접근가능

    [Header("UI")]
    [SerializeField] internal InputField Input_NetworkAdress;
    [SerializeField] internal InputField Input_UserName;

    [SerializeField] internal Button Btn_StartAsHostServer;
    [SerializeField] internal Button Btn_StartAsClient;

    [SerializeField] internal Text Text_Error;

    [SerializeField] NetworkingManager _netManager;

    // LoginPopup 클래스의 정적 인스턴스 변수(단일 인스턴스를 저장)
    // private set을 사용하여 외부에서 인스턴스 변경할수 없음
    public static LoginPopup instance { get; private set; }

    // 네트워크 주소의 기본값 저장 
    private string _originNetworkAddress;

    private void Awake()
    {
        // instance를 현재 객체로 설정하여 싱글톤 인스턴스 초기화,
        // Loginpopup 클래스의 인스턴스가 하나만 존재하도록 보장
        instance = this;
        Text_Error.gameObject.SetActive(false);
    }

    private void Start()
    {
        // 기본 네트워크 주소를 설정하는 메서드 호출
        SetDefaultNetworkAddress();
    }
    private void Update()
    {
        CheckNatworkAddressValidOnUpdate();
    }

    private void OnEnable()
    {
        // 사용자 이름 입력 필드의 값 변경 이벤트 리스너 등록
        Input_UserName.onValueChanged.AddListener(OnValueChanged_ToggleButton);
    }

    private void OnDisable()
    {
        // 사용자 이름 입력 필드의 값 변경 이벤트 리스너 해제
        Input_UserName.onValueChanged.RemoveListener(OnValueChanged_ToggleButton);
    }

    private void SetDefaultNetworkAddress()
    {
        // 네트워크 주소 없는 경우, 디폴트 세팅
        // IsNullOrWhiteSpace : 문자열이 'null', 빈문자열, 또는 공백문자로만 이루어져 있는지 확인

        // 예제
        // string input = "   ";
        // bool isNullOrWhiteSpace = string.IsNullOrWhiteSpace(input);
        // isNullOrWhiteSpace는 true가 됨
        if (string.IsNullOrWhiteSpace(NetworkManager.singleton.networkAddress))
        {
            NetworkManager.singleton.networkAddress = "localhost";
        }

        // 네트워크 주소 공란으로 변경될 경우를 대비해 기존 네트워크 주소 보관
        _originNetworkAddress = NetworkManager.singleton.networkAddress;
    }


    private void CheckNatworkAddressValidOnUpdate()
    {
        // 네트워크 주소가 유효하지않으면 _originNetworkAddress로 되돌림
        if(string.IsNullOrWhiteSpace(NetworkManager.singleton.networkAddress))
        {
            NetworkManager.singleton.networkAddress = _originNetworkAddress;
        }

        // 입력된 네트워크 주소가 현재 네트워크 매니저의 주소와 일치하지 않을시 동기화
        if(Input_NetworkAdress.text != NetworkManager.singleton.networkAddress)
        {
            Input_NetworkAdress.text = NetworkManager.singleton.networkAddress;
        }
    }

    public void SetUIOnClientDisconnected()
    {
        this.gameObject.SetActive(true);
        // 사용자 이름 입력 필드의 내용을 지움
        Input_UserName.text = string.Empty;
        // 사용자 이름 입력 필드에 포커스를 맞춤
        Input_UserName.ActivateInputField();
    }

    // 인증값이 변경될때 ui 업데이트
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
        //  값이 변경될때 호출, 비어있지 않은 경우 버튼을 활성화
        bool isUserNameValid = !string.IsNullOrWhiteSpace(userName);
        Btn_StartAsHostServer.interactable = isUserNameValid;
        Btn_StartAsClient.interactable = isUserNameValid;
    }

    public void OnClick_StartAsHost()
    {
        if (_netManager == null)
            return;

        _netManager.StartHost();
        this.gameObject.SetActive(false);
    }

    public void OnClick_StartAsClient()
    {
        if (_netManager == null)
            return;

        _netManager.StartClient();
        this.gameObject.SetActive(false);
    }
}

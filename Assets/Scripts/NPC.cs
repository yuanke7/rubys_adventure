using TMPro;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public float displayTime;
    public GameObject dialogBox;
    // private float _timerDisplay;
    // 获取tmp控件
    public GameObject dlgTxtProGameObject;
    private TextMeshProUGUI _tmTxtBox;
    // 存储当前页数
    private int _curPage = 1;
    private int _totalPages;
    // Start is called before the first frame update
    void Start()
    {
        dialogBox.SetActive(false);
        // _timerDisplay = -1.0f;
        _tmTxtBox = dlgTxtProGameObject.GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        _totalPages = _tmTxtBox.textInfo.pageCount;
    }

    public void DisplayDialog()
    {
        if (!dialogBox.activeSelf)
        {
            _curPage = 1;
            _tmTxtBox.pageToDisplay = _curPage;
            dialogBox.SetActive(true);
        }
        else
        {
            if (_curPage < _totalPages)
            {
                _curPage ++;
                _tmTxtBox.pageToDisplay = _curPage;
            }
            else
            {
                dialogBox.SetActive(false);
            }
        }
    }
}

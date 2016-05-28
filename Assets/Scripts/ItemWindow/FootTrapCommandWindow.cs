using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class FootTrapCommandWindow : MonoBehaviour {
    [SerializeField]
    Image _imageIcon;

    [SerializeField]
    Text _textName;

    [SerializeField]
    Text _textDesc;

    [SerializeField]
    Button _btnClose;

    [SerializeField]
    Button _btnFire;

    Action<TrapActionType, Trap> _trapActionCallback;

    public void Init(Trap trap, Action<TrapActionType, Trap> trapActionCallback) {
        _trapActionCallback = trapActionCallback;
        Assert.IsTrue(_trapActionCallback != null);

        _textName.text = trap.Name();
        _textDesc.text = trap.Description();

        _imageIcon.sprite = Resources.Load<Sprite>(trap.ImagePath());

        _btnClose.onClick.RemoveAllListeners();
        _btnFire.onClick.RemoveAllListeners();

        _btnClose.onClick.AddListener(() => {
            Debug.Log("閉じるが押されました");
            gameObject.SetActive(false);

            _trapActionCallback(TrapActionType.Close, trap);
        });

        _btnFire.onClick.AddListener(() => {
            Debug.Log("踏むが押されました");
            gameObject.SetActive(false);

            _trapActionCallback(TrapActionType.Fire, trap);
        });
    }
}

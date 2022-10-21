using JetBrains.Annotations;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class ButtonPanel : MonoBehaviour
    {
        public event Action<TowerInfo> OnButtonClick;

        [SerializeField]
        private Button button1;
        [SerializeField]
        private Button button2;
        [SerializeField]
        private Button Misckick;

        private Animator animator;
        private TowerInfo towerInfo;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            button1.onClick.AddListener(() => { 
                OnButtonClick.Invoke(towerInfo);
                Close();
            });
            Misckick.onClick.AddListener(() => {
                Close();
            });
        }

        public void Show(TowerInfo info, bool isSellButton)
        {
            if (info != null && info.NextTiers != null && info.NextTiers.Count > 0)
            {
                gameObject.SetActive(true);
                int count = isSellButton ? 2 : 1;
                animator.SetInteger("ButtonCount", count);
                
                button1.image.sprite = info.NextTiers[0].Icon;
                towerInfo = info.NextTiers[0];
            }
        }

        public void Close()
        {
            towerInfo = null;
            gameObject.SetActive(false);
        }
    }

}
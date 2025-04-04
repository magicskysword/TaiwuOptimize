using System;
using DG.Tweening;
using GameData.Domains.Character.AvatarSystem;
using UICommon.Character.Avatar;
using UnityEngine;
using UnityEngine.UI;

namespace TaiwuOptimize.Behaviour
{
    public class AvatarRecord : MonoBehaviour
    {
        public CanvasGroup group;
        public Avatar avatar;
        private bool canRefresh = false;

        public bool CanRefresh => canRefresh;
        public bool NeedRefresh { get; set; } = false;

        public void Init()
        {
            group = gameObject.GetComponent<CanvasGroup>();
            if(group == null)
                group = gameObject.AddComponent<CanvasGroup>();
            avatar = gameObject.GetComponent<Avatar>();
            HideAvatar();
        }

        private void OnEnable()
        {
            if (NeedRefresh)
            {
                ModMain.ModMono.EnqueueRefreshAvatar(this);
                NeedRefresh = false;
            }
        }

        private void OnDisable()
        {
            group.DOKill(true);
        }

        public void Refresh()
        {
            group.DOKill();
            RenderAvatar();
        }

        public void RenderAvatar()
        {
            group.alpha = 0f;
            canRefresh = true;
            var rawImage = gameObject.GetComponent<RawImage>();
            if (rawImage != null)
            {
                rawImage.enabled = false;
            }
            
            avatar.transform.GetChild(0).gameObject.SetActive(true);
            avatar.Refresh();
            canRefresh = false;
            FadeIn();
        }

        private void FadeIn()
        {
            if(ModMain.AvatarFadeInTime > 0f)
            {
                group.DOFade(1f, ModMain.AvatarFadeInTime).SetEase(Ease.InCubic);
            }
            else
            {
                group.alpha = 1f;
            }
        }

        public bool IsActive()
        {
            return gameObject.activeInHierarchy;
        }

        public void HideAvatar()
        {
            group.alpha = 0;
        }
    }
}
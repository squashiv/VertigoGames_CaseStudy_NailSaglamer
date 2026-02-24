using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace WeaponWindow
{
    public enum Category
    {
        Sight,
        Mag,
        Barrel,
        Stock,
        Tactical,
    }

    public class WeaponWindowUI : MonoBehaviour
    {
        public AttachmentCategoryUI[] categories;
        public Camera weaponCamera;
        public Camera[] attachmentCameras;
        public Button equipButton;
        public GameObject weaponModel;

        void Start()
        {
            equipButton.onClick.AddListener(OnEquipClicked);
            
            ActivateRenderersWithPrefix("sk_primary_dash_att_", false);

            categories[0].GetComponent<Toggle>().SetIsOnWithoutNotify(true);
            foreach (var category in categories)
            {
                var categoryToggle = category.GetComponent<Toggle>();
                categoryToggle.onValueChanged.AddListener((bool isOn) => OnCategoryClicked(isOn, category));
                
                category.equippedAttachment = category.attachments[0];
                category.equippedAttachmentImage.sprite = category.equippedAttachment.attachmentImage.sprite;
                category.equippedAttachment.GetComponent<Toggle>().SetIsOnWithoutNotify(true);
                foreach (var attachment in category.attachments)
                {
                    var attachmentToggle = attachment.GetComponent<Toggle>();
                    attachmentToggle.onValueChanged.AddListener((bool isOn) => OnAttachmentClicked(isOn, attachment));
                    attachmentToggle.SetIsOnWithoutNotify(attachment == category.equippedAttachment);
                }
                
                ActivateAttachmentsForCategory(category, category == categories[0]);
                ActivateRenderersWithPrefix(category.equippedAttachment.attachmentRendererNamePrefix, true);
            }

            var newCamera = attachmentCameras[0];
            weaponCamera.transform.position = newCamera.transform.position;
            weaponCamera.transform.rotation = newCamera.transform.rotation;
            weaponCamera.fieldOfView = newCamera.fieldOfView;
            
            SetEquipButtonInteractable(false);
        }

        void OnCategoryClicked(bool isOn, AttachmentCategoryUI category)
        {
            if (isOn)
            {
                SetEquipButtonInteractable(false);
                
                var newCamera = attachmentCameras[(int)category.category];

                weaponCamera.transform.DOKill();
                DOTween.Kill(weaponCamera);

                weaponCamera.transform.DOMove(newCamera.transform.position, 0.4f).SetEase(Ease.OutCubic);
                weaponCamera.transform.DORotateQuaternion(newCamera.transform.rotation, 0.4f).SetEase(Ease.OutCubic);
                DOTween.To(() => weaponCamera.fieldOfView, x => weaponCamera.fieldOfView = x, newCamera.fieldOfView, 0.4f).SetEase(Ease.OutCubic);
            }
            else
            {
                var currentAttachmentToggle = category.GetComponent<ToggleGroup>().GetFirstActiveToggle();
                if (currentAttachmentToggle && currentAttachmentToggle.gameObject != category.equippedAttachment.gameObject)
                {
                    category.equippedAttachmentImage.sprite = category.equippedAttachment.attachmentImage.sprite;
                    category.equippedAttachment.GetComponent<Toggle>().isOn = true;
                }
            }
            
            ActivateAttachmentsForCategory(category, isOn);
        }

        void OnAttachmentClicked(bool isOn, AttachmentUI attachment)
        {
            var category = attachment.GetComponent<Toggle>().group.GetComponent<AttachmentCategoryUI>();
            ActivateRenderersWithPrefix(attachment.attachmentRendererNamePrefix, isOn);

            if (isOn)
            {
                SetEquipButtonInteractable(attachment != category.equippedAttachment);
            }
        }

        void OnEquipClicked()
        {
            var category = categories[0].GetComponent<Toggle>().group.GetFirstActiveToggle().GetComponent<AttachmentCategoryUI>();
            var attachment = category.GetComponent<ToggleGroup>().GetFirstActiveToggle().GetComponent<AttachmentUI>();
            category.equippedAttachment = attachment;
            category.equippedAttachmentImage.sprite = attachment.attachmentImage.sprite;
            SetEquipButtonInteractable(false);
        }

        void ActivateAttachmentsForCategory(AttachmentCategoryUI category, bool on)
        {
            foreach (var attachment in category.attachments)
            {
                attachment.gameObject.SetActive(on);
            }
        }

        void ActivateRenderersWithPrefix(string prefix, bool on)
        {
            foreach (var ch in weaponModel.GetComponentsInChildren<Transform>(true))
            {
                if (ch.name.StartsWith(prefix))
                {
                    ch.gameObject.SetActive(on);
                }
            }
        }

        void SetEquipButtonInteractable(bool on)
        {
            equipButton.interactable = on;
            equipButton.GetComponentInChildren<TMP_Text>().text = on ? "Equip" : "Equipped";
        }
    }
}
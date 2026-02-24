using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace WeaponWindow
{
    public class AttachmentCategoryUI : MonoBehaviour
    {
        public Category category;
        public Image equippedAttachmentImage;
        public AttachmentUI[] attachments;
        public AttachmentUI equippedAttachment;
    }
}
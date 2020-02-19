﻿#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-10-21 오후 12:31:30
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// 관리받는(관리자(<see cref="IUIManager"/>가 있는) <see cref="IUIObject"/> 
/// </summary>
public interface IUIObject_Managed
{
    IUIManager pUIManager { get; set; }
}

/// <summary>
/// <see cref="Canvas"/>와 <see cref="IUIWidget"/> 자식들을 가지고 있는 UI의 한 장면을 담당하는 객체
/// </summary>
public interface ICanvas : IUIObject, IUIObject_Managed
{
    // UIWidgetContainer pWidgetContainer { get; }
}

//public class UIWidgetContainer
//{

//}

static public class ICanvasHelper
{

#if UNITY_EDITOR
    [UnityEditor.MenuItem("GameObject/UI/Custom/" + "PopupBase")]
    static public void CreatePopup(MenuCommand pCommand)
    {
        GameObject pObjectParents = pCommand.context as GameObject;

        GameObject pObjectPopup = new GameObject("SomthingPopup");
        GameObjectUtility.SetParentAndAlign(pObjectPopup, pObjectParents);

        RectTransform pRectTransform_Popup = pObjectPopup.AddComponent<RectTransform>();
        pRectTransform_Popup.SetAnchor(AnchorPresets.StretchAll);
        pRectTransform_Popup.sizeDelta = Vector2.zero;
        pObjectPopup.AddComponent<Canvas>();
        pObjectPopup.AddComponent<GraphicRaycaster>();

        GameObject pObjectBG = new GameObject("Image_BG");
        GameObjectUtility.SetParentAndAlign(pObjectBG, pObjectPopup);

        Image pImageBG = pObjectBG.AddComponent<Image>();
        pImageBG.color = Color.white;
        pImageBG.rectTransform.SetAnchor(AnchorPresets.StretchAll);
        pImageBG.rectTransform.sizeDelta = new Vector2(-600f, -300f);

        GameObject pObjectTitleBG = new GameObject("Image_TitleBG");
        GameObjectUtility.SetParentAndAlign(pObjectTitleBG, pObjectBG);

        Image pImageTitleBG = pObjectTitleBG.AddComponent<Image>();
        pImageTitleBG.color = Color.gray;
        pImageTitleBG.rectTransform.SetAnchor(AnchorPresets.TopCenter);
        pImageTitleBG.rectTransform.sizeDelta = new Vector2(300f, 100f);

        GameObject pObjectTitleText = new GameObject("Text_Title");
        GameObjectUtility.SetParentAndAlign(pObjectTitleText, pObjectTitleBG);

        Text pTextTitle = pObjectTitleText.AddComponent<Text>();
        pTextTitle.color = Color.black;
        pTextTitle.text = "Title";



        GameObject pObjectButtonClose = new GameObject("Button_Close");
        GameObjectUtility.SetParentAndAlign(pObjectButtonClose, pObjectBG);

        Image pImageButton = pObjectButtonClose.AddComponent<Image>();
        pImageButton.rectTransform.sizeDelta = new Vector2(150f, 50f);

        pObjectButtonClose.AddComponent<Button>();

        GameObject pObjectButtonCloseText = new GameObject("Text_Close");
        GameObjectUtility.SetParentAndAlign(pObjectButtonCloseText, pObjectButtonClose);

        Text pTextCloseButton = pObjectButtonCloseText.AddComponent<Text>();
        pTextCloseButton.rectTransform.SetAnchor(AnchorPresets.StretchAll);
        pTextCloseButton.text = "Close";

        // 생성된 오브젝트를 Undo 시스템에 등록.
        Undo.RegisterCreatedObjectUndo(pObjectPopup, "Create " + pObjectPopup.name);
        Selection.activeObject = pObjectPopup;
    }
#endif


    /// <summary>
    /// 이 오브젝트를 관리하는 매니져를 찾아 매니져를 통해 오브젝트를 켭니다.
    /// </summary>
    static public UICommandHandle<T> DoShow<T>(this T pObject)
        where T : MonoBehaviour, ICanvas
    {
        if (pObject == null)
            return null;

        if (pObject.pUIManager.IsNull())
        {
            Debug.LogWarningFormat("{0} {1} - Not Found Manager - Check Regist Manager", pObject.gameObject.name, nameof(DoShow), pObject);
            pObject.gameObject.SetActive(true);

            return null;
        }

        return pObject.pUIManager.IUIManager_Show(pObject);
    }

    /// <summary>
    /// 이 오브젝트를 관리하는 매니져를 찾아 매니져를 통해 오브젝트를 끕니다.
    /// </summary>
    static public UICommandHandle<T> DoHide<T>(this T pObject)
        where T : ICanvas
    {
        if (pObject == null)
            return null;

        if (pObject.pUIManager.IsNull())
        {
            Debug.LogWarningFormat("{0} {1} - Not Found Manager - Check Regist Manager", pObject.gameObject.name, nameof(DoHide), pObject);
            pObject.gameObject.SetActive(false);

            return null;
        }

        return pObject.pUIManager.IUIManager_Hide(pObject, true);
    }

    /// <summary>
    /// 이 오브젝트를 관리하는 매니져를 찾아 매니져를 통해 오브젝트를 끕니다.
    /// </summary>
    static public void DoHideOnly<T>(this T pObject)
        where T : ICanvas
    {
        if (pObject.IsNull())
            return;

        if (pObject.pUIManager.IsNull())
        {
            Debug.LogWarningFormat("{0} {1} - Not Found Manager - Check Regist Manager", pObject.gameObject.name, nameof(DoHide), pObject);
            pObject.gameObject.SetActive(false);

            return;
        }

        pObject.pUIManager.IUIManager_Hide(pObject, true);
    }

    /// <summary>
    /// 이 오브젝트를 관리하는 매니져를 찾아 매니져를 통해 오브젝트를 끕니다.
    /// </summary>
    static public UICommandHandle<T> DoHide_NotPlayHideCoroutine<T>(this T pObject)
        where T : ICanvas
    {
        if (pObject.IsNull())
            return null;

        if (pObject.pUIManager.IsNull())
        {
            Debug.LogWarningFormat("{0} {1} - Not Found Manager - Check Regist Manager", pObject.gameObject.name, nameof(DoHide_NotPlayHideCoroutine), pObject);
            pObject.gameObject.SetActive(false);

            return null;
        }

        return pObject.pUIManager.IUIManager_Hide(pObject, false);
    }

    /// <summary>
    /// 이 오브젝트의 UI 상태를 얻습니다.
    /// </summary>
    static public EUIObjectState GetUIObjectState<T>(this T pObject)
        where T : ICanvas
    {
        if (pObject.IsNull())
            return EUIObjectState.Disable;

        if (pObject.pUIManager.IsNull())
        {
            Debug.LogWarningFormat("{0} {1} - Not Found Manager - Check Regist Manager", pObject.gameObject.name, nameof(GetUIObjectState), pObject);
            return EUIObjectState.Disable;
        }

        return pObject.pUIManager.IUIManager_GetUIObjectState(pObject);
    }
}
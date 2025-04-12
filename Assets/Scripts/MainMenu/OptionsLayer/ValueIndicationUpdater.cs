using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ValueIndicationUpdater : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI m_valueIndicator;

    [SerializeField] 
    private Scrollbar m_scrollBar;

    private void Update()
    {
        m_valueIndicator.text = Mathf.Floor(m_scrollBar.value * 100).ToString();
    }

    private void OnValidate()
    {
        if (m_scrollBar == null || m_scrollBar != transform.GetComponentInParent<Scrollbar>())
        {
            m_scrollBar = transform.GetComponentInParent<Scrollbar>();
        }

        if (m_valueIndicator == null || m_valueIndicator != transform.GetComponent<TextMeshProUGUI>())
        {
            m_valueIndicator = transform.GetComponent<TextMeshProUGUI>();
        }
    }
}

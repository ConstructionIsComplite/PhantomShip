using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuLayersController : MonoBehaviour
{
    [SerializeField] private Canvas m_mainMenu;
    [SerializeField] private Canvas m_options;

    private List<Canvas> m_layers = new();

    private void Start()
    {
        m_layers.Add(m_mainMenu);
        m_layers.Add(m_options);

        foreach (var layer in m_layers)
        {
            if (layer == m_mainMenu)
            {
                layer.gameObject.SetActive(true);
            }
            else
            {
                layer.gameObject.SetActive(false);
            }
        }
    }

    public void ToggleLayers(Canvas necessaryLayer)
    {
        foreach (var layer in m_layers)
        {
            if (layer == necessaryLayer)
            {
                layer.gameObject.SetActive(true);
            }
            else if (layer != necessaryLayer)
            {
                layer.gameObject.SetActive(false);
            }
        }
    }
}

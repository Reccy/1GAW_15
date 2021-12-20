using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Preloader : MonoBehaviour
{
    [SerializeField] private ShaderVariantCollection m_shaderVariantCollection;

    private void Start()
    {
        if (m_shaderVariantCollection == null)
            FindObjectOfType<SceneLoader>().LoadScene();
        else
            StartCoroutine(Preload());
    }

    private IEnumerator Preload()
    {
        Debug.Log("Starting Preload");
        m_shaderVariantCollection.WarmUp();

        while (!m_shaderVariantCollection.isWarmedUp)
            yield return new WaitForEndOfFrame();

        Debug.Log("Preload Done");

        FindObjectOfType<SceneLoader>().LoadScene();
    }
}

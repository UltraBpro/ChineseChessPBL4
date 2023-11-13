using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScreen : MonoBehaviour
{
    public GameObject LoadingIcon;
    private void Start()
    {
        StartCoroutine(BeginLoadScene());
    }
    private void Update()
    {
        LoadingIcon.transform.Rotate(new Vector3(0, 0, -100 * Time.deltaTime));
    }
    IEnumerator BeginLoadScene()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(LoadingThings.LoadingTarget);
        operation.allowSceneActivation = false;
        while (!operation.isDone)
        {
            if (operation.progress >= 0.9f)
            {
                // Đợi một thời gian trước khi hoàn thành hoạt động
                yield return new WaitForSeconds(2f);

                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
public static class LoadingThings
{
    public static int LoadingTarget=0;
}

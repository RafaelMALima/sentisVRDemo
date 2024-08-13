using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Sentis;

public class getImage : MonoBehaviour
{
    public Camera _mCamera;

    public ModelAsset modelAsset;
    private Model runtimeModel;
    private IWorker worker;
    private TensorFloat inputTensor;

    public RenderTexture renderTexture;
    private ImageResizer resizer;
    public float[] results;
    

    void Start()
    {
        _mCamera = Camera.main;
        resizer = new ImageResizer();
        runtimeModel = ModelLoader.Load(modelAsset);
        worker = WorkerFactory.CreateWorker(BackendType.GPUCompute, runtimeModel);
        if (renderTexture == null)
        {
            renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
        }


    }

    void Update()
    {
        results = CallModel();
    }

    float[] CallModel()
    {
        _mCamera.targetTexture = renderTexture;
        _mCamera.Render();

        Texture2D texture2D = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
        RenderTexture.active = renderTexture;
        texture2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture2D.Apply();

        RenderTexture.active = null;
        _mCamera.targetTexture = null;

        Texture2D input = resizer.ResizeImage(texture2D, 244, 244);
        Debug.Log(input.width + " " + input.height);
        saveToAssets(input);
        return RunInference(input);

 
    }


    public float[] RunInference(Texture2D inputTexture)
    {
        inputTensor?.Dispose();
        inputTensor = TextureConverter.ToTensor(inputTexture,244,244,3);
        worker.Execute(inputTensor);

        TensorFloat outputTensor = worker.PeekOutput() as TensorFloat;
        outputTensor.MakeReadable();
        results =  outputTensor.ToReadOnlyArray();
        outputTensor.Dispose();
        return results;
    }

    void saveToAssets(Texture2D input)
    {
        // write the texture to a 244x244 png on assets folder
        byte[] bytes = input.EncodeToPNG();
        System.IO.File.WriteAllBytes(Application.dataPath + "/input.png", bytes);
        
    }
    void OnDestroy()
    {
        if (renderTexture != null)
        {
            renderTexture.Release();
        }
    }
}

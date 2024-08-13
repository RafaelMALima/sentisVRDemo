using System.Collections;
using System.Collections.Generic;
using Unity.Sentis.Layers;
using UnityEngine;
using Unity.Sentis;

public class runModel : MonoBehaviour
{

    [SerializeField]
    public ModelAsset modelAsset;
    private Model runtimeModel;
    private IWorker worker;
    private TensorFloat inputTensor;
    
    private float[] results;

    void Start()
    {
        runtimeModel = ModelLoader.Load(modelAsset);
        worker = WorkerFactory.CreateWorker(BackendType.GPUCompute, runtimeModel);
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
    private void OnDisable()
    {
        inputTensor?.Dispose();
        worker.Dispose();
    }
}

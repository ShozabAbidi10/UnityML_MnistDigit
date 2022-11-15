using System;
using System.Collections; 
using System.Collections.Generic;
using System.Linq;
using Unity.Barracuda;
using UnityEngine;

public class ModelInference : MonoBehaviour
{

    // In Barracude we don't really have to do something if its already a 'Texture'. {What does this mean?}
    // Input
    public Texture2D texture;

    public NNModel modelAsst;
    
    private Model _runtimeModel;
    
    private IWorker _engine;

    [Serializable]
    public struct Prediction
    {
        public int predictedValue;
        public float[] predicted;

        public void SetPrediction(Tensor t)
        {
            predicted = t.AsFloats();
            predictedValue = Array.IndexOf(predicted, predicted.Max());
            Debug.Log(message:$"Predicted {predictedValue}");
        }
    } 

    public Prediction prediction;

    // Start is called before the first frame update
    void Start()
    {
        _runtimeModel = ModelLoader.Load(modelAsst);

        // Barracuda is very cool bcoz it take these models and is able to pass them into 'shadder' that runs on GPU.
        // We are essentially doing compute shadders.
        _engine = WorkerFactory.CreateWorker(_runtimeModel, WorkerFactory.Device.GPU);
        prediction = new Prediction();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            // making a gray-scale tensor which will be our output, out of a gray-scale texture.
            var channelCount = 1; // 1 = grayscale, 3 = color, 4 = color+alpha. 
            var inputX = new Tensor(texture, channelCount);

            // output
            // Here, PeekOutput going to examine the content of buffer and pull it into memory temporary. This more
            // efficient but its value will replace in next iteration. 
            Tensor outputX = _engine.Execute(inputX).PeekOutput();

            // At this point we are done with inputX. So we will despose it. 
            inputX.Dispose();
            prediction.SetPrediction(outputX);
        }
    }

    private void OnDestroy()
    {
        _engine.Dispose();
    }
}

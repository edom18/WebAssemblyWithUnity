using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Wasm;
using Wasm.Interpret;

public class WebAssemblyTest : MonoBehaviour
{
    [SerializeField] private Text _text = null;
    [SerializeField] private int _param = 100;
    [SerializeField] private string _url = "http://localhost:8080/files/test.wasm";

    private IReadOnlyList<object> GetParam(IReadOnlyList<object> args)
    {
        return new object[] {_param,};
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 50, 20), "Param: ");
        
        string paramStr = GUI.TextField(new Rect(70, 10, 100, 20), _param.ToString());
        int.TryParse(paramStr, out _param);
        
        if (GUI.Button(new Rect(10, 50, 130, 30), "Load from file"))
        {
            LoadFromFile();
        }

        if (GUI.Button(new Rect(10, 90, 130, 30), "Load from Server"))
        {
            LoadFromServer();
        }
    }

    private void LoadFromServer()
    {
        UnityWebRequest req = UnityWebRequest.Get(_url);
        req.SendWebRequest().completed += operation =>
        {
            MemoryStream stream = new MemoryStream();

            stream.Write(req.downloadHandler.data, 0, req.downloadHandler.data.Length);
            stream.Seek(0, SeekOrigin.Begin);

            WasmFile file = WasmFile.ReadBinary(stream);
            
            Perform(file);
        };
    }

    private void LoadFromFile()
    {
        string path = Application.streamingAssetsPath + "/test.wasm";
        WasmFile file = WasmFile.ReadBinary(path);
        Perform(file);
    }

    private void Perform(WasmFile file)
    {
        var importer = new PredefinedImporter();
        importer.DefineFunction(
            "GetParam",
            new DelegateFunctionDefinition(
                new WasmValueType[] { },
                new[] {WasmValueType.Int32,},
                GetParam));

        ModuleInstance module = ModuleInstance.Instantiate(file, importer);
        if (module.ExportedFunctions.TryGetValue("Test", out FunctionDefinition funcDef2))
        {
            IReadOnlyList<object> results = funcDef2.Invoke(new object[] {1,});
            Debug.Log(results[0]);
            _text.text = results[0].ToString();
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wasm;
using Wasm.Interpret;

public class WebAssemblyTest : MonoBehaviour
{
    [SerializeField] private int _param = 100;
    
    private IReadOnlyList<object> GetParam(IReadOnlyList<object> args)
    {
        return new object[] { _param, };
    }

    private void Start()
    {
        string path = Application.streamingAssetsPath + "/test.wasm";
        WasmFile file = WasmFile.ReadBinary(path);

        var importer = new PredefinedImporter();
        importer.DefineFunction(
            "GetParam",
            new DelegateFunctionDefinition(
                new WasmValueType[] { },
                new [] { WasmValueType.Int32, },
                GetParam));

        ModuleInstance module = ModuleInstance.Instantiate(file, importer);
        if (module.ExportedFunctions.TryGetValue("Add", out FunctionDefinition funcDef))
        {
            IReadOnlyList<object> results = funcDef.Invoke(new object[] { 1, 2, });
            Debug.Log(results[0]);
        }
        
        if (module.ExportedFunctions.TryGetValue("Test", out FunctionDefinition funcDef2))
        {
            IReadOnlyList<object> results = funcDef2.Invoke(new object[] { 1, });
            Debug.Log(results[0]);
        }
    }

    void Update()
    {
    }
}
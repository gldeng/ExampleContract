using System;
using AElf.Sdk.CSharp;

namespace HelloWorld
{
    public class HelloWorld : CSharpSmartContract
    {
        public string Greet()
        {
            return "Hello world!";
        }
    }
}
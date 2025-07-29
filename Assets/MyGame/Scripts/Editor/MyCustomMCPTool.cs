using System.ComponentModel;
using ModelContextProtocol.Server;

[McpServerToolType, Description("カスタムMCPツールの説明")]
public class MyCustomMCPTool
{
    [McpServerTool, Description("メソッドの説明")]
    public string MyMethod()
    {
        return "Hello from Unity!";
    }
}
/*
Copyright 2024 The Spice.ai OSS Authors

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

namespace Spice.Config;

internal static class UserAgent
{
    public static string agent(string? client = null, string? clientVersion = null, string? clientSystem = null, string? clientExtension = null)
    {
        // get OS type, release and machine type (x86, x64, arm, etc)
        var os = Environment.OSVersion;
        var osTypeStr = os.Platform.ToString();
        var osVersion = os.VersionString;
        osVersion = osVersion.Replace($"{osTypeStr} ", ""); // remove osType from version
        if (osTypeStr == "Win32NT")
        {
            osVersion = osVersion.Replace("Microsoft Windows NT ", ""); // remove "Microsoft Windows NT " from version
            osTypeStr = "Windows";
        }

        var osArch = System.Runtime.InteropServices.RuntimeInformation.OSArchitecture;

        var osArchStr = "unknown";
        // replace X64 with amd64, X86 with i386, etc
        switch (osArch)
        {
            case System.Runtime.InteropServices.Architecture.X64:
                osArchStr = "x86_64";
                break;
            case System.Runtime.InteropServices.Architecture.X86:
                osArchStr = "i386";
                break;
            case System.Runtime.InteropServices.Architecture.Arm:
                osArchStr = "arm";
                break;
            case System.Runtime.InteropServices.Architecture.Arm64:
                osArchStr = "aarch64";
                break;
        }

        // get the runtime version
        var appVersion = typeof(SpiceClient).Assembly.GetName().Version;

        var clientName = client ?? "spice-dotnet";
        var clientVer = clientVersion ?? appVersion?.ToString();
        var clientSys = clientSystem ?? $"{osTypeStr}/{osVersion} {osArchStr}";
        var clientExt = $" {clientExtension}" ?? "";
        // return the user agent string
        return $"spice-dotnet/{clientVer} ({clientSys}){clientExt}";
    }
}
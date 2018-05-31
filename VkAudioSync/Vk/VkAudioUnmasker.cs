using System;
using System.Reflection;
using MsieJavaScriptEngine;
using MsieJavaScriptEngine.Helpers;

namespace VkAudioSync.Vk
{
    internal static class VkAudioUnmasker
    {
        public static string UnmaskFrom(string maskedUrl, int userId)
        {
            try
            {
                using (var jsEngine = new MsieJsEngine())
                {
                    jsEngine.ExecuteResource("VkAudioSync.Vk.audioUnmask.js", Assembly.GetExecutingAssembly());
                    return jsEngine.Evaluate<string>($"unmaskUrl(\"{maskedUrl}\", {userId});");
                }
            }
            catch (JsEngineLoadException e)
            {
                throw new Exception($"JS Compile error: {JsErrorHelpers.Format(e)}");
            }
            catch (JsRuntimeException e)
            {
                throw new Exception($"JS Runtime error: {JsErrorHelpers.Format(e)}");
            }
        }
    }
}

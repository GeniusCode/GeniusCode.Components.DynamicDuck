using Microsoft.VisualStudio.TestTools.UnitTesting;
using GeniusCode.Components.DynamicDuck;
using GeniusCode.Components.DynamicDuck.Providers;

namespace GcDynamic.Tests.Voice
{

    public interface IGCSpeechClass
    {
        int Speak(string Text);
        object GetVoices();
    }

    [TestClass]
    public class Voice
    {

        [TestMethod]
        public void Test_Say_Hi()
        {
            IGCSpeechClass speech = DynamicProxyFactory.DuckCOMObject<IGCSpeechClass>("Sapi.SpVoice");
            speech.Speak("Anyone can be a Genius.  GeniusCode LLC, Copyright 2010");
        }

    }
}

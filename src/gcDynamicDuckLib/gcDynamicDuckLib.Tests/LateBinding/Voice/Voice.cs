using Microsoft.VisualStudio.TestTools.UnitTesting;

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

        //NOTE: This does not work on the build server, find a better COM test
        //[TestMethod]
        //public void Test_Say_Hi()
        //{
        //    IGCSpeechClass speech = DynamicProxyFactory.DuckCOMObject<IGCSpeechClass>("Sapi.SpVoice");
        //    speech.Speak("Anyone can be a Genius.  GeniusCode LLC, Copyright 2010");
        //}

    }
}

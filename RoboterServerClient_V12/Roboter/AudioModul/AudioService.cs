 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AudioModul
{
    class AudioService
    {
        private WaveInRecorder m_Recorder;
        private byte[] m_RecBuffer;
        

        UdpSender udpsender = null;
        int port = 2001;
        string actip = "127.0.0.1";

        public AudioService(string actip, int port)
        {
            this.actip = actip;
            this.port = port;
        }

        private void DataArrived(IntPtr data, int size)
        {
            if (m_RecBuffer == null || m_RecBuffer.Length < size)
                m_RecBuffer = new byte[size];
            System.Runtime.InteropServices.Marshal.Copy(data, m_RecBuffer, 0, size);
            //Send to Receiver
            udpsender.Send(m_RecBuffer);
        }

        public void Stop_AudioServer()
        {
            if (m_Recorder != null)
                try
                {
                    m_Recorder.Dispose();
                }
                finally
                {
                    m_Recorder = null;
                }
        }

        public void Start_AudioServer()
        {
            Stop_AudioServer();
            try
            {
                //create UdpSender ************************
                udpsender = new UdpSender(actip, port);
                WaveFormat fmt = new WaveFormat(44100, 16, 2);
                m_Recorder = new WaveInRecorder(-1, fmt, 16384, 3, new BufferDoneEventHandler(DataArrived));
            }
            catch
            {
                Stop_AudioServer();
                throw;
            }
        }
    }
}

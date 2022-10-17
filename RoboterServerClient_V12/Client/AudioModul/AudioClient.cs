using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace AudioModul
{
    class AudioClient
    {
        private WaveOutPlayer m_Player;
       
        private byte[] m_PlayBuffer;
        int port = 2002;
        public AudioClient(int port)
        {
            this.port = port;
        }

        public void Stop_AudioClient()
        {
            if (m_Player != null)
                try
                {
                    m_Player.Dispose();
                }
                finally
                {
                    m_Player = null;
                    udpreceiver.Close();
                }
        }

        public void Start_AudioClient()
        {
            Stop_AudioClient();
            try
            {
                udpreceiver = new UdpReceiver(port);
                WaveFormat fmt = new WaveFormat(44100, 16, 2);
                m_Player = new WaveOutPlayer(
                    -1, fmt, 16384, 3, new BufferFillEventHandler(Filler));
            }
            catch
            {
                Stop_AudioClient();
                throw;
            }
        }

        UdpReceiver udpreceiver = null;
        private void Filler(IntPtr data, int size)
        {
            //read from Sender
            m_PlayBuffer = udpreceiver.Receive();
            System.Runtime.InteropServices.Marshal.Copy(
                m_PlayBuffer, 0, data, size);
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace br.com.Bonus630DevToolsBar
{
    public class PipesServer
    {
        private NamedPipeServerStream _stream;

        public async Task StartServerAsync(string pipeName)
        {
            _stream = new NamedPipeServerStream(pipeName,PipeDirection.Out,6,PipeTransmissionMode.Byte);
            await _stream.WaitForConnectionAsync();
        }
        public void StopServer()
        {
            _stream.Close();
            _stream.Dispose();
        }
    }
    public class PipesClient
    {
        private NamedPipeClientStream _stream;

        public void ConnectToServer(string pipeName)
        {
            _stream = new NamedPipeClientStream("DataSourcePipe", pipeName, PipeDirection.In,PipeOptions.Asynchronous);
            _stream.Connect();
        }
        public void StopServer()
        {
            _stream.Close();
            _stream.Dispose();
        }
    }
}

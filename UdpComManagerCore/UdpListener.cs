using BaseComManager;
using PackageManager;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace UdpComManager
{
    public class UdpListener : BaseListener<UdpDestInfo, UdpInitInfo>, IBaseListener<UdpDestInfo, UdpInitInfo>
    {
        #region Events

        public override event NetworkPackageReceived PackageReceived;
        public override event NetworkBytesReceived BytesPackageReceived;
        public override event NetworkTextReceived TextReceived;
        public override event SendPackage<UdpDestInfo> SendPackage;
        public override event SendMessage<UdpDestInfo> SendMessage;
        public override event SendByteArray<UdpDestInfo> SendByteArray;

        #endregion

        #region Properties

        public string IpAddresses { get; set; }
        public int Port { get; set; }

        private UdpClient udp;
        private IAsyncResult ar_ = null;
        private NetworkPackageGenerator _packageGenerator; 

        #endregion

        public UdpListener()
        {

        }

        public override void Initialize(NetworkPackageGenerator packageGenerator, UdpInitInfo initInfo) 
        {
            _packageGenerator = packageGenerator;
            IpAddresses = initInfo.IpAddress;
            Port = initInfo.Port;
        }

        public override void Connect()
        {
            udp = new UdpClient(Port);
            StartReceive();
        }

        public override void Disconnect()
        {
            udp = null;
        }

        public override bool Send(string message, UdpDestInfo param)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(message);
            return SendBytes(bytes, param.IpAddress, param.Port);
        }

        public override bool Send(byte[] bytes, UdpDestInfo param)
        {
            return SendBytes(bytes, param.IpAddress, param.Port);
        }

        public override bool Send(NetworkPackage package, UdpDestInfo param)
        {
            byte[] bytes = package.GenerateByteArray();
            return SendBytes(bytes, param.IpAddress, param.Port); 
        }

        public override bool SendFromApi(string message, UdpDestInfo param)
        {
            return this.SendMessage(message, param);
        }

        public override bool SendFromApi(byte[] bytes, UdpDestInfo param)
        {
            return this.SendByteArray(bytes, param);
        }

        public override bool SendFromApi(NetworkPackage package, UdpDestInfo param)
        {
            return this.SendPackage(package, param);
        }




        private void StartReceive()
        {
            ar_ = udp.BeginReceive(Receive, new object());
        }

        private void Receive(IAsyncResult ar)
        {
            IPEndPoint ip = new IPEndPoint(IPAddress.Parse(IpAddresses), Port);
            byte[] bytes = udp.EndReceive(ar, ref ip);
            Receive(bytes);
            StartReceive();
        }

        private void Receive(byte[] bytes)
        {
            ReceivePackage(bytes);
            ReceiveBytes(bytes);
            ReceiveText(bytes);
        }

        private void ReceivePackage(byte[] bytes)
        {
            try
            {
                //NetworkPackage networkPackage = _packageGenerator.Generate(bytes);
                //PackageReceived?.Invoke(networkPackage);
            }
            catch
            {

            }
        }

        private void ReceiveBytes(byte[] bytes)
        {
            try
            {
                BytesPackageReceived?.Invoke(bytes);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void ReceiveText(byte[] bytes)
        {
            try
            {
                TextReceived?.Invoke(Encoding.ASCII.GetString(bytes));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool SendBytes(byte[] bytes, string ipAddress, int port)
        {
            UdpClient client = new UdpClient();
            IPEndPoint ip = new IPEndPoint(IPAddress.Parse(ipAddress), port);
            client.Send(bytes, bytes.Length, ip);
            client.Close();
            return true;
        }

    }
}

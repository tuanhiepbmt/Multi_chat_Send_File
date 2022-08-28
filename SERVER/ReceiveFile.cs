using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
namespace CHAT
{
    class ReceiveFile
    {
        IPEndPoint end;
        Socket sock;
        public ReceiveFile()
        {
            //khởi tạo socket
            sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            //gắn socket đến cổng
            end = new IPEndPoint(IPAddress.Any, 2014);
            sock.Bind(end);
        }
        public static string path;
        public static string MessageCurrent = "";

        public void StartServer()
        {
            try
            {
                //tạo hàm đợi
                sock.Listen(100);
                MessageCurrent = "Sẵn sàng Nhận!";
                //Client kết nối 
                Socket clientSock = sock.Accept();
                //bắt đầu nhận file
                byte[] clientData = new byte[1024 * 5000];
                int receiveByteLen = clientSock.Receive(clientData);
                int fNameLen = BitConverter.ToInt32(clientData, 0);
                string fName = Encoding.ASCII.GetString(clientData, 4, fNameLen);
                BinaryWriter write = new BinaryWriter(File.Open(path + "/" + fName, FileMode.Append));
                write.Write(clientData, 4 + fNameLen, receiveByteLen - 4 - fNameLen);
                write.Close();
                //đóng
                clientSock.Close();
                MessageCurrent = "Đã nhận";
            }
            catch
            {
                MessageCurrent = "Lỗi!";
            }

        }
    }
}

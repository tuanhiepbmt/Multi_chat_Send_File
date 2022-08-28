using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.IO;
namespace CLIENT
{
    class SendFile
    {
        public static string MessageCurrent = "";
        public static void Send(string fName)
        {
            try
            {
                //tạo socket
                Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
                //kết nối đến server
                IPEndPoint end = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 2014);
                sock.Connect(end);
                //bắt đầu truyền File đến server
                string path = "";
                //thay "\" -> "/"
                fName = fName.Replace("\\", "/");
                //lấy đường dẫn
                while (fName.IndexOf("/") > -1)
                {
                    path += fName.Substring(0, fName.IndexOf("/") + 1);
                    fName = fName.Substring(fName.IndexOf("/") + 1);
                }
                byte[] fNameByte = Encoding.ASCII.GetBytes(fName);

                byte[] fileData = File.ReadAllBytes(path + fName);
                byte[] clientData = new byte[4 + fNameByte.Length + fileData.Length];
                byte[] fNameLen = BitConverter.GetBytes(fNameByte.Length);
                fNameLen.CopyTo(clientData, 0);
                fNameByte.CopyTo(clientData, 4);
                fileData.CopyTo(clientData, 4 + fNameByte.Length);
                //gửi dữ liệu
                sock.Send(clientData);
                //đóng
                sock.Close();
                MessageCurrent = "File đã được gửi";
            }
            catch (Exception ex)
            {

            }

        }
    }
}

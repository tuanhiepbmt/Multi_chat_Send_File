using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace CHAT
{
    public partial class Form1 : Form
    {
        public static string path;
        public static string MessageCurrent = "";
        public Form1()
        {
            InitializeComponent();
            //tránh việc đụng độ tài nguyên
            CheckForIllegalCrossThreadCalls = false;
            Connect();
            //Đặt địa chỉ lưu file mặc định
            ReceiveFile.path = "C:/Users/ptuan/OneDrive/Máy tính/";
        }
        //Sẵn sàng nhận lần đầu 
        private void Form1_Load(object sender, EventArgs e)
        {
            if (ReceiveFile.path.Length > 0)
            {
                backgroundWorker1.RunWorkerAsync();
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Close();
        }
        //gửi tin
        private void btnSend_Click(object sender, EventArgs e)
        {
            foreach (Socket item in clientList)
            {
                Send(item);
            }
            AddMessage("SERVER: "+txbMessage.Text);
            txbMessage.Clear();
        }
        //gui file
        private void File_Click(object sender, EventArgs e)
        {
            if (ReceiveFile.path.Length > 0)
            {
                backgroundWorker1.RunWorkerAsync();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            label1.Text = ReceiveFile.MessageCurrent + Environment.NewLine + ReceiveFile.path;
        }

        ReceiveFile server = new ReceiveFile();

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            server.StartServer();
        }

        private void lsvMessage_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private void label1_Click(object sender, EventArgs e)
        {

        }

        IPEndPoint IP;
        Socket skServer;
        //khai báo 1 list các client
        List<Socket> clientList;

        //kết nối đến server
        void Connect()
        {
            clientList = new List<Socket>();//khởi tạo 1 list nhiều client
            //khởi tạo địa chỉ IP và socket để kết nối
            IP = new IPEndPoint(IPAddress.Any, 1997);
            skServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            //đợi kết nối từ client
            skServer.Bind(IP);
            //tạo 1 luồng lăng nghe từ client
            Thread Listen = new Thread( ()  => {
                try
                {
                    while (true)
                    {
                        //tạo hàm đợi
                        skServer.Listen(100);
                        //cho Client kết nối
                        Socket client = skServer.Accept();
                        clientList.Add(client);
                        //tạo luồng nhận thông tin từ client
                        Thread receive = new Thread(Receive);
                        receive.IsBackground = true;
                        receive.Start(client);
                    }
                }
                catch
                {//trong trường hợp lỗi thoát while
                    IP = new IPEndPoint(IPAddress.Any, 1997);
                    skServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
                }
            } );
            Listen.IsBackground = true;
            Listen.Start();
        }

        //đóng kết nối đến server
        void Close()
        {
            skServer.Close();
        }

        //gửi dữ liệu
        void Send(Socket client)
        {
            if ( (client != null) && (txbMessage.Text != string.Empty) )
            {
                client.Send(Serialize(txbMessage.Text));
            }
        }

        //nhận dữ liệu
        void Receive(object obj)
        {
            Socket client = obj as Socket;
            try
            {
                while (true)
                {
                    //khởi tạo mảng byte để nhận dữ liệu
                    byte[] data = new byte[1024 * 5000];
                    client.Receive(data);
                    //chuyển data từ dạng byte sang dạng string
                    string message = (string)Deseriliaze(data);

                    //khi 1 client gửi thì cả server và các client (ngoại trừ thằng client vừa gửi) cùng nhận đc
                    foreach(Socket item in clientList)
                    {
                        if(item != null && item != client )
                        {
                            item.Send(Serialize(message));
                        }                    
                    }

                    AddMessage("CLIENT: "+message);
                }
            }
            catch
            {
                clientList.Remove(client);
                client.Close();
            }
        }

        //add mesage vào khung chat
       public void AddMessage(string s)
        {
            lsvMessage.Items.Add(new ListViewItem() { Text = s });
        }

        //Hàm phân mảnh dữ liệu cần gửi từ dạng string sang dạng byte để gửi đi
        byte[] Serialize(object obj)
        {
            //lưu các byte phân mảnh
            MemoryStream stream = new MemoryStream();
            //phân mảnh dữ liệu sang kiểu byte
            BinaryFormatter formatter = new BinaryFormatter();
            //phân mảnh rồi ghi vào stream
            formatter.Serialize(stream, obj);
            return stream.ToArray();
        }

        //Hàm gom mảnh các byte nhận được rồi chuyển sang kiểu string 
        object Deseriliaze(byte[] data)
        {
            //khởi tạo stream đọc kết quả của quá trình phân mảnh 
            MemoryStream stream = new MemoryStream(data);
            //khởi tạo đối tượng chuyển đổi
            BinaryFormatter formatter = new BinaryFormatter();
            return formatter.Deserialize(stream);
        }
        
    }
}

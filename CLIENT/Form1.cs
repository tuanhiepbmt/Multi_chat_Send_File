using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

namespace CLIENT
{
    public partial class frmCLIENT : Form
    {
        public frmCLIENT()
        {
            InitializeComponent();
            //tránh việc đụng độ tài nguyên
            CheckForIllegalCrossThreadCalls = false;
            //kết nối đến server
            Connect();
        }
        //đóng
        private void frmCLIENT_FormClosed(object sender, FormClosedEventArgs e)
        {
            Close();
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            label1.Text = SendFile.MessageCurrent;
        }

        private void lsvMessage_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void frmCLIENT_Load(object sender, EventArgs e)
        {

        }
        private void btnSend_Click(object sender, EventArgs e)
        {
            Send();
            AddMessage("CLIENT: "+txbMessage.Text);
        }

        private void txbMessage_TextChanged(object sender, EventArgs e)
        {

        }
        //gửi File
        private void File_Click(object sender, EventArgs e)
        {
            FileDialog fd = new OpenFileDialog();//mở hộp chọn thư mục cần gửi
            if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                SendFile.Send(fd.FileName);
                AddMessage("Đã gửi! ");
            }
        }

        IPEndPoint IP;
        Socket client;

        //kết nối đến server
        void Connect()
        {
            //tạo socket
            IP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1997);
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            //bắt đầu kết nôi đến server
            try
            {
                client.Connect(IP);
            }
            catch
            {
                MessageBox.Show("Lỗi kết nối", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            //tạo luồng lắng nghe server khi vừa kết nối tới
            Thread listen = new Thread(Receive);
            listen.IsBackground = true;
            listen.Start();
        }

        //đóng kết nối đến server
        void Close()
        {
            client.Close();
        }

        //gửi dữ liệu
        void Send()
        {
            if(txbMessage.Text != string.Empty)
            {
                client.Send(Serialize(txbMessage.Text));
            }          
        }

        //nhận dữ liệu
        void Receive()
        {
            try
            {
                while (true)
                {
                    byte[] data = new byte[1024 * 5000];
                    client.Receive(data);
                    //chuyển data từ dạng byte sang dạng string
                    string message = (string)Deseriliaze(data);
                    AddMessage("\t\tSERVER : "+message);
                }
            }
            catch
            {
                Close();
            }                      
        }

        //add mesage vào khung chat
        void AddMessage(string s)
        {
            lsvMessage.Items.Add(new ListViewItem() { Text = s });
            txbMessage.Clear(); 
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

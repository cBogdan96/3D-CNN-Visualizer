using System;
using System.Net.Sockets;
using UnityEngine;


public class Connector
{

    byte[] serializedLength = new byte[sizeof(uint)];
    const int READ_SIZE = 8192;
    byte[] readBuffer = new byte[READ_SIZE];
    bool connected = false;
    int numberOfTimes = 0;

    public string ip = "127.0.0.1";
    public int port;
    private TcpClient client;
    [SerializeField]
    private int dataOut;
    public Connector() { }
    public void establishConnection(int port)
    {
        this.port = port;
        client = new TcpClient();

        while (!connected)
        {
            try
            {
                Debug.Log("I am trying to connect to " + port);
                client.Connect(ip, port);
                connected = true;
                Debug.Log("i got connected");
            }
            catch (SocketException retryConnectException)
            {
                System.Threading.Thread.Sleep(5000);
                //client.Connect(ip, port);
                Debug.Log(numberOfTimes);
                if (numberOfTimes == 5)
                {
                    break;
                }
                else
                {
                    numberOfTimes++;
                    continue;
                }
            }
        }
        //client.Connect(ip, port);
    }
    ~Connector()       /// se apeleaza automat cand se distruge obiectul se apeleaza la gc
    {
        Debug.Log("am inchis portul" + port);
        client.GetStream().Close();
        client.Close();
    }

    public void Send(byte[] input)
    {
        NetworkStream ns = client.GetStream();
        ns.Write(input, 0, input.Length);
    }

    public void Receive(ref byte[] frameInfo)
    {
        NetworkStream ns = client.GetStream();

        int bytesRead = ns.Read(serializedLength, 0, sizeof(uint));
        int length = BitConverter.ToInt32(serializedLength, 0);

        Array.Resize(ref frameInfo, length);

        int read = 0;

        while (read < length)
        {
            int currentRead = ns.Read(readBuffer, 0, Math.Min(READ_SIZE, length - read));
            if (currentRead == 0)
            {
                Debug.Log("WTF, cum se paote sa citesc 0 octeti?");
            }
            else
            {
                Array.Copy(readBuffer, 0, frameInfo, read, currentRead);
            }
            read += currentRead;
        }
        if (read > length)
        {
            // https://docs.microsoft.com/en-us/dotnet/api/system.net.sockets.networkstream.read?view=netcore-3.1
            Debug.Log("Super WTF, standardul zice ca asta nu se poate intampla?!");
        }
    }

    public byte[] SendAndReceive(byte[] cameraImput)
    {
        Send(cameraImput);
        byte[] response = new byte[1];
        Receive(ref response);  
        return response;
    }
}
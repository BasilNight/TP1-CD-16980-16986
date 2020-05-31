using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;


namespace Client
{
    internal class Client
    {

        public static void Main(string[] args)
        {
            TcpClient tcpClient = new TcpClient();
            try
            {              
                tcpClient.Connect(IPAddress.Parse("192.168.43.159"), 7778);
            }catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            string message = null;
            string pass = "";
            string tipo = "";
            string curso = "";

            Console.WriteLine("======== Chat Room ========");
            Console.WriteLine("[1] - Registar novo utilizador");
            Console.WriteLine("[2] - Fazer Log-in");
            string escolha = Console.ReadLine();
            Console.Clear();

            if (escolha == "1")
            {
                Console.WriteLine("======== Registar Novo Utilizador ========");
                tcpClient.Client.Send(Encoding.ASCII.GetBytes(escolha));

                //Registo
                Console.Write("Insira o seu Username: ");
                tcpClient.Client.Send(Encoding.ASCII.GetBytes(Console.ReadLine()));

                Console.Write("Insira o seu Numero: ");
                tcpClient.Client.Send(Encoding.ASCII.GetBytes(Console.ReadLine()));

                while (pass.Length == 0)
                {
                    Console.Write("Insira a sua Password: ");
                    pass = Console.ReadLine();
                    if (pass.Length != 0)
                    {
                        tcpClient.Client.Send(Encoding.ASCII.GetBytes(pass));
                    }
                    else Console.WriteLine("Password invalida...");
                }

                while (curso != "1" && curso != "2" && curso != "3" && curso != "4" && curso != "5")
                {
                    Console.WriteLine("Indique o seu Curso:");
                    Console.WriteLine("[1] Design Grafico");
                    Console.WriteLine("[2] Engenharia de Sistemas Informáticos");
                    Console.WriteLine("[3] Engenharia e Gestão Industrial");
                    Console.WriteLine("[4] Finanças");
                    Console.WriteLine("[5] Fiscalidade");
                    curso = Console.ReadLine();
                    if (curso == "1" || curso == "2" || curso == "3" || curso == "4" || curso == "5")
                    {
                        tcpClient.Client.Send(Encoding.ASCII.GetBytes(curso));
                    }else Console.WriteLine("Escolha inválida..."); 
                }

                while (tipo != "1" && tipo != "2")
                {
                    Console.Write("Indique a sua profissão: \n1 - Aluno \n2 - Professor\n");
                    tipo = Console.ReadLine();

                    if (tipo == "1" || tipo == "2")
                    {
                        tcpClient.Client.Send(Encoding.ASCII.GetBytes(tipo));
                    }
                    else Console.WriteLine("Escolha inválida...");
                }


            }
            else if (escolha == "2")
            {
                Console.WriteLine("======== Fazer Log-in ========");
                tcpClient.Client.Send(Encoding.ASCII.GetBytes(escolha));

                //Log in
                Console.Write("Insira o seu Numero: ");
                tcpClient.Client.Send(Encoding.ASCII.GetBytes(Console.ReadLine()));

                Console.Write("Insira a Password: ");
                tcpClient.Client.Send(Encoding.ASCII.GetBytes(Console.ReadLine()));

            }

            NetworkStream stream = tcpClient.GetStream();

            Thread recebeMsg = new Thread(() => RecebeMsg(tcpClient));  // Começa uma nova thread para o client poder receber mensagens enquando comunica com o servidor
            recebeMsg.Start();

            Console.Clear();

            while (true)
            {
                Console.WriteLine(">> ");
                message = Console.ReadLine();

                byte[] byteBuffer = Encoding.ASCII.GetBytes(message);
                stream.Write(byteBuffer, 0, byteBuffer.Length);

                if (message == "/sair") { recebeMsg.Interrupt(); Environment.Exit(0); }

            }

            #region Codigo antigo
            /*
            Console.Write("Insira o seu Username: ");
            tcpClient.Client.Send(Encoding.ASCII.GetBytes(Console.ReadLine()));
            
            Console.Write("Insira a sua Password: ");
            tcpClient.Client.Send(Encoding.ASCII.GetBytes(Console.ReadLine()));

            Console.Write("Indique a sua profissão: \n0 - Aluno \n1 - Professor\n");
            tcpClient.Client.Send(Encoding.ASCII.GetBytes(Console.ReadLine()));

            Console.Clear();
            
            Console.WriteLine("Conectado ao chat.");
            string message = "lel";
            while (message != "sair")
            {
                message = Console.ReadLine();
                tcpClient.Client.Send(Encoding.ASCII.GetBytes(message));
            }
            tcpClient.Close();
        }
        private static void Choice()
        {
            Console.WriteLine("0 - Entrar \n" +
                "1 - Registar nova conta: +");
            Console.ReadLine();
            */
            #endregion
        }

        /// <summary>
        /// Metodo que recebe mensagens do servidor (Ligado a uma thread para receber mensagens enquanto comunica com o servidor)
        /// </summary>
        /// <param name="tcpClient"></param>
        public static void RecebeMsg(TcpClient tcpClient)
        {

            NetworkStream ns = tcpClient.GetStream();
            byte[] receivedMsg = new byte[1024];
            string message = "";
            int byteLidos;

            while (true)
            {
                byteLidos = 0;


                byteLidos = ns.Read(receivedMsg, 0, receivedMsg.Length);
                ns.Flush();


                message = Encoding.ASCII.GetString(receivedMsg, 0, byteLidos);

                Console.WriteLine(message);
                
            }
            
        }
    }
}
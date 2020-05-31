using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Objetos;



namespace Server
{
    internal class Server
    {
        public static void Main(string[] args)
        {
            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, 7778); // Cria endpoint para IP 
            TcpListener tcpListener = new TcpListener(ipEndPoint);
            tcpListener.Start();    //Começa a procurar clients
            Console.WriteLine("Servidor Começou");
            int cont = -1;

            //Começa loop onde vai procurar os clients
            while (true)
            {

                TcpClient tcpClients = tcpListener.AcceptTcpClient();
                string nomeCliente, pass, escolha, numero;
                Tipo tipo = Tipo.NULL;
                Curso curso = Curso.NULL;
                byte[] byteMessage = new byte[1024];
                int bytesLidos;

                bytesLidos = tcpClients.Client.Receive(byteMessage); //Recebe escolha
                escolha = Encoding.ASCII.GetString(byteMessage, 0, bytesLidos);
                Array.Clear(byteMessage, 0, byteMessage.Length);

                if (escolha == "1")
                {

                    //Registo
                    bytesLidos = tcpClients.Client.Receive(byteMessage); //Username
                    nomeCliente = Encoding.ASCII.GetString(byteMessage, 0, bytesLidos);
                    Array.Clear(byteMessage, 0, byteMessage.Length);

                    bytesLidos = tcpClients.Client.Receive(byteMessage); //Numero
                    numero = Encoding.ASCII.GetString(byteMessage, 0, bytesLidos);
                    Array.Clear(byteMessage, 0, byteMessage.Length);

                    bytesLidos = tcpClients.Client.Receive(byteMessage); //Password
                    pass = Encoding.ASCII.GetString(byteMessage, 0, bytesLidos);
                    Array.Clear(byteMessage, 0, byteMessage.Length);

                    bytesLidos = tcpClients.Client.Receive(byteMessage); //Curso
                    string curso1 = Encoding.ASCII.GetString(byteMessage, 0, bytesLidos);

                    #region ESCOLHAS CURSO E TIPO 
                    if (curso1 == "1")
                    {
                        curso = Curso.DG;
                    }
                    else if (curso1 == "2")
                    {
                        curso = Curso.ESI;
                    }
                    else if (curso1 == "3")
                    {
                        curso = Curso.EGI;
                    }
                    else if (curso1 == "4")
                    {
                        curso = Curso.FINANCAS;
                    }
                    else if (curso1 == "5")
                    {
                        curso = Curso.FISCALIDADE;
                    }
                    else curso = Curso.NULL;
                    

                    bytesLidos = tcpClients.Client.Receive(byteMessage); //Type
                    string tipo1 = Encoding.ASCII.GetString(byteMessage, 0, bytesLidos);
                    if (tipo1 == "1")
                    {
                        tipo = Tipo.ALUNO;
                    }
                    else if (tipo1 == "2")
                    {
                        tipo = Tipo.PROFESSOR;
                    }
                    else tipo = Tipo.NULL;
                    #endregion

                    Console.WriteLine("Cliente conectado!");

                    cont += 1;

                    Pessoa pessoa = new Pessoa(nomeCliente, pass, numero, curso, tipo);
                    ControlaCliente controlaCliente = new ControlaCliente();
                    if (controlaCliente.NovoCliente(tcpClients, pessoa, cont) == false)
                    {
                        Console.WriteLine("A operação nao foi concluida...");
                        tcpClients.Close();
                    }

                }else if (escolha == "2")
                {

                    //Log in
                    bytesLidos = tcpClients.Client.Receive(byteMessage); //Numero
                    numero = Encoding.ASCII.GetString(byteMessage, 0, bytesLidos);
                    Array.Clear(byteMessage, 0, byteMessage.Length);

                    bytesLidos = tcpClients.Client.Receive(byteMessage); //Password
                    pass = Encoding.ASCII.GetString(byteMessage, 0, bytesLidos);
                    Array.Clear(byteMessage, 0, byteMessage.Length);

                    ControlaCliente controlaCliente = new ControlaCliente();
                    if (controlaCliente.ClientExistente(tcpClients, numero, pass) == false)
                    {
                        Console.WriteLine("Foi inserida a password errada");
                        tcpClients.Close();
                    }
                }
                
            }
            
            tcpListener.Stop();
            Console.WriteLine("Servidor fechou");
        }
    }
}
/// <summary>
/// Desc: Esta classe controla varias propriedades dos clientes conectados (...evoluir)
/// </summary>

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
    public class ControlaCliente
    {

        #region Listas
        TcpClient tcpClient;
        
        

        private static List<Pessoa> listRegistados;
        private static List<TcpClient> listTcpClient;
        private static List<TcpClient> lobbyGeral;
        private static List<Pessoa> lobbyGeralUsers;
        private static List<TcpClient> lobbyDG;
        private static List<Pessoa> lobbyDGUsers;
        private static List<TcpClient> lobbyESI;
        private static List<Pessoa> lobbyESIUsers;
        private static List<TcpClient> lobbyEGI;
        private static List<Pessoa> lobbyEGIUser;
        private static List<TcpClient> lobbyFinancas;
        private static List<Pessoa> lobbyFinancasUsers;
        private static List<TcpClient> lobbyFiscalidade;
        private static List<Pessoa> lobbyFiscalidadeUsers;

        /// <summary>
        /// Inicializa listas usadas no projeto
        /// </summary>
        static ControlaCliente()
        {
            listRegistados = new List<Pessoa>(); //Lista de pessoas registadas
            listTcpClient = new List<TcpClient>(); // Lista de Clients conectados
            // Lista de clients e Utilizadores conectados no lobby Geral
            lobbyGeral = new List<TcpClient>();
            lobbyGeralUsers = new List<Pessoa>();
            // Lista de clients e Utilizadores conectados no lobby de Design Grafico
            lobbyDG = new List<TcpClient>();
            lobbyDGUsers = new List<Pessoa>();
            // Lista de clients e Utilizadores conectados no lobby de Engenharia de Sistemas Informáticos 
            lobbyESI = new List<TcpClient>();
            lobbyESIUsers = new List<Pessoa>();
            // Lista de clients e Utilizadores conectados no lobby de Engenharia e Gestão Industrial
            lobbyEGI = new List<TcpClient>();
            lobbyEGIUser = new List<Pessoa>();
            // Lista de clients e Utilizadores conectados no lobby de Finanças
            lobbyFinancas = new List<TcpClient>();
            lobbyFinancasUsers = new List<Pessoa>();
            // Lista de clients e Utilizadores conectados no lobby de FIscalidade
            lobbyFiscalidade = new List<TcpClient>();
            lobbyFiscalidadeUsers = new List<Pessoa>();
        }

        #endregion

        #region METODOS PARA ENTRAR NO CHAT COM UTILIZADOR NOVO OU EXISTENTE

        /// <summary>
        /// Metodo para entrar no chat com novo registo
        /// </summary>
        /// <param name="tcpCli"></param>
        /// <param name="cont"></param>
        public bool NovoCliente(TcpClient tcpCli, Pessoa pessoa, int cont)
        {
            this.tcpClient = tcpCli;
            

            if (RegistarUtilizador(pessoa) == true)
            {
                listTcpClient.Add(tcpCli);
                lobbyGeral.Add(tcpCli);
                lobbyGeralUsers.Add(pessoa);
                
                Thread chat = new Thread(() => ChatNovo(pessoa, tcpCli)); //Inicializa nova thread para conversar com objeto pessoa e tcpClient como parametros
                chat.Start(); //Inicia thread
            }
            else
            {
                string msg = "O utilizador com o número "+ pessoa.Numero + " já existe!";
                EnviaMsg(tcpCli, msg);
                return false;
                
            }
            return true;
        }

        /// <summary>
        /// Metodo para entrar no chat com utilizador existente
        /// </summary>
        /// <param name="tcpCli"></param>
        /// <param name="numero"></param>
        public bool ClientExistente(TcpClient tcpCli, string numero,string password)
        {
            int pos = ObterPosUtilizador(numero);
            if (pos != -1)
            {
                if (ExisteUtilizador(numero) == true)
                {
                    if (listRegistados[pos].Password == password)
                    {
                        
                        this.tcpClient = tcpCli;
                        listTcpClient.Add(tcpCli);
                        lobbyGeral.Add(tcpCli);
                        lobbyGeralUsers.Add(listRegistados[pos]);
                        Thread chat = new Thread(() => ChatNovo(listRegistados[pos], tcpCli)); //Inicializa nova thread para conversar com objeto pessoa como parametro 
                        chat.Start(); //Inicia thread

                    }
                    else
                    {
                        string msg = "Palavra passe Errada!";
                        EnviaMsg(tcpCli, msg);
                        return false;
                    }
                }
            }
            return true;
        }

        #endregion

        #region METODO QUE PERMITE A COMUNICAÇÃO ENTRE UTILIZADORES

        /// <summary>
        /// Loop que vai permitir que o client consiga comunicar com outros clients
        /// </summary>
        /// <param name="pessoa"></param>
        public bool ChatNovo(Pessoa pessoa , TcpClient tcpClient)
        {
            byte[] byteMessage = new byte[1024];
            List<TcpClient> lobby;
            List<Pessoa> lobbyUser;
            Console.WriteLine("Utilizador {0} <{1}> Entrou na Sala!", pessoa.Nome, pessoa.Tipo);
            string help = "Faça /help para ver os comandos disponiveis!";
            EnviaMsg(tcpClient, help);
            bool sair = false;
            while (sair == false)
            {
                try
                {
                    NetworkStream networkStream = tcpClient.GetStream();
                    int bytesLidos = networkStream.Read(byteMessage, 0, byteMessage.Length);
                    string msg;
                    //tcpClient.Client.Receive(byteMessage);
                    networkStream.Flush();

                    string message = Encoding.ASCII.GetString(byteMessage, 0, bytesLidos);
                    switch (message) // Switch case para apanhar comandos que o client pode enviar 
                    {
                        case "/sair":
                            lobby = LocalizaClient(tcpClient);
                            msg = "\nO Utilizador " + pessoa.Nome + "Saiu do Chat";
                            BroadcastMensagem(msg, lobby);

                            lobbyUser = LocalizaUser(pessoa);
                            lobby.Remove(tcpClient);
                            lobbyUser.Remove(pessoa);
                            listTcpClient.Remove(tcpClient);
                            networkStream.Close();
                            tcpClient.Close();

                            return false;
                            
                        case "/help":

                            msg = "============ Comandos ============" + "\n/goto lobbyDG" + "\n/goto lobbyESI" + "\n/goto lobbyEGI" + "\n/goto lobbyFinancas" + "\n/goto lobbyFiscalidade" + "\n/goto lobbyGeral" + "\n/list users" + "\n==================================\n"; //Adicionar mais..
                            EnviaMsg(tcpClient,msg);

                            
                            break;
                        case "/goto lobbyDG":
                            lobby = LocalizaClient(tcpClient);
                            msg = "\nO Utilizador " + pessoa.Nome + "Saiu do Chat\n";
                            BroadcastMensagem(msg, lobby);
                            lobby.Remove(tcpClient);
                            lobbyUser = LocalizaUser(pessoa);
                            lobbyUser.Remove(pessoa);
                            lobbyDG.Add(tcpClient);
                            lobbyDGUsers.Add(pessoa);
                            msg = "\nO Utilizador " + pessoa.Nome + "Entrou no Lobby\n";
                            BroadcastMensagem(msg, lobbyDG);
                            msg = "ENTROU NO LOBBY DO CURSO DE DESIGN GRÁFICO\n";
                            EnviaMsg(tcpClient, msg);

                            break;

                        case "/goto lobbyESI":
                            lobby = LocalizaClient(tcpClient);
                            msg = "\nO Utilizador " + pessoa.Nome + " Saiu do Chat\n";
                            BroadcastMensagem(msg, lobby);
                            lobby.Remove(tcpClient);
                            lobbyUser = LocalizaUser(pessoa);
                            lobbyUser.Remove(pessoa);
                            lobbyESI.Add(tcpClient);
                            lobbyESIUsers.Add(pessoa);
                            msg = "\nO Utilizador " + pessoa.Nome + " Entrou no Lobby\n";
                            BroadcastMensagem(msg, lobbyESI);
                            msg = "ENTROU NO LOBBY DO CURSO DE ESI\n";
                            EnviaMsg(tcpClient, msg);

                            break;
                        case "/goto lobbyEGI":
                            lobby = LocalizaClient(tcpClient);
                            msg = "\nO Utilizador " + pessoa.Nome + " Saiu do Chat\n";
                            BroadcastMensagem(msg, lobby);
                            lobby.Remove(tcpClient);
                            lobbyUser = LocalizaUser(pessoa);
                            lobbyUser.Remove(pessoa);
                            lobbyEGI.Add(tcpClient);
                            lobbyEGIUser.Add(pessoa);
                            msg = "\nO Utilizador " + pessoa.Nome + " Entrou no Lobby\n";
                            BroadcastMensagem(msg, lobbyEGI);
                            msg = "ENTROU NO LOBBY DO CURSO DE EGI\n";
                            EnviaMsg(tcpClient, msg);

                            break;
                            
                        case "/goto lobbyFinancas":
                            lobby = LocalizaClient(tcpClient);
                            msg = "\nO Utilizador " + pessoa.Nome + " Saiu do Chat\n";
                            BroadcastMensagem(msg, lobby);
                            lobby.Remove(tcpClient);
                            lobbyUser = LocalizaUser(pessoa);
                            lobbyUser.Remove(pessoa);
                            lobbyFinancas.Add(tcpClient);
                            lobbyFinancasUsers.Add(pessoa);
                            msg = "\nO Utilizador " + pessoa.Nome + " Entrou no Lobby\n";
                            BroadcastMensagem(msg, lobbyFinancas);
                            msg = "ENTROU NO LOBBY DO CURSO DE FINANÇAS\n";
                            EnviaMsg(tcpClient, msg);

                            break;
                        case "/goto lobbyFiscalidade":
                            lobby = LocalizaClient(tcpClient);
                            msg = "\nO Utilizador " + pessoa.Nome + " Saiu do Chat\n";
                            BroadcastMensagem(msg, lobby);
                            lobby.Remove(tcpClient);
                            lobbyUser = LocalizaUser(pessoa);
                            lobbyUser.Remove(pessoa);
                            lobbyFiscalidade.Add(tcpClient);
                            lobbyFiscalidadeUsers.Add(pessoa);
                            msg = "\nO Utilizador " + pessoa.Nome + " Entrou no Lobby\n";
                            BroadcastMensagem(msg, lobbyFiscalidade);
                            msg = "ENTROU NO LOBBY DO CURSO DE FISCALIDADE\n";
                            EnviaMsg(tcpClient, msg);

                            break;
                        case "/goto lobbyGeral":
                            lobby = LocalizaClient(tcpClient);
                            msg = "\nO Utilizador " + pessoa.Nome + " Saiu do Chat\n";
                            BroadcastMensagem(msg, lobby);
                            lobby.Remove(tcpClient);
                            lobbyUser = LocalizaUser(pessoa);
                            lobbyUser.Remove(pessoa);
                            lobbyGeral.Add(tcpClient);
                            lobbyGeralUsers.Add(pessoa);
                            msg = "\nO Utilizador " + pessoa.Nome + " Entrou no Lobby\n";
                            BroadcastMensagem(msg, lobbyGeral);
                            msg = "ENTROU NO LOBBY GERAL\n";
                            EnviaMsg(tcpClient, msg);

                            break;
                        case "/list users":

                            lobby = LocalizaClient(tcpClient);
                            lobbyUser = LocalizaUser(pessoa);
                            msg = "============ Utilizadores Neste Lobby ============";
                            EnviaMsg(tcpClient,msg);

                            foreach (Pessoa pessoa1 in lobbyUser)
                            {
                                msg = "\n" + pessoa1.Nome + " " + "<" + pessoa1.Tipo + ">";
                                EnviaMsg(tcpClient, msg);
                            }

                            msg = "\n==================================================";
                            EnviaMsg(tcpClient, msg);

                            break;

                        default:
                            
                            lobby = LocalizaClient(tcpClient);
                            Broadcast(pessoa.Nome, pessoa.Tipo, pessoa.Curso, message, lobby);
                            Console.WriteLine("\n >{0} <{1}> diz: {2}", pessoa.Nome, pessoa.Tipo, message); // Temporario, para testes
                            break;

                    }   
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                    sair = true;
                    listTcpClient.Remove(tcpClient);
                    lobby = LocalizaClient(tcpClient);
                    lobby.Remove(tcpClient);
                    lobbyUser = LocalizaUser(pessoa);
                    lobbyUser.Remove(pessoa);
                    return false;
                    
                }
            }
            return true;
        }

        #endregion

        #region METODOS QUE ENVIAM MENSAGENS PARA UTILIZADORES

        /// <summary>
        /// Metodos para enviar mensagem de um cliente para todos os outros clients conectados no lobby
        /// </summary>
        public static void Broadcast(string nome, Tipo tipo, Curso curso, string message, List<TcpClient> lobby)
        {
            foreach (TcpClient tcpClient in lobby)
            {
                NetworkStream broadcastStream = tcpClient.GetStream();
                string mensagemEnvio = nome + " " + "<" + curso + ">" + " (" + tipo + ") Diz: " + message;
                byte[] byteEnvio = Encoding.ASCII.GetBytes(mensagemEnvio);
                broadcastStream.Write(byteEnvio, 0, byteEnvio.Length);
                broadcastStream.Flush();
            }

        }

        /// <summary>
        /// Metodos para enviar uma mensagem especifica para todos os outros clients conectados no lobby
        /// </summary>
        public static void BroadcastMensagem(string message, List<TcpClient> lobby)
        {
            foreach (TcpClient tcpClient in lobby)
            {
                NetworkStream broadcastStream = tcpClient.GetStream();
                
                byte[] byteEnvio = Encoding.ASCII.GetBytes(message);
                broadcastStream.Write(byteEnvio, 0, byteEnvio.Length);
                broadcastStream.Flush();
            }

        }

        public static void EnviaMsg(TcpClient tcpClient, string msg)
        {
            NetworkStream broadcastStream = tcpClient.GetStream();
            byte[] byteEnvio = Encoding.ASCII.GetBytes(msg);
            broadcastStream.Write(byteEnvio, 0, byteEnvio.Length);
            broadcastStream.Flush();
            
        }

        #endregion

        /// <summary>
        /// Metodo que localiza client e retorna o lobby onde se encontra
        /// </summary>
        /// <param name="tcpClient"></param>
        /// <returns></returns>
        public List<TcpClient> LocalizaClient(TcpClient tcpClient)
        {

            if (lobbyGeral.Contains(tcpClient))
            {
                return lobbyGeral;

            }
            else if (lobbyFiscalidade.Contains(tcpClient))
            {
                return lobbyFiscalidade;

            }
            else if (lobbyFinancas.Contains(tcpClient))
            {
                return lobbyFinancas;
            }
            else if (lobbyESI.Contains(tcpClient))
            {
                return lobbyESI;
            }
            else if (lobbyEGI.Contains(tcpClient))
            {
                return lobbyEGI;
            }
            else if (lobbyDG.Contains(tcpClient))
            {
                return lobbyDG;
            }
            else return null;

        }

        /// <summary>
        /// Metodo que localiza um utilizador e retorna o lobby onde se encontra
        /// </summary>
        /// <param name="tcpClient"></param>
        /// <returns></returns>
        public List<Pessoa> LocalizaUser(Pessoa pessoa)
        {

            if (lobbyGeralUsers.Contains(pessoa))
            {
                return lobbyGeralUsers;

            }
            else if (lobbyDGUsers.Contains(pessoa))
            {
                return lobbyDGUsers;

            }else if (lobbyEGIUser.Contains(pessoa))
            {
                return lobbyEGIUser;
            }else if (lobbyESIUsers.Contains(pessoa))
            {
                return lobbyESIUsers;
            }else if (lobbyFinancasUsers.Contains(pessoa))
            {
                return lobbyFinancasUsers;
            }else if (lobbyFiscalidadeUsers.Contains(pessoa))
            {
                return lobbyFiscalidadeUsers;
            }
            else return null;

        }


        #region METODOS PARA REGISTAR UTILIZADOR
        /// <summary>
        /// Metodo que regista um novo utilizador na lista 
        /// </summary>
        /// <param name="pessoa"></param>
        /// <returns></returns>
        public static bool RegistarUtilizador(Pessoa pessoa)
        {
            if (ExisteUtilizador(pessoa.Numero) == false)
            {
                listRegistados.Add(pessoa);
                return true;
            }
            else return false;
        }


        /// <summary>
        /// Obtem a posiçao de um utilizador registado na lista
        /// </summary>
        /// <param name="numero"></param>
        /// <returns></returns>
        public static int ObterPosUtilizador(string numero) // Works
        {
            for (int i = 0; i < listRegistados.Count; i++)
            {
                if (listRegistados[i].Numero == numero)
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// Verifica se existe um utilizador na lista
        /// </summary>
        /// <param name="numero"></param>
        /// <returns></returns>
        public static bool ExisteUtilizador(string numero)
        {
            int pos = ObterPosUtilizador(numero);
            if (pos != -1)
            {
                if (listRegistados[pos].Numero == numero) { return true; }
            }
            return false;
        }

        #endregion

        /// <summary>
        /// Lista pacientes estaveis
        /// </summary>
        /// <returns></returns>
        public static bool ListarPessoas(List<Pessoa> pessoas)
        {
            Console.WriteLine("============ Utilizadores Neste Lobby ============");
            foreach (Pessoa pessoa in pessoas)
            {
                Console.WriteLine("" + pessoa.Nome + "<" + pessoa.Curso + ">" + " (" + pessoa.Tipo + ")");
                                  
            }
            Console.WriteLine("==================================================");
            return true;
        }

    }
}

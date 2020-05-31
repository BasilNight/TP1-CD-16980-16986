using System;
using System.Collections.Generic;
using System.Text;

namespace Objetos
{
    public enum Tipo
    {
        ALUNO = 1,
        PROFESSOR = 2,
        NULL
    }

    public enum Curso
    {
        DG = 1,             //Design Grafico
        ESI = 2,            //Engenharia de Sistemas Informáticos
        EGI = 3,            //Engenharia e Gestão Industrial
        FINANCAS = 4,       //Finanças
        FISCALIDADE = 5,    //Fiscalidade
        NULL
    }

    /// <summary>
    /// Descreve uma pessoa básica
    /// </summary>
    public class Pessoa
    {
        #region Atributos

        string nome;
        string password;
        string numero;
        Curso curso;
        Tipo tipo;
        #endregion

        #region Construtores      

        /// <summary>
        /// Cria objeto pessoa com valores predefinidos
        /// </summary>
        public Pessoa()
        {
            nome = "";
            password = "";
            numero = "";
            Curso curso = Curso.NULL;
            
        }
        /// <summary>
        /// Cria objeto pessoa com valores do exterior
        /// </summary>
        /// <param name="n"></param>
        /// <param name="i"></param>
        public Pessoa(string n, string pass, string num, Curso cur, Tipo tip)
        {
            this.nome = n;
            this.password = pass;
            this.numero = num;
            this.curso = cur;
            this.tipo = tip;
        }

        

        #endregion

        #region Propriedades

        
        /// <summary>
        /// Manipula o parametro do nome
        /// </summary>
        public string Nome
        {
            get { return nome; }
            set { nome = value; }
        }

        /// <summary>
        /// manipula a password
        /// </summary>
        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        /// <summary>
        /// Manipula a ocupaçao da pessoa
        /// </summary>
        public Tipo Tipo
        {
            get { return tipo; }
            set { tipo = value; }
        }

        /// <summary>
        /// Manipula o numero da pessoa
        /// </summary>
        public string Numero
        {
            get { return numero; }
            set { numero = value; }
        }

        /// <summary>
        /// Manipula o curso de uma pessoa
        /// </summary>
        public Curso Curso
        {
            get { return curso; }
            set { curso = value; }
        }
        #endregion

        public override string ToString()
        {
            return "\nNome: " + nome + "\nPassword: " + password + "\n Numero: " + numero + "\nCurso: " + curso + "\nTipo: " + tipo;
        }
    }


}


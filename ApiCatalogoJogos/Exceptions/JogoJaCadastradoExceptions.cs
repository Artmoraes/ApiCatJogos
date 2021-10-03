using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiCatalogoJogos.Exceptions
{
    public class JogoJaCadastradoExceptions : Exception
    {
        public JogoJaCadastradoExceptions()
            : base("Este jogo já está cadastrado!")
        { }
    }
}

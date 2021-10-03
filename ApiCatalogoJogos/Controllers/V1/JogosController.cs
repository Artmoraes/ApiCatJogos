using ApiCatalogoJogos.InputModel;
using ApiCatalogoJogos.ViewModel;
using ApiCatalogoJogos.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using ApiCatalogoJogos.Exceptions;

namespace ApiCatalogoJogos.Controllers.V1
{
    [Route("api/V1/[controller]")]
    [ApiController]
    public class JogosController : ControllerBase
    { //get => Consulta / Post => Criar recurso / Put/Patch => Atualizações / Delete => Excluir
        private readonly IJogoService _jogoService;

        public JogosController(IJogoService jogoService)
        {
            _jogoService = jogoService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<JogoViewModel>>> Obter([FromQuery, Range(1, int.MaxValue)] int pagina = 1, [FromQuery, Range(1,50)] int quantidade = 5) //Task se usa nas controllers para melhorar performance em requisições Web, por conta do gerenciamento, aplicações pelo próprio servidor que gerencia
        {
            var jogos = await _jogoService.Obter(pagina, quantidade);

            if (jogos.Count() == 0)
            {
                return NoContent(); //Retorna que não possui conteúdo.
            }
            return Ok(jogos);
        }

        [HttpGet("{idJogo:guid}")] //guid => Forma de identificar as variáveis, struct que gera valor aleatório e único.
        public async Task<ActionResult<JogoViewModel>> Obter([FromRoute] Guid idJogo)
        {
            var jogo = await _jogoService.Obter(idJogo);

            if (jogo == null)
            {
                return NoContent(); //Retorna que não possui conteúdo.
            }
            return Ok(jogo);
        }

        [HttpPost]
        public async Task<ActionResult<JogoViewModel>> InserirJogo([FromBody] JogoInputModel jogoInputModel)
        {
            try //try => tratam exceções ou erros que ocorrem no código.
            {
                var jogo = await _jogoService.Inserir(jogoInputModel);
                return Ok(jogo);
            }
            catch (JogoJaCadastradoExceptions)
            {
                return UnprocessableEntity("Já existe um jogo com este nome para esta produtora!"); //Estado 422, tratativa de caso já exista um jogo com este nome.
            }
        }

        [HttpPut("{idJogo:guid}")] //Put => Atualiza o recurso inteiro, nome, preço, tudo de uma vez.
        public async Task<ActionResult> AtualizarJogo([FromRoute] Guid idJogo, [FromBody] JogoInputModel jogoInputModel)
        {
            try //try => tratam exceções ou erros que ocorrem no código.
            {
                await _jogoService.Atualizar(idJogo, jogoInputModel);
                return Ok();
            }
            catch (JogoNaoCadastradoExceptions)
            {
                return NotFound("Não existe esse jogo!"); 
            }
        }

        [HttpPatch("{idJogo:guid}/preco/{preco:double}")] //Patch => Atualiza uma coisa em específico, como apenas o nome ou o preço.
        public async Task<ActionResult> AtualizarJogo([FromRoute] Guid idJogo, [FromRoute] double preco)
        {
            try //try => tratam exceções ou erros que ocorrem no código.
            {
                await _jogoService.Atualizar(idJogo, preco);
                return Ok();
            }
            catch (JogoNaoCadastradoExceptions)
            {
                return NotFound("Não existe esse jogo!");
            }
        }

        [HttpDelete("{idJogo:guid}")]
        public async Task<ActionResult> ApagarJogo([FromRoute] Guid idJogo)
        {
            try
            {
                await _jogoService.Remover(idJogo);

                return Ok();
            }
            catch (JogoNaoCadastradoExceptions)
            {
                return NotFound("Não existe esse jogo!"); //Estado 404 => Não exite.
            }

        }
    }
}

import React, { useState } from 'react';
import './../PreCadastro.css';
import Text from '../components/Text';
import api from '../utils/Api';
import { showError } from '../components/Messages';
import Formulario from './Formulario';

function PreCadastro() {
   const [formData, setFormData] = useState({});
   const [cadastroRealizado, setCadastroRealizado] = useState(false);
   const [fichaPreenchida, setFichaPreenchida] = useState(false);

   const [buscandoCEP, setBuscandoCEP] = useState(false);

   // Função para aplicar máscara de telefone/celular
   const aplicarMascaraTelefone = (valor) => {
      // Remove tudo que não é dígito
      const apenasDigitos = valor.replace(/\D/g, '');

      // Aplica máscara baseada no número de dígitos
      if (apenasDigitos.length <= 10) {
         // Telefone fixo: (00) 0000-0000
         return apenasDigitos.replace(/(\d{2})(\d{0,4})(\d{0,4})/, '($1) $2-$3').trim();
      } else {
         // Celular: (00) 00000-0000
         return apenasDigitos.replace(/(\d{2})(\d{0,5})(\d{0,4})/, '($1) $2-$3').trim();
      }
   };

   // Função para aplicar máscara de CPF
   const aplicarMascaraCPF = (valor) => {
      // Remove tudo que não é dígito
      const apenasDigitos = valor.replace(/\D/g, '');

      // Aplica máscara: 000.000.000-00
      return apenasDigitos.replace(/(\d{3})(\d{3})(\d{3})(\d{2})/, '$1.$2.$3-$4');
   };

   // Função para aplicar máscara de CEP
   const aplicarMascaraCEP = (valor) => {
      // Remove tudo que não é dígito
      const apenasDigitos = valor.replace(/\D/g, '');

      // Aplica máscara: 00000-000
      return apenasDigitos.replace(/(\d{5})(\d{0,3})/, '$1-$2');
   };

   // Função para buscar CEP na API Via CEP
   const buscarCEP = async (cep) => {
      // Remove caracteres não numéricos
      const cepLimpo = cep.replace(/\D/g, '');

      // Verifica se o CEP tem 8 dígitos
      if (cepLimpo.length !== 8) {
         return;
      }

      setBuscandoCEP(true);

      try {
         const response = await fetch(`https://viacep.com.br/ws/${cepLimpo}/json/`);
         const data = await response.json();

         if (!data.erro) {
            // Preenche os campos do endereço automaticamente
            setFormData((prevState) => ({
               ...prevState,
               logradouro: data.logradouro || '',
               bairro: data.bairro || '',
               cidade: data.localidade || '',
               estado: data.uf?.toLowerCase() || '',
               pais: 'brasil', // Define Brasil como padrão
            }));
         } else {
            showError('CEP não encontrado. Verifique o número digitado.');
         }
      } catch (error) {
         console.error('Erro ao buscar CEP:', error);
         showError('Erro ao buscar CEP. Tente novamente.');
      } finally {
         setBuscandoCEP(false);
      }
   };

   const handleChange = (e) => {
      const { name, value, type, checked, files } = e.target;

      let valorProcessado = value;

      // Aplica máscaras específicas
      if (name === 'celular' || name === 'telefone') {
         valorProcessado = aplicarMascaraTelefone(value);
      } else if (name === 'cpf') {
         valorProcessado = aplicarMascaraCPF(value);
      } else if (name === 'cep') {
         valorProcessado = aplicarMascaraCEP(value);
      }

      setFormData((prevState) => ({
         ...prevState,
         [name]: type === 'checkbox' ? checked : type === 'file' ? files[0] : valorProcessado,
      }));

      // Busca CEP automaticamente quando o campo estiver completo
      if (name === 'cep' && valorProcessado.length === 9) {
         buscarCEP(valorProcessado);
      }
   };

   const handleSubmit = (e) => {
      e.preventDefault();
      console.log('Dados do pré-cadastro:', formData);

      api.post('/lead/precadastro', formData)
         .then((response) => {
            if (response) {
               showError(response).then(() => setFichaPreenchida(true));
            } else {
               setFichaPreenchida(true);
            }
         })
         .catch((error) => {});
   };

   return (
      <div className='pre-cadastro'>
         <div className='pre-cadastro-container'>
            {!cadastroRealizado && (
               <>
                  <header className='pre-cadastro-header'>
                     <h1>Cadastro Dr. Hair</h1>
                     <p>Preencha os dados abaixo para realizar seu cadastro</p>
                  </header>
                  <main className='pre-cadastro-main-content'>
                     <form onSubmit={handleSubmit} className='pre-cadastro-form'>
                        {/* Seção: Dados básicos */}
                        <div className='pre-cadastro-form-section'>
                           <h2 className='pre-cadastro-section-title'>Dados básicos</h2>

                           <div className='pre-cadastro-form-group'>
                              <label htmlFor='nome'>Nome *</label>
                              <input
                                 type='text'
                                 id='nome'
                                 name='nome'
                                 value={formData.nome}
                                 onChange={handleChange}
                                 required
                                 placeholder='Digite seu nome completo'
                              />
                           </div>
                           <div className='pre-cadastro-form-row'>
                              <div className='pre-cadastro-form-group'>
                                 <label htmlFor='cpf'>CPF *</label>
                                 <input
                                    type='text'
                                    id='cpf'
                                    name='cpf'
                                    value={formData.cpf}
                                    onChange={handleChange}
                                    placeholder='000.000.000-00'
                                    maxLength='14'
                                    required
                                 />
                              </div>

                              <div className='pre-cadastro-form-group'>
                                 <label htmlFor='celular'>Celular *</label>
                                 <input
                                    type='tel'
                                    id='celular'
                                    name='celular'
                                    value={formData.celular}
                                    onChange={handleChange}
                                    required
                                    placeholder='(00) 00000-0000'
                                    maxLength='15'
                                 />
                              </div>
                           </div>
                           <div className='pre-cadastro-form-row'>
                              <div className='pre-cadastro-form-group'>
                                 <label htmlFor='dataNascimento'>Data de nascimento *</label>
                                 <input
                                    type='date'
                                    id='dataNascimento'
                                    name='dataNascimento'
                                    value={formData.dataNascimento}
                                    onChange={handleChange}
                                    required
                                 />
                              </div>

                              <div className='pre-cadastro-form-group'>
                                 <label htmlFor='genero'>Gênero *</label>
                                 <select
                                    id='genero'
                                    name='genero'
                                    value={formData.genero}
                                    onChange={handleChange}
                                    required
                                 >
                                    <option value=''></option>
                                    <option value='1'>Masculino</option>
                                    <option value='2'>Feminino</option>
                                    <option value='3'>Outro</option>
                                 </select>
                              </div>
                           </div>
                        </div>

                        {/* Seção: Informações adicionais */}
                        <div className='pre-cadastro-form-section'>
                           <h2 className='pre-cadastro-section-title'>Informações adicionais</h2>
                           <div className='pre-cadastro-form-row'>
                              <div className='pre-cadastro-form-group'>
                                 <label htmlFor='estadoCivil'>Estado Civil</label>
                                 <select
                                    id='estadoCivil'
                                    name='estadoCivil'
                                    value={formData.estadoCivil}
                                    onChange={handleChange}
                                 >
                                    <option value=''></option>
                                    <option value='1'>Solteiro(a)</option>
                                    <option value='2'>Casado(a)</option>
                                    <option value='3'>Viúvo(a)</option>
                                    <option value='4'>Divorciado(a)</option>
                                    <option value='5'>Separado(a)</option>
                                    <option value='6'>União Civil</option>
                                 </select>
                              </div>

                              <div className='pre-cadastro-form-group'>
                                 <label htmlFor='profissao'>Profissão</label>
                                 <select
                                    id='profissao'
                                    name='profissao'
                                    value={formData.profissao}
                                    onChange={handleChange}
                                 >
                                    <option value=''></option>
                                    <option value='68'>Açougueiro(a)</option>
                                    <option value='4'>Administrador(a)</option>
                                    <option value='5'>Advogado(a)</option>
                                    <option value='70'>Agente de Viagem</option>
                                    <option value='69'>Agente Funerário(a)</option>
                                    <option value='71'>Agente Geral</option>
                                    <option value='54'>Agente Penitenciário(a)</option>
                                    <option value='67'>Agricultor(a)</option>
                                    <option value='73'>Ajudante de Obras</option>
                                    <option value='74'>Alfaiate</option>
                                    <option value='75'>Almoxarife</option>
                                    <option value='2'>Analista</option>
                                    <option value='6'>Analista de Sistemas</option>
                                    <option value='7'>Analista Financeiro</option>
                                    <option value='14'>Aposentado(a)</option>
                                    <option value='76'>Aprendiz</option>
                                    <option value='8'>Arquiteto(a)</option>
                                    <option value='10'>Assessor(a)</option>
                                    <option value='78'>Assistente Comercial</option>
                                    <option value='77'>Assistente de Cobrança</option>
                                    <option value='52'>Autônomo(a)</option>
                                    <option value='38'>Auxiliar Administrativo(a)</option>
                                    <option value='72'>Auxiliar de Carga e Descarga</option>
                                    <option value='37'>Auxiliar serviços gerais</option>
                                    <option value='79'>Babá</option>
                                    <option value='42'>Bancário(a)</option>
                                    <option value='84'>Barbeiro(a)</option>
                                    <option value='80'>Barman</option>
                                    <option value='15'>Biolólogo(a)</option>
                                    <option value='16'>Biomédico(a)</option>
                                    <option value='81'>Blogueiro(a)</option>
                                    <option value='82'>Bombeiro(a)</option>
                                    <option value='83'>Borracheiro(a)</option>
                                    <option value='85'>Cabeleireiro(a)</option>
                                    <option value='86'>Camareiro(a)</option>
                                    <option value='87'>Carpinteiro(a)</option>
                                    <option value='88'>Chefe de Cozinha</option>
                                    <option value='89'>Conferente</option>
                                    <option value='90'>Consultor Comercial(a)</option>
                                    <option value='17'>Contador(a)</option>
                                    <option value='91'>Coordenador(a)</option>
                                    <option value='47'>Corretor(a)</option>
                                    <option value='167'>corretora</option>
                                    <option value='35'>Costureira</option>
                                    <option value='50'>Cozinheiro(a)</option>
                                    <option value='92'>Cuidador de Idosos(a)</option>
                                    <option value='3'>DBA</option>
                                    <option value='93'>Decorador(a)</option>
                                    <option value='13'>Dentista</option>
                                    <option value='95'>Desenhista</option>
                                    <option value='1'>Desenvolvedor(a)</option>
                                    <option value='18'>Designer</option>
                                    <option value='94'>Designer</option>
                                    <option value='41'>Diarista</option>
                                    <option value='96'>Diretor Comercial(a)</option>
                                    <option value='97'>DJ</option>
                                    <option value='33'>Do lar</option>
                                    <option value='99'>Doceiro(a)</option>
                                    <option value='98'>Doméstica(a)</option>
                                    <option value='19'>Economista</option>
                                    <option value='100'>Eletricista</option>
                                    <option value='101'>Embalador(a)</option>
                                    <option value='32'>Empresário(a)</option>
                                    <option value='102'>Encanador(a)</option>
                                    <option value='43'>Enfermeiro(a)</option>
                                    <option value='21'>Engenheiro(a)</option>
                                    <option value='105'>Entregador(a)</option>
                                    <option value='106'>Escrevente</option>
                                    <option value='55'>Estagiário(a)</option>
                                    <option value='31'>Esteticista</option>
                                    <option value='103'>Estoquista</option>
                                    <option value='166'>estudante</option>
                                    <option value='46'>Estudante</option>
                                    <option value='104'>Executivo(a)</option>
                                    <option value='45'>Farmacêutico(a)</option>
                                    <option value='107'>Ferreiro(a)</option>
                                    <option value='56'>Fiscal Caixa</option>
                                    <option value='22'>Fisioterapeuta</option>
                                    <option value='39'>Fonoaudiólogo(a)</option>
                                    <option value='24'>Fotógrafo(a)</option>
                                    <option value='108'>Frentista</option>
                                    <option value='23'>Funcionário(a) Público(a)</option>
                                    <option value='109'>Funileiro(a)</option>
                                    <option value='111'>Garagista</option>
                                    <option value='110'>Garçom</option>
                                    <option value='53'>Gerente</option>
                                    <option value='112'>Guia de Turismo</option>
                                    <option value='113'>Hoteleiro(a)</option>
                                    <option value='114'>Instrutor de Autoescola(a)</option>
                                    <option value='115'>Instrutor de Pilates(a)</option>
                                    <option value='116'>Jardineiro(a)</option>
                                    <option value='117'>Jornaleiro(a)</option>
                                    <option value='25'>Jornalista</option>
                                    <option value='120'>Líder de Produção</option>
                                    <option value='119'>Limpador(a)</option>
                                    <option value='118'>Locutor(a)</option>
                                    <option value='121'>Manicure</option>
                                    <option value='122'>Maquiador(a)</option>
                                    <option value='123'>Marceneiro(a)</option>
                                    <option value='124'>Marmorista</option>
                                    <option value='48'>Massoterapeuta</option>
                                    <option value='125'>Mecânico(a)</option>
                                    <option value='12'>Médico(a)</option>
                                    <option value='126'>Merendeiro(a)</option>
                                    <option value='127'>Mestre de Obras</option>
                                    <option value='129'>Montador(a)</option>
                                    <option value='130'>Motoboy</option>
                                    <option value='128'>Motorista</option>
                                    <option value='131'>Músico(a)</option>
                                    <option value='30'>Não informado(a)</option>
                                    <option value='26'>Nutricionista</option>
                                    <option value='132'>Operador de Caixa(a)</option>
                                    <option value='133'>Operador de Máquina(a)</option>
                                    <option value='29'>Outros(a)</option>
                                    <option value='134'>Padeiro(a)</option>
                                    <option value='135'>Passador(a)</option>
                                    <option value='49'>Pedagogo(a)</option>
                                    <option value='136'>Pedreiro(a)</option>
                                    <option value='40'>Pensionista</option>
                                    <option value='20'>Personal Trainer</option>
                                    <option value='137'>Pintor(a)</option>
                                    <option value='138'>Porteiro(a)</option>
                                    <option value='9'>Professor(a)</option>
                                    <option value='140'>Programador(a)</option>
                                    <option value='139'>Projetista</option>
                                    <option value='27'>Psicólogo(a)</option>
                                    <option value='11'>Publicitário(a)</option>
                                    <option value='141'>Radialista</option>
                                    <option value='142'>Recepcionista</option>
                                    <option value='143'>Relações Públicas</option>
                                    <option value='144'>Sapateiro(a)</option>
                                    <option value='145'>Secretário(a)</option>
                                    <option value='147'>Serralheiro(a)</option>
                                    <option value='146'>Servente</option>
                                    <option value='148'>Soldador(a)</option>
                                    <option value='149'>Supervisor(a)</option>
                                    <option value='51'>Supervisor(a) RH</option>
                                    <option value='150'>Técnico(a)</option>
                                    <option value='44'>Técnico(a) Enfermagem</option>
                                    <option value='152'>Telefonista</option>
                                    <option value='151'>Tesoureiro(a)</option>
                                    <option value='153'>Tosador(a)</option>
                                    <option value='36'>Vendedor(a)</option>
                                    <option value='28'>Veterinário(a)</option>
                                    <option value='155'>Vidraceiro(a)</option>
                                    <option value='154'>Vigilante</option>
                                    <option value='156'>Web Designer</option>
                                    <option value='34'>Zelador(a)</option>
                                 </select>
                              </div>
                           </div>

                           <div className='pre-cadastro-form-row'>
                              <div className='pre-cadastro-form-group'>
                                 <label htmlFor='telefone'>Telefone fixo</label>
                                 <input
                                    type='tel'
                                    id='telefone'
                                    name='telefone'
                                    value={formData.telefone}
                                    onChange={handleChange}
                                    placeholder='(00) 0000-0000'
                                    maxLength='14'
                                 />
                              </div>
                              <div className='pre-cadastro-form-group'>
                                 <label htmlFor='email'>Email</label>
                                 <input
                                    type='email'
                                    id='email'
                                    name='email'
                                    value={formData.email}
                                    onChange={handleChange}
                                    placeholder='Digite seu email'
                                 />
                              </div>
                           </div>
                           <div className='pre-cadastro-form-row'>
                              <div className='pre-cadastro-form-group'>
                                 <label htmlFor='rg'>RG</label>
                                 <input
                                    type='text'
                                    id='rg'
                                    name='rg'
                                    value={formData.rg}
                                    onChange={handleChange}
                                    placeholder='Digite seu RG'
                                 />
                              </div>
                              {/* 
                              <div className='pre-cadastro-form-group'>
                                 <label htmlFor='cnh'>CNH</label>
                                 <input
                                    type='text'
                                    id='cnh'
                                    name='cnh'
                                    value={formData.cnh}
                                    onChange={handleChange}
                                    placeholder='Digite sua CNH'
                                 />
                              </div> */}
                           </div>
                        </div>

                        {/* Seção: Endereço */}
                        <div className='pre-cadastro-form-section'>
                           <h2 className='pre-cadastro-section-title'>Endereço</h2>
                           <div className='pre-cadastro-form-row'>
                              {/* <div className='pre-cadastro-form-group'>
                           <label htmlFor='pais'>País</label>
                           <select id='pais' name='pais' value={formData.pais} onChange={handleChange}>
                              <option value='brasil'>Brasil</option>
                           </select>
                        </div> */}
                              <div className='pre-cadastro-form-group'>
                                 <label htmlFor='cep'>CEP</label>
                                 <div className='cep-input-container'>
                                    <input
                                       type='text'
                                       id='cep'
                                       name='cep'
                                       value={formData.cep}
                                       onChange={handleChange}
                                       placeholder='00000-000'
                                       maxLength='9'
                                       disabled={buscandoCEP}
                                    />
                                    {buscandoCEP && (
                                       <div className='loading-indicator'>
                                          <div className='spinner'></div>
                                       </div>
                                    )}
                                 </div>
                                 {buscandoCEP && <small className='loading-text'>Buscando endereço...</small>}
                              </div>
                           </div>
                           <div className='pre-cadastro-form-row'>
                              <div className='pre-cadastro-form-group'>
                                 <label htmlFor='logradouro'>Logradouro</label>
                                 <input
                                    type='text'
                                    id='logradouro'
                                    name='logradouro'
                                    value={formData.logradouro}
                                    onChange={handleChange}
                                    placeholder='Digite o logradouro'
                                 />
                              </div>
                              <div className='pre-cadastro-form-group'>
                                 <label htmlFor='bairro'>Bairro</label>
                                 <input
                                    type='text'
                                    id='bairro'
                                    name='bairro'
                                    value={formData.bairro}
                                    onChange={handleChange}
                                    placeholder='Digite o bairro'
                                 />
                              </div>
                           </div>

                           <div className='pre-cadastro-form-row'>
                              <div className='pre-cadastro-form-group'>
                                 <label htmlFor='cidade'>Cidade</label>
                                 <input
                                    type='text'
                                    id='cidade'
                                    name='cidade'
                                    value={formData.cidade}
                                    onChange={handleChange}
                                    placeholder='Digite a cidade'
                                 />
                              </div>

                              <div className='pre-cadastro-form-group'>
                                 <label htmlFor='estado'>Estado</label>
                                 <select id='estado' name='estado' value={formData.estado} onChange={handleChange}>
                                    <option value=''></option>
                                    <option value='ac'>Acre</option>
                                    <option value='al'>Alagoas</option>
                                    <option value='ap'>Amapá</option>
                                    <option value='am'>Amazonas</option>
                                    <option value='ba'>Bahia</option>
                                    <option value='ce'>Ceará</option>
                                    <option value='df'>Distrito Federal</option>
                                    <option value='es'>Espírito Santo</option>
                                    <option value='go'>Goiás</option>
                                    <option value='ma'>Maranhão</option>
                                    <option value='mt'>Mato Grosso</option>
                                    <option value='ms'>Mato Grosso do Sul</option>
                                    <option value='mg'>Minas Gerais</option>
                                    <option value='pa'>Pará</option>
                                    <option value='pb'>Paraíba</option>
                                    <option value='pr'>Paraná</option>
                                    <option value='pe'>Pernambuco</option>
                                    <option value='pi'>Piauí</option>
                                    <option value='rj'>Rio de Janeiro</option>
                                    <option value='rn'>Rio Grande do Norte</option>
                                    <option value='rs'>Rio Grande do Sul</option>
                                    <option value='ro'>Rondônia</option>
                                    <option value='rr'>Roraima</option>
                                    <option value='sc'>Santa Catarina</option>
                                    <option value='sp'>São Paulo</option>
                                    <option value='se'>Sergipe</option>
                                    <option value='to'>Tocantins</option>
                                 </select>
                              </div>
                           </div>

                           <div className='pre-cadastro-form-row'>
                              <div className='pre-cadastro-form-group'>
                                 <label htmlFor='numero'>Número</label>
                                 <input
                                    type='text'
                                    id='numero'
                                    name='numero'
                                    value={formData.numero}
                                    onChange={handleChange}
                                    placeholder='Número'
                                 />
                              </div>
                              <div className='pre-cadastro-form-group'>
                                 <label htmlFor='complemento'>Complemento</label>
                                 <input
                                    type='text'
                                    id='complemento'
                                    name='complemento'
                                    value={formData.complemento}
                                    onChange={handleChange}
                                    placeholder='Apartamento, bloco, etc.'
                                 />
                              </div>
                           </div>
                        </div>

                        {/* Seção: Observações */}
                        {/* <div className='pre-cadastro-form-section'>
                           <h2 className='pre-cadastro-section-title'>Observações</h2>
                           <div className='pre-cadastro-form-group'>
                              <textarea
                                 id='observacoes'
                                 name='observacoes'
                                 value={formData.observacoes}
                                 onChange={handleChange}
                                 placeholder='Digite observações adicionais...'
                                 rows='4'
                              ></textarea>
                           </div>
                        </div> */}

                        <button type='submit' className='pre-cadastro-submit-btn'>
                           Continuar
                        </button>
                     </form>
                  </main>
               </>
            )}

            {fichaPreenchida && (
               <>
                  <header className='pre-cadastro-header'>
                     <h1>Cadastro Dr. Hair</h1>
                     <br />
                     <p>Cadastro realizado com sucesso!</p>
                  </header>
               </>
            )}
         </div>
      </div>
   );
}

export default PreCadastro;

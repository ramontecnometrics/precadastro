import React, { useEffect, useRef, useState } from 'react';
import './../PreCadastro.css';
import api from '../utils/Api';
import { showError } from '../components/Messages';
import Formulario from './Formulario';
import { FlexRow, FlexCol } from '../components/FlexItems';
import IconButton from '../components/IconButton';
import { faArrowLeft } from '@fortawesome/free-solid-svg-icons';
import { LayoutParams } from '../config/LayoutParams';
import Filler from '../components/Filler';

function PreCadastro() {
   const [erro, setErro] = useState(null);
   const [formData, setFormData] = useState({});
   const [inicializado, setInicializado] = useState(false);
   const [parametros, setParametros] = useState(false);
   /* const [parametros, setParametros] = useState({
      unidade: { id: 2, nome: 'DR HAIR - ANCHIETA' },
      fichaDeAvaliacaoClinicaParaGeneroMasculino: {
         id: 1,
         nome: 'Ficha de Avaliação Clínica (Masculino)',
         descricao: 'Teste',
         grupos: [
            {
               id: 3,
               ordem: 1,
               titulo: 'Formulário',
               campos: [
                  {
                     id: 1,
                     ordem: 1,
                     titulo: 'Há quanto tempo você começou a perceber a queda de cabelo?',
                     tipo: 'texto',
                     obrigatorio: true,
                  },
                  { id: 3, ordem: 2, titulo: 'Foi de uma vez ou gradativa?', tipo: 'texto', obrigatorio: true },
                  {
                     id: 4,
                     ordem: 3,
                     titulo: 'Queixa principal (O que te incomoda)?',
                     tipo: 'texto',
                     obrigatorio: true,
                  },
                  {
                     id: 5,
                     ordem: 4,
                     titulo: 'Em caso de queda, ela está aumentando ou está estável?',
                     tipo: 'texto',
                     obrigatorio: true,
                  },
                  {
                     id: 6,
                     ordem: 5,
                     titulo: 'Histórico de queda ou calvície na família? Quem?',
                     tipo: 'texto',
                     obrigatorio: true,
                  },
                  {
                     id: 7,
                     ordem: 6,
                     titulo: 'Já fez algum tratamento capilar? Se sim, quais? Como foi o resultado? Há quanto tempo?',
                     tipo: 'texto',
                     obrigatorio: true,
                  },
                  {
                     id: 8,
                     ordem: 7,
                     titulo: 'Faz uso de algum medicamento contra queda do cabelo? Qual?',
                     tipo: 'texto',
                     obrigatorio: false,
                  },
                  { id: 9, ordem: 8, titulo: 'Algum medicamento em uso? Qual?', tipo: 'texto', obrigatorio: false },
                  { id: 10, ordem: 9, titulo: 'Faz uso de reposição hormonal?', tipo: 'texto', obrigatorio: true },
                  {
                     id: 11,
                     ordem: 10,
                     titulo: 'Pretende ter filhos nos próximos 12 meses?',
                     tipo: 'simnao',
                     obrigatorio: true,
                  },
                  { id: 12, ordem: 11, titulo: 'Uso de boné/chapéu?', tipo: 'simnao', obrigatorio: true },
                  { id: 13, ordem: 12, titulo: 'Uso de capacete?', tipo: 'simnao', obrigatorio: true },
                  { id: 14, ordem: 13, titulo: 'Faz dieta? Como é a dieta?', tipo: 'texto', obrigatorio: true },
                  { id: 15, ordem: 14, titulo: 'Cirurgia bariátrica?', tipo: 'simnao', obrigatorio: true },
                  { id: 16, ordem: 15, titulo: 'Atividade física? Frequência?', tipo: 'texto', obrigatorio: true },
                  { id: 17, ordem: 16, titulo: 'Ansiedade?', tipo: 'simnao', obrigatorio: true },
                  { id: 18, ordem: 17, titulo: 'Estresse?', tipo: 'simnao', obrigatorio: true },
                  { id: 19, ordem: 18, titulo: 'Depressão?', tipo: 'simnao', obrigatorio: true },
                  { id: 20, ordem: 19, titulo: 'Hipertensão?', tipo: 'simnao', obrigatorio: true },
                  { id: 21, ordem: 20, titulo: 'Diabetes?', tipo: 'simnao', obrigatorio: true },
                  { id: 22, ordem: 21, titulo: 'Tireoide?', tipo: 'simnao', obrigatorio: true },
                  {
                     id: 23,
                     ordem: 22,
                     titulo: 'Histórico de câncer na família? Se sim, quem?',
                     tipo: 'texto',
                     obrigatorio: true,
                  },
                  {
                     id: 24,
                     ordem: 23,
                     titulo: 'Histórico de doença autoimune? Se sim, qual?',
                     tipo: 'texto',
                     obrigatorio: true,
                  },
                  {
                     id: 25,
                     ordem: 24,
                     titulo: 'Condição dermatológica? Se sim, qual?',
                     tipo: 'texto',
                     obrigatorio: true,
                  },
                  { id: 26, ordem: 25, titulo: 'Outras patologias?', tipo: 'texto', obrigatorio: true },
               ],
            },
         ],
      },
      fichaDeAvaliacaoClinicaParaGeneroFeminino: {
         id: 2,
         nome: 'Ficha de Avaliação Clínica (Feminino)',
         grupos: [
            { id: 7, ordem: 1, titulo: 'sdfasd', campos: [] },
            { id: 8, ordem: 2, titulo: 'asdfasd', campos: [] },
         ],
      },
      unidades: [
         { id: 2, nome: 'DR HAIR - ANCHIETA' },
         { id: 1, nome: 'DR HAIR - CONTAGEM' },
      ],
   });*/
   const [cadastroRealizado, setCadastroRealizado] = useState(false);
   const [avaliacaoRealizada, setAvaliacaoRealizada] = useState(false);
   const [idDoLead, setIdDoLead] = useState(null);
   const [buscandoCEP, setBuscandoCEP] = useState(false);
   const [avaliacao, setAvaliacao] = useState(null);
   const [tokenParaAvaliacaoClinica, setTokenParaAvaliacaoClinica] = useState(null); 
   const containerRef = useRef(null);
   const firstInputRef = useRef(null);

   const aplicarMascaraTelefone = (valor) => {
      const apenasDigitos = valor.replace(/\D/g, '');

      if (apenasDigitos.length <= 10) {
         return apenasDigitos.replace(/(\d{2})(\d{0,4})(\d{0,4})/, '($1) $2-$3').trim();
      } else {
         return apenasDigitos.replace(/(\d{2})(\d{0,5})(\d{0,4})/, '($1) $2-$3').trim();
      }
   };

   const aplicarMascaraCPF = (valor) => {
      const apenasDigitos = valor.replace(/\D/g, '');
      return apenasDigitos.replace(/(\d{3})(\d{3})(\d{3})(\d{2})/, '$1.$2.$3-$4');
   };

   const aplicarMascaraCEP = (valor) => {
      const apenasDigitos = valor.replace(/\D/g, '');
      return apenasDigitos.replace(/(\d{5})(\d{0,3})/, '$1-$2');
   };

   const aplicarMascaraDeData = (valor) => {
      const apenasDigitos = valor.replace(/\D/g, '');
      return apenasDigitos.replace(/(\d{2})(\d{2})(\d{4})/, '$1/$2/$3');
   };

   const buscarCEP = async (cep) => {
      const cepLimpo = cep.replace(/\D/g, '');
      if (cepLimpo.length !== 8) {
         return;
      }
      setBuscandoCEP(true);
      try {
         const response = await fetch(`https://viacep.com.br/ws/${cepLimpo}/json/`);
         const data = await response.json();
         if (!data.erro) {
            setFormData((prevState) => ({
               ...prevState,
               logradouro: data.logradouro || '',
               bairro: data.bairro || '',
               cidade: data.localidade || '',
               estado: data.uf?.toLowerCase() || '',
               pais: 'brasil',
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
      } else if (name === 'dataNascimento') {
         valorProcessado = aplicarMascaraDeData(value);
      }

      setFormData((prevState) => ({
         ...prevState,
         [name]: type === 'checkbox' ? checked : type === 'file' ? files[0] : valorProcessado,
      }));

      // Busca CEP automaticamente quando o campo estiver completo
      if (name === 'cep' && valorProcessado.length === 9) {
         buscarCEP(valorProcessado);
      }

      if (name === 'genero') {
         const ficha = value
            ? value === '2'
               ? parametros.fichaDeAvaliacaoClinicaParaGeneroFeminino
               : parametros.fichaDeAvaliacaoClinicaParaGeneroMasculino
            : null;

         setAvaliacao(ficha);
      }
   };

   useEffect(() => {
      const params = new URL(location.href).searchParams;
      const id = params.get('id');
      const cpf = params.get('cpf');
      if (cpf){
         setFormData((prevState) => ({
            ...prevState,
            cpf: cpf,
         }));
      }
      api.get(`/lead/precadastro/parametros?id=${id}`, false, null, false, false)
         .then((result) => {
            setFormData((prevState) => ({
               ...prevState,
               idDaUnidade: result.unidade?.id,
            }));
            setParametros(result);
            setInicializado(true);
         })
         .catch((error) => {
            setErro(api.getErrorMessage(error));
         });
   }, []);

   const handleSubmitCadastro = (e) => {
      e.preventDefault();
      console.log('Dados do pré-cadastro:', formData);

      api.post('/lead/precadastro', formData)
         .then((response) => {
            setCadastroRealizado(true);
            setIdDoLead(response.idDoLead);
            setTokenParaAvaliacaoClinica(response.tokenParaAvaliacaoClinica);
         })
         .catch((error) => {});
   };

   const handleSubmitAvaliacaoClinica = (formulario) => {
      setAvaliacao(formulario);

      console.log('Dados da avaliação clínica:', formulario);

      const input = {
         idDoLead: idDoLead,
         fichaDeAvaliacao: formulario,
      };

      api.post('/lead/precadastro/avaliacaoclinica', input, null, null, {
         TokenParaAvaliacaoClinica: tokenParaAvaliacaoClinica,
      })
         .then((response) => {
            setAvaliacaoRealizada(true);
            setIdDoLead(response.idDoLead);
         })
         .catch((error) => {});
   };

   useEffect(() => {
      console.log('inicializado', inicializado);
      if (firstInputRef.current) {
         firstInputRef.current.focus();
      }
   }, [firstInputRef, inicializado]);

   return (
      <div className='pre-cadastro'>
         {erro && (
            <>
               <div ref={containerRef} className='pre-cadastro-container'>
                  <header className='pre-cadastro-header'>
                     <h1>Cadastro Dr. Hair</h1>
                  </header>
                  <main className='pre-cadastro-main-content'>
                     <div style={{ textAlign: 'center' }}>
                        <div style={{ margin: 'auto', width: 'fit-content' }}>
                           <span>{erro}</span>
                        </div>
                     </div>
                  </main>
               </div>
            </>
         )}

         {!erro && (
            <div ref={containerRef} className='pre-cadastro-container'>
               {!inicializado && (
                  <>
                     <header className='pre-cadastro-header'>
                        <h1>Cadastro Dr. Hair</h1>
                        <p>Preparando...</p>
                     </header>
                     <main className='pre-cadastro-main-content'>
                        <div style={{ textAlign: 'center' }}>
                           <div style={{ margin: 'auto', width: 'fit-content' }}>
                              <div className='spinner'></div>
                           </div>
                        </div>
                     </main>
                  </>
               )}

               {inicializado && !formData.idDaUnidade && parametros.unidades && (
                  <div>
                     <header className='pre-cadastro-header'>
                        <h1>Cadastro Dr. Hair</h1>
                     </header>
                     <main className='pre-cadastro-main-content'>
                        <div className='pre-cadastro-form-group'>
                           <label htmlFor='genero'>Unidade</label>
                           <select
                              id='unidade'
                              name='unidade'
                              value={formData.idDaUnidade}
                              onChange={(e) => {
                                 setFormData((prevState) => ({ ...prevState, idDaUnidade: e.target.value }));
                              }}
                              required
                           >
                              <option value=''></option>
                              {parametros.unidades.map((i, index) => {
                                 return <option value={i.id}>{i.nome}</option>;
                              })}
                           </select>
                        </div>
                     </main>
                  </div>
               )}

               {inicializado && !cadastroRealizado && !avaliacaoRealizada && formData.idDaUnidade && (
                  <>
                     <header className='pre-cadastro-header'>
                        <h1>Cadastro Dr. Hair</h1>
                        <p>Preencha os dados abaixo para realizar seu cadastro</p>
                     </header>
                     <main className='pre-cadastro-main-content'>
                        <form onSubmit={handleSubmitCadastro} className='pre-cadastro-form'>
                           {/* Seção: Dados básicos */}
                           <div className='pre-cadastro-form-section'>
                              <h2 className='pre-cadastro-section-title'>Dados básicos</h2>

                              <div className='pre-cadastro-form-row'>
                                 <div className='pre-cadastro-form-group full'>
                                    <label htmlFor='nome'>Nome *</label>
                                    <input
                                       ref={firstInputRef}
                                       type='text'
                                       id='nome'
                                       name='nome'
                                       value={formData.nome}
                                       onChange={handleChange}
                                       required
                                       placeholder='Digite seu nome completo'
                                    />
                                 </div>
                              </div>
                              <div className='pre-cadastro-form-row'>
                                 <div className='pre-cadastro-form-group '>
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
                                       type='text'
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
                                    <input
                                       type='text'
                                       id='profissao'
                                       name='profissao'
                                       value={formData.profissao}
                                       onChange={handleChange} 
                                    />
                                 </div>
                              </div>

                              <div className='pre-cadastro-form-row'>
                                 <div className='pre-cadastro-form-group full'>
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
                           </div>

                           {/* <div className='pre-cadastro-form-group'>
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
                                 </div> */}

                           {/* 
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
                              </div> 
                           </div>
                           */}

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

                     <div className='show-on-small-screen'>
                        <Filler height={100} />
                     </div>
                  </>
               )}

               {idDoLead && cadastroRealizado && !avaliacaoRealizada && (
                  <>
                     <FlexRow>
                        <FlexCol width='50px'>
                           <h1>
                              <IconButton
                                 icon={faArrowLeft}
                                 onClick={() => setCadastroRealizado(false)}
                                 style={{ fontSize: 30, color: LayoutParams.colors.corSecundaria }}
                                 title={'Voltar'}
                              />
                           </h1>
                        </FlexCol>
                        <FlexCol>
                           <header className='pre-cadastro-header'>
                              <h1>Cadastro Dr. Hair</h1>
                              <p>Avaliação clínica</p>
                           </header>
                        </FlexCol>
                     </FlexRow>
                     <Formulario
                        nome='avaliacaoClinica'
                        // formulario={avaliacao}
                        formulario={parametros.fichaDeAvaliacaoClinicaParaGeneroMasculino}
                        setFormulario={handleSubmitAvaliacaoClinica}
                        mostrarTitulo={false}
                        containerRef={containerRef}
                     />

                     <div className='show-on-small-screen'>
                        <Filler height={100} />
                     </div>
                  </>
               )}

               {avaliacaoRealizada && (
                  <>
                     <header className='pre-cadastro-header'>
                        <h1>Cadastro Dr. Hair</h1>
                        <br />
                        <p>Cadastro realizado com sucesso!</p>
                     </header>
                  </>
               )}
            </div>
         )}
      </div>
   );
}

export default PreCadastro;

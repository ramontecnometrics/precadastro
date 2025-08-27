import React, { useState } from 'react';
import './../PreCadastro.css';
import Text from '../components/Text';

function PreCadastro() {
   const [formData, setFormData] = useState({
      // Dados básicos
      celular: '',
      nome: '',
      genero: '',

      // Informações adicionais
      dataNascimento: '',
      estadoCivil: '',
      profissao: '',
      nacionalidade: '',
      telefone: '',
      email: '',
      cpf: '',
      rg: '',
      cnh: '',

      // Endereço
      pais: 'brasil',
      cep: '',
      bairro: '',
      estado: '',
      logradouro: '',
      numero: '',
      complemento: '',
      cidade: '',

      // Configurações
      foto: null,
      ativo: false,

      // Observações
      observacoes: '',
   });

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
            alert('CEP não encontrado. Verifique o número digitado.');
         }
      } catch (error) {
         console.error('Erro ao buscar CEP:', error);
         alert('Erro ao buscar CEP. Tente novamente.');
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
      alert('Pré-cadastro enviado com sucesso!');
      // Reset do formulário seria feito aqui
   };

   return (
      <div className='App'>
         <div className='container'>
            <header className='header'>
               <h1>Sistema de Pré-Cadastro</h1>
               <p>Preencha os dados abaixo para realizar seu pré-cadastro</p>
            </header>

            <main className='main-content'>
               <form onSubmit={handleSubmit} className='form'>
                  {/* Seção: Dados básicos */}
                  <div className='form-section'>
                     <h2 className='section-title'>Dados Básicos</h2>
                     <div className='form-row'>
                        <div className='form-group'>
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
                        <div className='form-group'>
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
                     </div>
                     <div className='form-group'>
                        <label htmlFor='genero'>Gênero *</label>
                        <select id='genero' name='genero' value={formData.genero} onChange={handleChange} required>
                           <option value=''>Selecione o gênero</option>
                           <option value='masculino'>Masculino</option>
                           <option value='feminino'>Feminino</option>
                           <option value='outro'>Outro</option>
                        </select>
                     </div>
                  </div>

                  {/* Seção: Informações adicionais */}
                  <div className='form-section'>
                     <h2 className='section-title'>Informações Adicionais</h2>
                     <div className='form-row'>
                        <div className='form-group'>
                           <label htmlFor='dataNascimento'>Data de Nascimento</label>
                           <input
                              type='date'
                              id='dataNascimento'
                              name='dataNascimento'
                              value={formData.dataNascimento}
                              onChange={handleChange}
                           />
                        </div>
                        <div className='form-group'>
                           <label htmlFor='estadoCivil'>Estado Civil</label>
                           <select
                              id='estadoCivil'
                              name='estadoCivil'
                              value={formData.estadoCivil}
                              onChange={handleChange}
                           >
                              <option value=''>Selecione o estado civil</option>
                              <option value='solteiro'>Solteiro(a)</option>
                              <option value='casado'>Casado(a)</option>
                              <option value='divorciado'>Divorciado(a)</option>
                              <option value='viuvo'>Viúvo(a)</option>
                              <option value='separado'>Separado(a)</option>
                           </select>
                        </div>
                     </div>
                     <div className='form-group'>
                        <label htmlFor='profissao'>Profissão</label>
                        <input
                           type='text'
                           id='profissao'
                           name='profissao'
                           value={formData.profissao}
                           onChange={handleChange}
                           placeholder='Digite sua profissão'
                        />
                     </div>
                     <div className='form-group'>
                        <label htmlFor='nacionalidade'>Nacionalidade *</label>
                        <select
                           id='nacionalidade'
                           name='nacionalidade'
                           value={formData.nacionalidade}
                           onChange={handleChange}
                           required
                        >
                           <option value=''>Selecione a nacionalidade</option>
                           <option value='brasileira'>Brasileira</option>
                           <option value='estrangeira'>Estrangeira</option>
                        </select>
                     </div>
                     <div className='form-row'>
                        <div className='form-group'>
                           <label htmlFor='telefone'>Telefone</label>
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
                        <div className='form-group'>
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
                     <div className='form-row'>
                        <div className='form-group'>
                           <label htmlFor='cpf'>CPF</label>
                           <input
                              type='text'
                              id='cpf'
                              name='cpf'
                              value={formData.cpf}
                              onChange={handleChange}
                              placeholder='000.000.000-00'
                              maxLength='14'
                           />
                        </div>
                        <div className='form-group'>
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
                     </div>
                     <div className='form-group'>
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

                  {/* Seção: Endereço */}
                  <div className='form-section'>
                     <h2 className='section-title'>Endereço</h2>
                     <div className='form-row'>
                        <div className='form-group'>
                           <label htmlFor='pais'>País</label>
                           <select id='pais' name='pais' value={formData.pais} onChange={handleChange}>
                              <option value='brasil'>Brasil</option>
                           </select>
                        </div>
                        <div className='form-group'>
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
                     <div className='form-group'>
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
                     <div className='form-row'>
                        <div className='form-group'>
                           <label htmlFor='estado'>Estado</label>
                           <select id='estado' name='estado' value={formData.estado} onChange={handleChange}>
                              <option value=''>Selecione o estado</option>
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
                        <div className='form-group'>
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
                     </div>
                     <div className='form-row'>
                        <div className='form-group'>
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
                        <div className='form-group'>
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
                     </div>
                     <div className='form-group'>
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

                  {/* Seção: Configurações */}
                  <div className='form-section'>
                     <h2 className='section-title'>Configurações</h2>
                     <div className='form-group'>
                        <label htmlFor='foto'>Foto</label>
                        <input type='file' id='foto' name='foto' onChange={handleChange} accept='image/*' />
                     </div>
                     <div className='form-group checkbox-group'>
                        <label className='checkbox-label'>
                           <input type='checkbox' name='ativo' checked={formData.ativo} onChange={handleChange} />
                           <Text className='checkmark'></Text>
                           Ativo
                        </label>
                     </div>
                  </div>

                  {/* Seção: Observações */}
                  <div className='form-section'>
                     <h2 className='section-title'>Observações</h2>
                     <div className='form-group'>
                        <label htmlFor='observacoes'>Observações</label>
                        <textarea
                           id='observacoes'
                           name='observacoes'
                           value={formData.observacoes}
                           onChange={handleChange}
                           placeholder='Digite observações adicionais...'
                           rows='4'
                        ></textarea>
                     </div>
                  </div>

                  <button type='submit' className='submit-btn'>
                     Enviar Pré-Cadastro
                  </button>
               </form>
            </main>
         </div>
      </div>
   );
}

export default PreCadastro;

import { useEffect, useState } from 'react';
import './../PreCadastro.css';

function Formulario({ nome, formulario, setFormulario, mostrarTitulo, containerRef }) {
   const [formData, setFormData] = useState(formulario ?? null);

   useEffect(() => {
      containerRef.current.scrollTo(0, 0);
   }, []);

   if (!formData) return null;

   const handleChange = (grupoIndex, campoIndex, valor) => {
      setFormData((prev) => {
         const novo = structuredClone(prev); // ou JSON.parse/stringify se não tiver polyfill
         novo.grupos[grupoIndex].campos[campoIndex].valor = valor;
         return novo;
      });
   };

   const handleSubmit = (e) => {
      e.preventDefault();
      setFormulario(formData);
   };

   const getOpcoes = (opcoes) => {
      let result = [];

      if (opcoes) {
         let count = opcoes.split('|').length - 1;
         result = opcoes.split('|', count);
      }
      return result;
   };

   const getOpcaoMarcada = (opcao, valor) => {
      if (!valor) {
         return false;
      }

      if (valor.split('|').filter((item) => item === opcao).length > 0) {
         return true;
      } else {
         return false;
      }
   };

   const adicionarOpcao = (opcao, valor, acao) => {
      let result = '';

      if (acao) {
         result = (valor || '') + '|' + opcao;
      } else {
         result = valor
            .split('|')
            .filter((item) => item !== opcao)
            .join('|');
      }
      console.log(result);
      return result;
   };

   return (
      <>
         <form onSubmit={handleSubmit} className='pre-cadastro-form'>
            {formData.grupos.map((grupo, grupoIndex) => (
               <div key={grupo.id} className='pre-cadastro-grupo'>
                  {grupo.titulo && mostrarTitulo && (
                     <div style={{ marginBottom: '25px', borderLeft: '2px solid #d4af37', paddingLeft: '10px' }}>
                        <span style={{ fontWeight: '600', fontSize: 18 }}>{grupo.titulo}</span>
                     </div>
                  )}

                  {grupo.campos.map((campo, campoIndex) => {
                     const nomeDoCampo = `${nome}-grupo-${grupo.id}-campo-${campo.id}`;

                     let result = null;

                     if (campo.tipo === 'texto') {
                        result = (
                           <div key={campo.id} className='pre-cadastro-form-group' style={{ marginBottom: '25px' }}>
                              <label htmlFor={nomeDoCampo}>{campo.titulo + (campo.obrigatorio ? ' *' : '')}</label>
                              <input
                                 type='text'
                                 id={nomeDoCampo}
                                 name={nomeDoCampo}
                                 value={formData.grupos[grupoIndex].campos[campoIndex].valor || ''}
                                 onChange={(e) => handleChange(grupoIndex, campoIndex, e.target.value)}
                                 required={campo.obrigatorio}
                              />
                           </div>
                        );
                     } else if (campo.tipo === 'simnao') {
                        result = (
                           <div key={campo.id} className='pre-cadastro-form-group' style={{ marginBottom: '25px' }}>
                              <label htmlFor={nomeDoCampo}>{campo.titulo + (campo.obrigatorio ? ' *' : '')}</label>
                              <div style={{ display: 'flex', flexDirection: 'row', gap: '10px', flexWrap: 'wrap' }}>
                                 <label className='radio-label' style={{ cursor: 'pointer' }}>
                                    <input
                                       type='radio'
                                       id={`${nomeDoCampo}-sim`}
                                       name={nomeDoCampo}
                                       value='Sim'
                                       checked={formData.grupos[grupoIndex].campos[campoIndex].valor === 'Sim'}
                                       onChange={(e) => handleChange(grupoIndex, campoIndex, e.target.value)}
                                       required={campo.obrigatorio}
                                    />
                                    &nbsp;Sim
                                 </label>

                                 <label className='radio-label'>
                                    <input
                                       type='radio'
                                       id={`${nomeDoCampo}-nao`}
                                       name={nomeDoCampo}
                                       value='Não'
                                       checked={formData.grupos[grupoIndex].campos[campoIndex].valor === 'Não'}
                                       onChange={(e) => handleChange(grupoIndex, campoIndex, e.target.value)}
                                       required={campo.obrigatorio}
                                    />
                                    &nbsp;Não
                                 </label>
                              </div>
                           </div>
                        );
                     } else if (campo.tipo === 'opcoes') {
                        result = (
                           <div key={campo.id} className='pre-cadastro-form-group' style={{ marginBottom: '25px' }}>
                              <label htmlFor={nomeDoCampo}>{campo.titulo + (campo.obrigatorio ? ' *' : '')}</label>
                              <div style={{ display: 'flex', flexDirection: 'row', gap: '10px', flexWrap: 'wrap' }}>
                                 {getOpcoes(campo.opcoes).map((opcao, index) => (
                                    <label className='radio-label' style={{ cursor: 'pointer' }}>
                                       <input
                                          type='radio'
                                          id={nomeDoCampo}
                                          name={nomeDoCampo}
                                          value={formData.grupos[grupoIndex].campos[campoIndex].valor || ''}
                                          onChange={(e) => {
                                             handleChange(grupoIndex, campoIndex, opcao);
                                          }}
                                          required={campo.obrigatorio}
                                       />
                                       &nbsp;
                                       {opcao}
                                    </label>
                                 ))}
                                 {!campo.obrigatorio && (
                                    <label className='radio-label' style={{ cursor: 'pointer' }}>
                                       <input
                                          type='radio'
                                          id={nomeDoCampo}
                                          name={nomeDoCampo}
                                          value={formData.grupos[grupoIndex].campos[campoIndex].valor || ''}
                                          onChange={(e) => {
                                             handleChange(grupoIndex, campoIndex, '');
                                          }}
                                          required={campo.obrigatorio}
                                       />
                                       &nbsp;
                                       {'Não informado'}
                                    </label>
                                 )}
                              </div>
                           </div>
                        );
                     } else if (campo.tipo === 'opcoesmultiplas') {
                        result = (
                           <div key={campo.id} className='pre-cadastro-form-group' style={{ marginBottom: '25px' }}>
                              <label htmlFor={nomeDoCampo}>{campo.titulo + (campo.obrigatorio ? ' *' : '')}</label>
                              <div style={{ display: 'flex', flexDirection: 'row', gap: '10px', flexWrap: 'wrap' }}>
                                 {getOpcoes(campo.opcoes).map((opcao, index) => (
                                    <label className='radio-label' style={{ cursor: 'pointer' }}>
                                       <input
                                          type='checkbox'
                                          id={nomeDoCampo}
                                          name={nomeDoCampo}
                                          value={getOpcaoMarcada(
                                             opcao,
                                             formData.grupos[grupoIndex].campos[campoIndex].valor
                                          )}
                                          onChange={(e) => {
                                             handleChange(
                                                grupoIndex,
                                                campoIndex,
                                                adicionarOpcao(
                                                   opcao,
                                                   formData.grupos[grupoIndex].campos[campoIndex].valor,
                                                   e.target.checked
                                                )
                                             );
                                             console.log(e.target.value);
                                          }}
                                       />
                                       &nbsp;
                                       {opcao}
                                    </label>
                                 ))}
                              </div>
                           </div>
                        );
                     } else if (campo.tipo === 'selecao') {
                        result = (
                           <div key={campo.id} className='pre-cadastro-form-group' style={{ marginBottom: '25px' }}>
                              <label htmlFor={nomeDoCampo}>{campo.titulo + (campo.obrigatorio ? ' *' : '')}</label>
                              <select
                                 id={nomeDoCampo}
                                 name={nomeDoCampo}
                                 value={formData.grupos[grupoIndex].campos[campoIndex].valor || ''}
                                 onChange={(e) => handleChange(grupoIndex, campoIndex, e.target.value)}
                                 required={campo.obrigatorio}
                              >
                                 <option value={''}></option>
                                 {getOpcoes(campo.opcoes).map((opcao, index) => (
                                    <option key={index} value={opcao}>
                                       {opcao}
                                    </option>
                                 ))}
                              </select>
                           </div>
                        );
                     }

                     return result;
                  })}
               </div>
            ))}

            <button type='submit' className='pre-cadastro-submit-btn'>
               Enviar
            </button>
         </form>
      </>
   );
}

export default Formulario;

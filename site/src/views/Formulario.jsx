import { useEffect, useState } from 'react';
import './../PreCadastro.css';

function Formulario({ formulario, setFormulario, mostrarTitulo, containerRef }) {
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

   return (
      <>
         <form onSubmit={handleSubmit} className='pre-cadastro-form'>
            {formData.grupos.map((grupo, grupoIndex) => (
               <div key={grupo.id} className='pre-cadastro-grupo'>
                  {grupo.titulo && mostrarTitulo && <span>{grupo.titulo}</span>}

                  {grupo.campos.map((campo, campoIndex) => {
                     const nomeDoCampo = `grupo-${grupo.id}-campo-${campo.id}`;

                     let result = null;

                     if (campo.tipo === 'texto') {
                        result = (
                           <div key={campo.id} className='pre-cadastro-form-group'>
                              <label htmlFor={nomeDoCampo}>{campo.titulo}</label>
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
                           <div key={campo.id} className='pre-cadastro-form-group'>
                              <label htmlFor={nomeDoCampo}>{campo.titulo}</label>
                              <div>
                                 <label>
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

                                 <label style={{ marginLeft: '10px' }}>
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
